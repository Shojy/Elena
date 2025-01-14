using Shojy.FF7.Elena.Characters;
using Shojy.FF7.Elena.Extensions;
using Shojy.FF7.Elena.Inventory;
using System;

namespace Shojy.FF7.Elena.Sections
{
    /// <summary>
    /// This is merged character data from the initial data and growth data sections.
    /// </summary>
    public class CharacterData
    {
        #region Private Fields

        private readonly Character[] characterList = new Character[9];
        private const int CharacterRecordLength = 132, GrowthDataLength = 56, AILength = 2048;

        #endregion

        #region Public Constructors

        public CharacterData(byte[] initialData, byte[] growthData)
        {
            byte[] initial = new byte[CharacterRecordLength],
                growth = new byte[GrowthDataLength];

            for (int i = 0; i < 9; i++)
            {
                characterList[i] = new Character();
                Array.Copy(
                    initialData,
                    CharacterRecordLength * i,
                    initial,
                    0,
                    CharacterRecordLength);
                ParseInitialData(characterList[i], initial);
                Array.Copy(
                    growthData,
                    GrowthDataLength * i,
                    growth,
                    0,
                    GrowthDataLength);
                ParseGrowthData(characterList[i], growth);
            }
            Cloud = characterList[0];
            Barret = characterList[1];
            Tifa = characterList[2];
            Aerith = characterList[3];
            RedXIII = characterList[4];
            Yuffie = characterList[5];
            CaitSithYoungCloud = characterList[6];
            VincentSephiroth = characterList[7];
            Cid = characterList[8];

            Array.Copy(growthData, 0x61C, CharacterAIBlock, 0, AILength);
        }

        #endregion

        #region Public Properties

        public Character Cloud { get; }
        public Character Barret { get; }
        public Character Tifa { get; }
        public Character Aerith { get; }
        public Character RedXIII { get; }
        public Character Yuffie { get; }
        public Character CaitSithYoungCloud { get; }
        public Character VincentSephiroth { get; }
        public Character Cid { get; }
        public byte[] CharacterAIBlock { get; } = new byte[AILength];

        #endregion

        #region Private Methods

        private static void ParseInitialData(Character c, byte[] data)
        {
            c.ID = data[0x0];
            c.Level = data[0x1];
            c.Strength = data[0x2];
            c.Vitality = data[0x3];
            c.Magic = data[0x4];
            c.Spirit = data[0x5];
            c.Dexterity = data[0x6];
            c.Luck = data[0x7];
            c.StrengthBonus = data[0x8];
            c.VitalityBonus = data[0x9];
            c.MagicBonus = data[0xA];
            c.SpiritBonus = data[0xB];
            c.DexterityBonus = data[0xC];
            c.LuckBonus = data[0xD];
            c.LimitLevel = data[0xE];
            c.CurrentLimitBar = data[0xF];

            var temp = new byte[12];
            Array.Copy(data, 0x10, temp, 0, 12);
            c.Name = temp.ToFFString();

            c.WeaponID = data[0x1C]; 
            c.ArmorID = data[0x1D];
            c.AccessoryID = data[0x1E];
            c.CharacterFlags = (CharacterFlags)data[0x1F];
            c.IsBackRow = data[0x20] == 0xFE;
            c.LevelProgressBar = data[0x21];
            c.LearnedLimits = (LearnedLimits)BitConverter.ToUInt16(data, 0x22);
            c.KillCount = BitConverter.ToUInt16(data, 0x24);
            c.Limit1Uses = BitConverter.ToUInt16(data, 0x26);
            c.Limit2Uses = BitConverter.ToUInt16(data, 0x28);
            c.Limit3Uses = BitConverter.ToUInt16(data, 0x2A);
            c.CurrentHP = BitConverter.ToUInt16(data, 0x2C);
            c.BaseHP = BitConverter.ToUInt16(data, 0x2E);
            c.CurrentMP = BitConverter.ToUInt16(data, 0x30);
            c.BaseMP = BitConverter.ToUInt16(data, 0x32);
            c.MaxHP = BitConverter.ToUInt16(data, 0x38);
            c.MaxMP = BitConverter.ToUInt16(data, 0x3A);
            c.CurrentEXP = BitConverter.ToUInt32(data, 0x3C);

            temp = new byte[4];
            for (int i = 0; i < 8; ++i)
            {
                c.WeaponMateria[i] = new InventoryMateria();
                Array.Copy(data, 0x40 + (i * 4), temp, 0, 4);
                c.WeaponMateria[i].ParseData(temp);
            }
            for (int i = 0; i < 8; ++i)
            {
                c.ArmorMateria[i] = new InventoryMateria();
                Array.Copy(data, 0x60 + (i * 4), temp, 0, 4);
                c.ArmorMateria[i].ParseData(temp);
            }
            c.EXPtoNextLevel = BitConverter.ToUInt32(data, 0x80);
        }

        private void ParseGrowthData(Character c, byte[] data)
        {
            c.StrengthCurveIndex = data[0x0];
            c.VitalityCurveIndex = data[0x1];
            c.MagicCurveIndex = data[0x2];
            c.SpiritCurveIndex = data[0x3];
            c.DexterityCurveIndex = data[0x4];
            c.LuckCurveIndex = data[0x5];
            c.HPCurveIndex = data[0x6];
            c.MPCurveIndex = data[0x7];
            c.EXPCurveIndex = data[0x8];
            c.RecruitLevelOffset = (sbyte)((sbyte)data[0xA] / 2);
            c.Limit1_1Index = data[0xC];
            c.Limit1_2Index = data[0xD];
            c.Limit2_1Index = data[0xF];
            c.Limit2_2Index = data[0x10];
            c.Limit3_1Index = data[0x12];
            c.Limit3_2Index = data[0x13];
            c.Limit4Index = data[0x15];
            c.KillsForLimitLv2 = BitConverter.ToUInt16(data, 0x18);
            c.KillsForLimitLv3 = BitConverter.ToUInt16(data, 0x1A);
            c.UsesForLimit1_2 = BitConverter.ToUInt16(data, 0x1C);
            c.UsesForLimit2_2 = BitConverter.ToUInt16(data, 0x20);
            c.UsesForLimit3_2 = BitConverter.ToUInt16(data, 0x24);
            c.LimitLv1HPDivisor = BitConverter.ToUInt32(data, 0x28);
            c.LimitLv2HPDivisor = BitConverter.ToUInt32(data, 0x2C);
            c.LimitLv3HPDivisor = BitConverter.ToUInt32(data, 0x30);
            c.LimitLv4HPDivisor = BitConverter.ToUInt32(data, 0x34);
        }

        #endregion
    }
}
