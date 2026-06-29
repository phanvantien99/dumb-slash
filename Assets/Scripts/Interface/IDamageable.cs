using System;
using UnityEngine;

namespace Combat
{
    public interface IDamageable
    {
        void TakeDamage(float amount, AttackStep sourceStep, Vector3 attackerPosition);
        bool IsDead { get; }
    }

}
