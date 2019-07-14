using System;
using Shojy.FF7.Elena.Battle;
using Shojy.FF7.Elena.Items;

namespace Shojy.FF7.Elena.Sections
{
    public class ItemData
    {
        private const int ItemSize = 28;
        public Item[] Items { get; }
        private byte[] _sectionData;
        public ItemData(byte[] sectionData, string[] names)
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

                if (i < names.Length)
                {
                    items[i].Name = names[i];
                }

            }

            this.Items = items;

        }

        private Item ParseItemData(byte[] data)
        {
            var item = new Item();

            item.CameraMovementId = BitConverter.ToUInt16(data, 0x8);
            item.Restrictions = (Restrictions) BitConverter.ToUInt16(data, 0xA);
            item.TargetData = (TargetData) data[0xC];
            item.AttackEffectId = data[0xD];
            item.DamageCalculationId = data[0xE];
            item.AttackPower = data[0xF];
            item.Status = (Statuses) BitConverter.ToUInt32(data, 0x14);
            item.Element = (Elements) BitConverter.ToUInt16(data, 0x18);

            return item;
        }
    }
}