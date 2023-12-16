using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shojy.FF7.Elena.Battle
{
    public class Command
    {
        #region Public Constructors

        public Command() { }

        #endregion

        #region Public Properties

        public ushort CameraMovementIDSingle { get; set; }
        public ushort CameraMovementIDMulti { get; set; }
        public string Description { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
        public byte InitialCursorAction { get; set; }
        public TargetData TargetFlags { get; set; }

        #endregion
    }
}
