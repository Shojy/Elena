using Shojy.FF7.Elena.Attacks;
using Shojy.FF7.Elena.Battle;
using Shojy.FF7.Elena.Items;
using Shojy.FF7.Elena.Text;
using System;
using System.Collections.Generic;

namespace Shojy.FF7.Elena.Sections
{
    public class ItemData
    {
        #region Private Fields

        private const int ItemSize = 28;
        private byte[] _sectionData;

        #endregion Private Fields

        #region Public Constructors

        public ItemData(byte[] sectionData, IReadOnlyList<FFText> names, IReadOnlyList<FFText> descriptions)
        {
            this._sectionData = sectionData;

            var count = sectionData.Length / ItemSize;
            var items = new Item[count];

            for (var i = 0; i < count; ++i)
            {
                var itemBytes = new byte[44];
                Array.Copy(
                    sectionData,
                    i * ItemSize,
                    itemBytes,
                    0,
                    ItemSize);
                items[i] = this.ParseItemData(itemBytes);

                items[i].Index = i;
                items[i].Name = names[i];
                items[i].Description = descriptions[i];
            }

            this.Items = items;
        }

        #endregion Public Constructors

        #region Public Properties

        public Item[] Items { get; }

        #endregion Public Properties

        #region Private Methods

        private Item ParseItemData(byte[] data)
        {
            var item = new Item();

            item.CameraMovementId = BitConverter.ToUInt16(data, 0x8);
            item.Restrictions = (Restrictions) ~BitConverter.ToUInt16(data, 0xA);
            item.TargetData = (TargetData)data[0xC];
            item.AttackEffectId = data[0xD];
            item.DamageCalculationId = data[0xE];
            item.AttackPower = data[0xF];
            item.ConditionSubmenu = (ConditionSubmenu)data[0x10];
            item.StatusChange = new StatusChange(data[0x11]);
            item.AdditionalEffects = data[0x12];
            item.AdditionalEffectsModifier = data[0x13];
            item.Status = (Statuses)BitConverter.ToUInt32(data, 0x14);
            item.Element = (Elements)BitConverter.ToUInt16(data, 0x18);
            item.Special = (SpecialEffects) ~ BitConverter.ToUInt16(data, 0x1A);

            return item;
        }

        #endregion Private Methods
    }
}