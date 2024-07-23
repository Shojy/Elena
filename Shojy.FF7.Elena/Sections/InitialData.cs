using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using Shojy.FF7.Elena.Inventory;

namespace Shojy.FF7.Elena.Sections
{
    /// <summary>
    /// The initial data section. This does not include character data,
    /// which can instead be found in CharacterData.
    /// </summary>
    public class InitialData
    {
        #region Private Fields

        private int InventorySize = 320, MateriaInventorySize = 200, StolenMateriaCount = 48;
        private byte[] _sectionData;

        #endregion

        #region Public Constructors

        public InitialData(byte[] sectionData)
        {
            _sectionData = sectionData;
            ParseData(sectionData);
        }

        #endregion

        #region Public Properties

        public byte Party1 { get; set; }
        public byte Party2 { get; set; }
        public byte Party3 { get; set; }
        public InventoryItem[] Inventory { get; } = new InventoryItem[320];
        public InventoryMateria[] Materia { get; } = new InventoryMateria[200];
        public InventoryMateria[] StolenMateria { get; } = new InventoryMateria[48];
        public uint Gil { get; set; }

        #endregion

        #region Private Methods

        private void ParseData(byte[] data)
        {
            Party1 = data[0x4A4];
            Party2 = data[0x4A5];
            Party3 = data[0x4A6];

            var temp = new byte[2];
            for (int i = 0; i < InventorySize; ++i)
            {
                Array.Copy(data, 0x4A8 + (i * 2), temp, 0, 2);
                Inventory[i] = new InventoryItem();
                Inventory[i].ParseData(temp);
            }
            temp = new byte[4];
            for (int i = 0; i < MateriaInventorySize; ++i)
            {
                Array.Copy(data, 0x728 + (i * 4), temp, 0, 2);
                Materia[i] = new InventoryMateria();
                Materia[i].ParseData(temp);
            }
            for (int i = 0; i < StolenMateriaCount; ++i)
            {
                Array.Copy(data, 0xA48 + (i * 4), temp, 0, 2);
                StolenMateria[i] = new InventoryMateria();
                StolenMateria[i].ParseData(temp);
            }

            Gil = BitConverter.ToUInt32(data, 0xB28);
        }

        #endregion
    }
}
