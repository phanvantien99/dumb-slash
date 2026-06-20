using System;
using UnityEngine;

namespace Combat
{
    public interface IDamageable
    {
        void TakeDamage(float amount, AttackStep sourceStep);
        bool IsDead { get; }
    }

}
