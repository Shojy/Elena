using Shojy.FF7.Elena.Equipment;
using System;
using System.Collections.Generic;
using Shojy.FF7.Elena.Battle;
using Shojy.FF7.Elena.Items;

namespace Shojy.FF7.Elena.Sections
{
    public class AccessoryData
    {
        #region Private Fields

        private const int AccessorySize = 16;
        private byte[] _sectionData;

        #endregion Private Fields

        #region Public Constructors

        public AccessoryData(byte[] sectionData, IReadOnlyList<string> names, IReadOnlyList<string> descriptions)
        {
            this._sectionData = sectionData;

            var count = sectionData.Length / AccessorySize;
            var accessories = new Accessory[count];

            for (var acc = 0; acc < count; ++acc)
            {
                var bytes = new byte[AccessorySize];
                Array.Copy(
                    sectionData,
                    acc * AccessorySize,
                    bytes,
                    0,
                    AccessorySize);
                accessories[acc] = ParseData(bytes);

                accessories[acc].Index = acc;
                accessories[acc].Name = names[acc];
                accessories[acc].Description = descriptions[acc];
            }

            this.Accessories = accessories;
        }

        #endregion Public Constructors

        #region Public Properties

        public Accessory[] Accessories { get; set; }

        #endregion Public Properties

        #region Private Methods

        private static Accessory ParseData(byte[] data)
        {
            var acc = new Accessory();

            acc.BoostedStat1 = (CharacterStat) data[0x0];
            acc.BoostedStat2 = (CharacterStat) data[0x1];
            acc.BoostedStat1Bonus = data[0x2];
            acc.BoostedStat2Bonus = data[0x3];
            acc.ElementalDamageModifier = (DamageModifier) data[0x4];
            acc.SpecialEffect = (AccessoryEffect) data[0x5];
            acc.ElementalDefense = (Elements) BitConverter.ToUInt16(data, 0x6);
            acc.StatusDefense = (EquipmentStatus) BitConverter.ToUInt32(data, 0x8);
            acc.EquipableBy = (EquipableBy) BitConverter.ToUInt16(data, 0xC);
            acc.Restrictions = (Restrictions) ~BitConverter.ToUInt16(data, 0xE);

            return acc;
        }

        #endregion Private Methods
    }
}