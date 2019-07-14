using System;
using System.Dynamic;
using Shojy.FF7.Elena.Battle;
using Shojy.FF7.Elena.Items;

namespace Shojy.FF7.Elena.Equipment
{
    public class Weapon
    {
        public TargetData Targets { get; set; }
        public byte DamageCalculationId { get; set; }
        public byte AttackStrength { get; set; }
        public EquipmentStatus Status { get; set; }
        public GrowthRate GrowthRate { get; set; }
        public byte CriticalRate { get; set; }
        public byte AccuracyRate { get; set; }
        public byte WeaponModelId { get; set; }
        public EquipableBy EquipableBy { get; set; }
        public Elements AttackElements { get; set; }
        public MateriaSlot[] MateriaSlots { get; set; }
        public Restrictions Restrictions { get; set; }

        public Weapon()
        {
            // Default materia slots array to empty slots
            var slots = new MateriaSlot[8];
            for (var i = 0; i < slots.Length; ++i)
            {
                slots[i] = MateriaSlot.None;
            }

            this.MateriaSlots = slots;
        }
    }

}