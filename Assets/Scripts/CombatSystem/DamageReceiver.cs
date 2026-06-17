using System;
using UnityEngine;

namespace Combat
{

    /// <summary>
    /// Attach to any enemy/damageable object.
    /// ComboController's HitboxController will call TakeDamage via OnHitConfirmed.
    /// </summary>
    public class DamageReceiver : MonoBehaviour
    {
        [Header("Stat")]
        [SerializeField] float _maxHealth = 100;

        public event Action<float, float> OnHealthChanged;
        public event Action OnDeath;

        public float CurrentHealth { get; private set; }
        public bool IsDead => CurrentHealth <= 0;
        void Awake()
        {
            CurrentHealth = _maxHealth;
        }

        public void TakeDamage(float amount, AttackStep sourceStep)
        {
            if (IsDead) return;
            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            OnHealthChanged.Invoke(CurrentHealth, _maxHealth);

            // Apply impluse (UX)
            if (TryGetComponent<Rigidbody>(out Rigidbody _rb))
            {
                _rb.AddForce(sourceStep.impulse, ForceMode.Impulse);
            }

            if (CurrentHealth <= 0)
            {
                OnDeath.Invoke();
            }
        }

        public void Heal(float amount)
        {
            if (IsDead) return;
            CurrentHealth = Mathf.Max(0, CurrentHealth + amount);
            OnHealthChanged.Invoke(CurrentHealth, _maxHealth);
        }
    }

}
