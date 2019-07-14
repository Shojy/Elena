using System;
using Shojy.FF7.Elena.Battle;
using Shojy.FF7.Elena.Equipment;
using Shojy.FF7.Elena.Items;

namespace Shojy.FF7.Elena.Sections
{
    public class WeaponData
    {
        private readonly byte[] _sectionData;
        public Weapon[] Weapons;

        public WeaponData(byte[] sectionData)
        {
            this._sectionData = sectionData;

            var count = sectionData.Length / 44;
            var weapons = new Weapon[count];

            for (var wpn = 0; wpn < count; ++wpn)
            {
                var wpnBytes = new byte[44];
                Array.Copy(sectionData, wpn * 44, wpnBytes, 0, 44);
                weapons[wpn] = this.ParseWeaponData(wpnBytes);

            }
            this.Weapons = weapons;
        }

        private Weapon ParseWeaponData(byte[] wpnData)
        {
            var wpn = new Weapon();
            wpn.Targets = (TargetData) wpnData[0x0];
            wpn.DamageCalculationId = wpnData[0x2];
            wpn.AttackStrength = wpnData[0x4];
            wpn.Status = (EquipmentStatus) wpnData[0x5];
            wpn.GrowthRate = (GrowthRate) wpnData[0x6];
            wpn.CriticalRate = wpnData[0x7];
            wpn.AccuracyRate = wpnData[0x8];
            wpn.WeaponModelId = wpnData[0x9];
            wpn.EquipableBy = (EquipableBy) BitConverter.ToUInt16(wpnData, 0xE);
            wpn.AttackElements = (Elements) BitConverter.ToUInt16(wpnData, 0x10);

            for (var slot = 0; slot < 8; ++slot)
            {
                wpn.MateriaSlots[slot] = (MateriaSlot) wpnData[0x1C + slot];
            }

            wpn.Restrictions = (Restrictions) wpnData[0x2B];
            return wpn;
        }
    }
}