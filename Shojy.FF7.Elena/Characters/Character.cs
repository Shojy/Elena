using Shojy.FF7.Elena.Inventory;
using Shojy.FF7.Elena.Text;

namespace Shojy.FF7.Elena.Characters
{
    public class Character
    {
        //initial data
        public FFText Name { get; set; }
        public byte ID { get; set; }
        public byte Level { get; set; }
        public byte Strength { get; set; }
        public byte Vitality { get; set; }
        public byte Magic { get; set; }
        public byte Spirit { get; set; }
        public byte Dexterity { get; set; }
        public byte Luck { get; set; }
        public byte StrengthBonus { get; set; }
        public byte VitalityBonus { get; set; }
        public byte MagicBonus { get; set; }
        public byte SpiritBonus { get; set; }
        public byte DexterityBonus { get; set; }
        public byte LuckBonus { get; set; }
        public byte LimitLevel { get; set; }
        public byte CurrentLimitBar { get; set; }
        public byte WeaponID { get; set; }
        public byte ArmorID { get; set; }
        public byte AccessoryID { get; set; }
        public CharacterFlags CharacterFlags { get; set; }
        public bool IsBackRow { get; set; }
        public byte LevelProgressBar { get; set; }
        public LearnedLimits LearnedLimits { get; set; }
        public ushort KillCount { get; set; }
        public ushort Limit1Uses { get; set; }
        public ushort Limit2Uses { get; set; }
        public ushort Limit3Uses { get; set; }
        public ushort CurrentHP { get; set; }
        public ushort BaseHP { get; set; }
        public ushort MaxHP { get; set; }
        public ushort CurrentMP { get; set; }
        public ushort BaseMP { get; set; }
        public ushort MaxMP { get; set; }
        public uint CurrentEXP { get; set; }
        public InventoryMateria[] WeaponMateria { get; } = new InventoryMateria[8];
        public InventoryMateria[] ArmorMateria { get; } = new InventoryMateria[8];
        public uint EXPtoNextLevel { get; set; }

        //growth data
        public byte StrengthCurveIndex { get; set; }
        public byte VitalityCurveIndex { get; set; }
        public byte MagicCurveIndex { get; set; }
        public byte SpiritCurveIndex { get; set; }
        public byte DexterityCurveIndex { get; set; }
        public byte LuckCurveIndex { get; set; }
        public byte HPCurveIndex { get; set; }
        public byte MPCurveIndex { get; set; }
        public byte EXPCurveIndex { get; set; }
        public sbyte RecruitLevelOffset { get; set; }
        public byte Limit1_1Index { get; set; }
        public byte Limit1_2Index { get; set; }
        public byte Limit2_1Index { get; set; }
        public byte Limit2_2Index { get; set; }
        public byte Limit3_1Index { get; set; }
        public byte Limit3_2Index { get; set; }
        public byte Limit4Index { get; set; }
        public ushort KillsForLimitLv2 { get; set; }
        public ushort KillsForLimitLv3 { get; set; }
        public ushort UsesForLimit1_2 { get; set; }
        public ushort UsesForLimit2_2 { get; set; }
        public ushort UsesForLimit3_2 { get; set; }
        public uint LimitLv1HPDivisor { get; set; }
        public uint LimitLv2HPDivisor { get; set; }
        public uint LimitLv3HPDivisor { get; set; }
        public uint LimitLv4HPDivisor { get; set; }
    }
}
