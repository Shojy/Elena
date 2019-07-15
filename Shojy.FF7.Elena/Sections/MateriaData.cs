using Shojy.FF7.Elena.Battle;
using System;
using System.Collections.Generic;

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
            this._sectionData = sectionData;

            var count = sectionData.Length / MateriaSize;
            var materias = new Materia.Materia[count];

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

            this.Materias = materias;
        }

        #endregion Public Constructors

        #region Public Properties

        public Materia.Materia[] Materias { get; }

        #endregion Public Properties

        #region Private Methods

        private static Materia.Materia ParseData(byte[] data)
        {
            var materia = new Materia.Materia();

            materia.Level2AP = BitConverter.ToUInt16(data, 0x0) * 100;
            materia.Level3AP = BitConverter.ToUInt16(data, 0x2) * 100;
            materia.Level4AP = BitConverter.ToUInt16(data, 0x4) * 100;
            materia.Level5AP = BitConverter.ToUInt16(data, 0x6) * 100;

            materia.Element = (MateriaElements)data[0xC];
            materia.MateriaType = Materia.Materia.GetMateriaType(data[0xD]);
            return materia;
        }

        #endregion Private Methods
    }
}