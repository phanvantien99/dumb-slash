using System;
using UnityEngine;

namespace Combat
{

    /// <summary>
    /// Attach to any enemy/damageable object.
    /// ComboController's HitboxController will call TakeDamage via OnHitConfirmed.
    /// </summary>
    public class DamageReceiver : MonoBehaviour, IDamageable
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
            OnDeath += _onDeadHandler;
        }

        public void TakeDamage(float amount, AttackStep sourceStep, Vector3 attackerPosition)
        {
            if (IsDead)
            {
                OnDeath.Invoke();
            }
            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            Debug.Log("Damge taken: " + CurrentHealth);
            OnHealthChanged?.Invoke(CurrentHealth, _maxHealth);

            // Apply impluse (UX)
            if (TryGetComponent<Rigidbody>(out Rigidbody _rb))
            {
                if (!_rb.isKinematic)
                {
                    Vector3 knockbackDir = (transform.position - attackerPosition).normalized;
                    knockbackDir.y = 0f;
                    float thrustPower = sourceStep.impulse.magnitude * 100; // get float strength
                    _rb.AddForce(knockbackDir * thrustPower, ForceMode.Impulse);
                }
            }
        }

        public void Heal(float amount)
        {
            if (IsDead) return;
            CurrentHealth = Mathf.Max(0, CurrentHealth + amount);
            OnHealthChanged?.Invoke(CurrentHealth, _maxHealth);
        }

        void _onDeadHandler()
        {
            Destroy(gameObject, .2f);
        }
    }

}
