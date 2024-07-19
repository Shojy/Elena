using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shojy.FF7.Elena.Battle;
using Shojy.FF7.Elena.Characters;

namespace Shojy.FF7.Elena.Sections
{
    /// <summary>
    /// The battle and growth section. This goes not include character data,
    /// which can instead be found in CharacterData.
    /// </summary>
    public class BattleAndGrowthData
    {
        #region Private Fields

        private byte[] _sectionData;
        private const int
            MinStatModifier = 1,
            MaxStatModifier = 8,
            BracketCount = 12,
            StatCurveCount = 64,
            RNGTableSize = 256,
            LookupTableSize = 64,
            IndexedSpellCount = 56;

        #endregion

        #region Public Constructors

        public BattleAndGrowthData(byte[] sectionData)
        {
            _sectionData = sectionData;
            ParseData(sectionData);
        }

        #endregion

        #region Public Properties

        public byte[] RandomBonusToPrimaryStats { get; } = new byte[BracketCount];
        public byte[] RandomBonusToHP { get; } = new byte[BracketCount];
        public byte[] RandomBonusToMP { get; } = new byte[BracketCount];
        public StatCurve[] StatCurves { get; } = new StatCurve[StatCurveCount];
        public byte[] RNGTable { get; } = new byte[RNGTableSize];
        public byte[] SceneLookupTable { get; } = new byte[LookupTableSize];
        public SpellIndex[] SpellIndexes { get; } = new SpellIndex[IndexedSpellCount];

        #endregion

        #region Public Methods

        public int[] CalculateMinStats(Character chara, CurveStats stat)
        {
            return CalcStats(chara, stat, MinStatModifier);
        }

        public int[] CalculateMaxStats(Character chara, CurveStats stat)
        {
            return CalcStats(chara, stat, MaxStatModifier);
        }

        #endregion

        #region Private Methods

        private void ParseData(byte[] data)
        {
            for (int i = 0; i < BracketCount; i++)
            {
                RandomBonusToPrimaryStats[i] = data[0x1F8 + i];
            }
            for (int i = 0; i < BracketCount; i++)
            {
                RandomBonusToHP[i] = data[0x204 + i];
            }
            for (int i = 0; i < BracketCount; i++)
            {
                RandomBonusToMP[i] = data[0x210 + i];
            }

            var temp = new byte[16];
            for (int i = 0; i < StatCurveCount; i++)
            {
                Array.Copy(data, 0x21C + (i * 16), temp, 0, 16);
                StatCurves[i] = new StatCurve();
                StatCurves[i].ParseData(temp);
            }

            Array.Copy(data, 0xE1C, RNGTable, 0, RNGTableSize);
            Array.Copy(data, 0xF1C, SceneLookupTable, 0, LookupTableSize);

            for (byte i = 0; i < IndexedSpellCount; i++)
            {
                SpellIndexes[i] = new SpellIndex(i, data[0xF5C + i]);
            }
        }

        private int GetBaseStat(Character chara, CurveStats stat)
        {
            switch (stat) //get base stat
            {
                case CurveStats.Strength:
                    return chara.Strength;
                case CurveStats.Vitality:
                    return chara.Vitality;
                case CurveStats.Magic:
                    return chara.Magic;
                case CurveStats.Spirit:
                    return chara.Spirit;
                case CurveStats.Dexterity:
                    return chara.Dexterity;
                case CurveStats.Luck:
                    return chara.Luck;
                case CurveStats.HP:
                    return chara.BaseHP;
                case CurveStats.MP:
                    return chara.BaseMP;
                default:
                    return 0;
            }
        }

        private byte GetCurveIndex(Character chara, CurveStats stat)
        {
            switch (stat)
            {
                case CurveStats.Strength:
                    return chara.StrengthCurveIndex;
                case CurveStats.Vitality:
                    return chara.VitalityCurveIndex;
                case CurveStats.Magic:
                    return chara.MagicCurveIndex;
                case CurveStats.Spirit:
                    return chara.SpiritCurveIndex;
                case CurveStats.Dexterity:
                    return chara.DexterityCurveIndex;
                case CurveStats.Luck:
                    return chara.LuckCurveIndex;
                case CurveStats.HP:
                    return chara.HPCurveIndex;
                case CurveStats.MP:
                    return chara.MPCurveIndex;
                case CurveStats.EXP:
                    return chara.EXPCurveIndex;
            }
            return 0;
        }

        private int GetBracket(int level)
        {
            if (level < 12) { return 0; }
            else if (level < 22) { return 1; }
            else if (level < 32) { return 2; }
            else if (level < 42) { return 3; }
            else if (level < 52) { return 4; }
            else if (level < 62) { return 5; }
            else if (level < 82) { return 6; }
            else { return 7; }
        }

        private int CalcStatGain(Character chara, int level, int baseLevel, CurveStats stat, int prevStat,
            int rnd)
        {
            //thanks to DLPB for helping with these formulas!

            int bracket = GetBracket(level);
            var curve = StatCurves[GetCurveIndex(chara, stat)];

            //calculate the stat gain
            if (level <= baseLevel) //don't calc at start level
            {
                return 0;
            }
            else if (stat == CurveStats.EXP) //special calc for EXP
            {
                return curve.Gradients[bracket] * ((level - 1) * (level - 1)) / 10;
            }
            else //everything else
            {
                int baseline = CalcBaseline(stat, curve, level),
                    difference = CalcDifference(stat, baseline, prevStat, rnd);

                if (stat == CurveStats.HP)
                {
                    return (RandomBonusToHP[difference] * curve.Gradients[bracket]) / 100;
                }
                else if (stat == CurveStats.MP)
                {
                    return RandomBonusToMP[difference] * ((level * curve.Gradients[bracket] / 10) -
                        ((level - 1) * curve.Gradients[bracket] / 10)) / 100;
                }
                else
                {
                    return RandomBonusToPrimaryStats[difference];
                }
            }
        }

        private int CalcBaseline(CurveStats stat, StatCurve curve, int level)
        {
            int bracket = GetBracket(level);
            if (stat == CurveStats.HP)
            {
                return (curve.Bases[bracket] * 40) + (level - 1) * curve.Gradients[bracket];
            }
            else if (stat == CurveStats.MP)
            {
                return (curve.Bases[bracket] * 2) + ((level - 1) * curve.Gradients[bracket] / 10);
            }
            else
            {
                return curve.Bases[bracket] + (curve.Gradients[bracket] * level / 100);
            }
        }

        private int CalcDifference(CurveStats stat, int baseline, int prevStat, int rnd)
        {
            int difference = 0;
            if (stat == CurveStats.HP || stat == CurveStats.MP)
            {
                difference = rnd + (100 * baseline / prevStat) - 100;
            }
            else
            {
                difference = rnd + baseline - prevStat;
            }
            if (difference < 0) { difference = 0; }
            if (difference > 11) { difference = 11; }
            return difference;
        }

        private int[] CalcStats(Character chara, CurveStats stat, int modifier)
        {
            var stats = new int[99];
            int currStat = GetBaseStat(chara, stat);
            var brackets = new int[8];

            for (int i = chara.Level; i <= 99; ++i)
            {
                if (stat == CurveStats.EXP)
                {
                    for (int j = 0; j < 8; ++j)
                    {
                        brackets[j] += CalcStatGain(chara, i, 1, stat, 0, 0);
                    }
                    currStat = brackets[GetBracket(i)];
                }
                else
                {
                    currStat += CalcStatGain(chara, i, chara.Level, stat, currStat, modifier);
                    if (stat == CurveStats.HP)
                    {
                        if (currStat > 9999) { currStat = 9999; }
                    }
                    else if (stat == CurveStats.MP)
                    {
                        if (currStat > 999) { currStat = 999; }

                    }
                    else if (currStat > 100) { currStat = 100; }
                }
                stats[i - 1] = currStat;
            }
            return stats;
        }

        #endregion
    }
}
