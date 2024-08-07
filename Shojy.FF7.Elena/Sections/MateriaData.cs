using Shojy.FF7.Elena.Battle;
using System;
using System.Collections.Generic;
using Shojy.FF7.Elena.Materias;

namespace Shojy.FF7.Elena.Sections
{
    public class MateriaData
    {
        #region Private Fields

        private const int MateriaSize = 20;
        private byte[] _sectionData;

        #endregion Private Fields

        #region Public Constructors

        public MateriaData(byte[] sectionData, IReadOnlyList<string> names, IReadOnlyList<string> descriptions)
        {
            _sectionData = sectionData;

            var count = sectionData.Length / MateriaSize;
            var materias = new Materia[count];

            for (var mat = 0; mat < count; ++mat)
            {
                var bytes = new byte[MateriaSize];
                Array.Copy(
                    sectionData,
                    mat * MateriaSize,
                    bytes,
                    0,
                    MateriaSize);
                materias[mat] = ParseData(bytes);

                materias[mat].Index = mat;
                materias[mat].Name = names[mat];
                materias[mat].Description = descriptions[mat];
            }

            Materias = materias;
        }

        #endregion Public Constructors

        #region Public Properties

        public Materia[] Materias { get; }
        public const int AttributeCount = 6;

        #endregion Public Properties

        #region Private Methods

        private static Materia ParseData(byte[] data)
        {
            var materia = new Materia();

            materia.Level2AP = BitConverter.ToUInt16(data, 0x0) * 100;
            materia.Level3AP = BitConverter.ToUInt16(data, 0x2) * 100;
            materia.Level4AP = BitConverter.ToUInt16(data, 0x4) * 100;
            materia.Level5AP = BitConverter.ToUInt16(data, 0x6) * 100;
            materia.EquipEffect = data[0x8];

            var temp = new byte[4];
            Array.Copy(data, 0x9, temp, 0, 3);
            materia.Status = (Statuses)BitConverter.ToUInt32(temp, 0);

            materia.Element = (MateriaElements)data[0xC];
            materia.MateriaTypeByte = data[0xD];

            for (int i = 0; i < AttributeCount; ++i)
            {
                materia.Attributes[i] = data[0xE + i];
            }
            return materia;
        }

        #endregion Private Methods
    }
}