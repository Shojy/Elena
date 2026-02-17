using Shojy.FF7.Elena.Attacks;
using Shojy.FF7.Elena.Battle;
using Shojy.FF7.Elena.Text;
using System;
using System.Collections.Generic;

namespace Shojy.FF7.Elena.Sections
{
    public class AttackData
    {
        #region Private Fields

        private const int AttackSize = 28;
        private readonly byte[] _sectionData;

        #endregion

        #region Public Constructors

        public AttackData(byte[] sectionData, IReadOnlyList<FFText> names, IReadOnlyList<FFText> descriptions)
        {
            this._sectionData = sectionData;

            var count = sectionData.Length / AttackSize;
            var attacks = new Attack[count];

            for (var atk = 0; atk < count; ++atk)
            {
                var atkBytes = new byte[AttackSize];
                Array.Copy(
                    sectionData,
                    atk * AttackSize,
                    atkBytes,
                    0,
                    AttackSize);
                attacks[atk] = this.ParseAttackData(atkBytes);

                attacks[atk].Index = atk;
                attacks[atk].Name = names[atk];
                attacks[atk].Description = descriptions[atk];
            }
            this.Attacks = attacks;
        }

        #endregion

        #region Public Properties

        public Attack[] Attacks;

        #endregion

        #region Private Methods

        private Attack ParseAttackData(byte[] atkData)
        {
            var atk = new Attack();
            atk.AccuracyRate = atkData[0x0];
            atk.ImpactEffectID = atkData[0x1];
            atk.TargetHurtActionIndex = atkData[0x2];
            atk.MPCost = BitConverter.ToUInt16(atkData, 0x4);
            atk.ImpactSound = BitConverter.ToUInt16(atkData, 0x6);
            atk.CameraMovementIDSingle = BitConverter.ToUInt16(atkData, 0x8);
            atk.CameraMovementIDMulti = BitConverter.ToUInt16(atkData, 0xA);
            atk.TargetFlags = (TargetData)atkData[0xC];
            atk.AttackEffectID = atkData[0xD];
            atk.DamageCalculationID = atkData[0xE];
            atk.AttackStrength = atkData[0xF];
            atk.ConditionSubmenu = (ConditionSubmenu)atkData[0x10];
            atk.StatusChange = new StatusChange(atkData[0x11]);
            atk.AditionalEffects = atkData[0x12];
            atk.AdditionalEffectsModifier = atkData[0x13];
            atk.Statuses = (Statuses)BitConverter.ToUInt32(atkData, 0x14);
            atk.Elements = (Elements)BitConverter.ToUInt16(atkData, 0x18);

            atk.SpecialAttackFlags = (SpecialEffects) ~ BitConverter.ToUInt16(atkData, 0x1A);
            return atk;
        }

        #endregion
    }
}
