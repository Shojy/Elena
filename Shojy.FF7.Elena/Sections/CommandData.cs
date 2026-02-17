using Shojy.FF7.Elena.Battle;
using Shojy.FF7.Elena.Text;
using System.Collections.Generic;
using System;

namespace Shojy.FF7.Elena.Sections
{
    public class CommandData
    {
        #region Private Fields

        private const int CommandSize = 8;
        private readonly byte[] _sectionData;

        #endregion

        #region Public Constructors

        public CommandData(byte[] sectionData, IReadOnlyList<FFText> names, IReadOnlyList<FFText> descriptions)
        {
            this._sectionData = sectionData;

            var count = sectionData.Length / CommandSize;
            var commands = new Command[count];

            for (var cmd = 0; cmd < count; ++cmd)
            {
                var cmdBytes = new byte[CommandSize];
                Array.Copy(
                    sectionData,
                    cmd * CommandSize,
                    cmdBytes,
                    0,
                    CommandSize);
                commands[cmd] = this.ParseCommandData(cmdBytes);

                commands[cmd].Index = cmd;
                commands[cmd].Name = names[cmd];
                commands[cmd].Description = descriptions[cmd];
            }
            this.Commands = commands;
        }

        #endregion

        #region Public Properties

        public Command[] Commands;

        #endregion

        #region Private Methods

        private Command ParseCommandData(byte[] cmdData)
        {
            var cmd = new Command();
            cmd.InitialCursorAction = cmdData[0x0];
            cmd.TargetFlags = (TargetData)cmdData[0x1];
            cmd.CameraMovementIDSingle = BitConverter.ToUInt16(cmdData, 0x4);
            cmd.CameraMovementIDMulti = BitConverter.ToUInt16(cmdData, 0x6);
            return cmd;
        }

        #endregion
    }
}