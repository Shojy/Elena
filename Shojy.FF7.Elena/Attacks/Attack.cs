using Shojy.FF7.Elena.Battle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shojy.FF7.Elena.Attacks
{
    public class Attack
    {
        #region Public Constructors

        public Attack() { }

        #endregion

        #region Public Properties

        public byte AccuracyRate { get; set; }
        public byte AditionalEffects { get; set; }
        public byte AdditionalEffectsModifier { get; set; }
        public byte AttackEffectID { get; set; }
        public byte AttackStrength { get; set; }
        public ushort CameraMovementIDSingle { get; set; }
        public ushort CameraMovementIDMulti { get; set; }
        public ConditionSubmenu ConditionSubmenu { get; set; }
        public byte DamageCalculationID { get; set; }
        public string Description { get; set; }
        public Elements Elements { get; set; }
        public byte ImpactEffectID { get; set; }
        public ushort ImpactSound { get; set; }
        public int Index { get; set; }
        public ushort MPCost { get; set; }
        public string Name { get; set; }
        public SpecialEffects SpecialAttackFlags { get; set; }
        public StatusChange StatusChange { get; set; }
        public Statuses Statuses { get; set; }
        public TargetData TargetFlags { get; set; }
        public byte TargetHurtActionIndex { get; set; }

        #endregion
    }
}
