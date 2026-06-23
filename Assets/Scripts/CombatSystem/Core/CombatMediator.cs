using UnityEngine;


namespace Combat
{
    /// <summary>
    /// Listens to ComboController events and routes them to other systems:
    ///   • Deals damage to DamageReceiver
    ///   • Spawns hit VFX
    ///   • Plays SFX
    ///
    /// This keeps ComboController ignorant of downstream systems.
    /// Swap this class or add more Mediators as the game grows.
    /// </summary>
    [RequireComponent(typeof(ComboController))]
    public class CombatMediator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] AudioSource _audioSource;

        ComboController _combo;

        void Awake()
        {
            _combo = GetComponent<ComboController>();
        }

        void OnEnable()
        {
            _combo.OnHitConfirmed += _handleHitConfirmed;
            _combo.OnAttackStarted += _handAttackStarted;
            _combo.OnComboFinished += _handleComboFinished;
            _combo.OnCancelTriggered += _handleCancel;
        }



        void _handleHitConfirmed(AttackStep step, GameObject target)
        {
            // damge
            if (target.TryGetComponent<DamageReceiver>(out DamageReceiver damageReceiver))
            {
                float damage = step.damage;
                damageReceiver.TakeDamage(damage, step);
            }

            // VFX
            if (step.hitVfx != null)
            {
                Vector3 hitPosition = target.transform.position + Vector3.up;
                Instantiate(step.hitVfx, hitPosition, Quaternion.identity);
            }


            //SFX
            if (step.hitSfx != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(step.hitSfx);
            }

        }

        void _handAttackStarted(AttackStep step, int comboCount)
        {
            if (step.swingSfx != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(step.swingSfx);
            }
        }

        void _handleComboFinished(int totalHits)
        {
            Debug.Log($"[Combat] Combo finished — {totalHits} hits");
            // Fire combo score event, UI popup, etc.
        }

        void _handleCancel(AttackStep step, string cancelTarget)
        {
            Debug.Log($"[Combat] Cancelled '{step.stepName}' into {cancelTarget}");
            // Hand off to DodgeController, SpecialAbilityController, etc.
        }
    }

}
