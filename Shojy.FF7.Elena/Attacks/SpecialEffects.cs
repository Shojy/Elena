using System;

namespace Shojy.FF7.Elena.Attacks
{
    [Flags]
    public enum SpecialEffects
    {
        DamageMP = 0x1,
        ForcePhysical = 0x4,
        DrainPartialInflictedDamage = 0x10,
        DrainHPAndMP = 0x20,
        DiffuseAttack = 0x40,
        IgnoreStatusDefence = 0x80,
        MissWhenTargetNotDead = 0x100,
        CanReflect = 0x200,
        BypassDefence = 0x400,
        DontAutoRetargetWhenOriginalTargetKilled = 0x800,
        AlwaysCritical = 0x2000
    }
}