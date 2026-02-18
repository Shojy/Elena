using Shojy.FF7.Elena.Battle;
using Shojy.FF7.Elena.Items;
using Shojy.FF7.Elena.Text;

namespace Shojy.FF7.Elena.Equipment
{
    public class Weapon
    {
        #region Public Constructors

        public Weapon()
        {
            this.MateriaSlots = new MateriaSlot[8];
        }

        #endregion Public Constructors

        #region Public Properties

        public byte AccuracyRate { get; set; }
        public Elements AttackElements { get; set; }
        public byte AttackStrength { get; set; }
        public CharacterStat BoostedStat1 { get; set; }
        public byte BoostedStat1Bonus { get; set; }
        public CharacterStat BoostedStat2 { get; set; }
        public byte BoostedStat2Bonus { get; set; }
        public CharacterStat BoostedStat3 { get; set; }
        public byte BoostedStat3Bonus { get; set; }
        public CharacterStat BoostedStat4 { get; set; }
        public byte BoostedStat4Bonus { get; set; }
        public byte CriticalHitSoundID { get; set; }
        public byte CriticalRate { get; set; }
        public byte DamageCalculationId { get; set; }
        public FFText Description { get; set; }
        public EquipableBy EquipableBy { get; set; }
        public GrowthRate GrowthRate { get; set; }
        public byte HighSoundIDMask { get; set; }
        public byte ImpactEffectID { get; set; }
        public int Index { get; set; }
        public MateriaSlot[] MateriaSlots { get; set; }
        public byte MissedAttackSoundID { get; set; }
        public FFText Name { get; set; }
        public byte NormalHitSoundID { get; set; }
        public Restrictions Restrictions { get; set; }
        public EquipmentStatus Status { get; set; }
        public TargetData Targets { get; set; }
        public byte WeaponModelId { get; set; }

        #endregion Public Properties
    }
}