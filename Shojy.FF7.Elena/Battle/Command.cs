using Shojy.FF7.Elena.Text;

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
        public FFText Description { get; set; }
        public int Index { get; set; }
        public FFText Name { get; set; }
        public byte InitialCursorAction { get; set; }
        public TargetData TargetFlags { get; set; }

        #endregion
    }
}
