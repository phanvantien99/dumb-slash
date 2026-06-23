using System;
using Combat;
using UnityEditor.Animations;
using UnityEngine;


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ComboController))]
public class PlayerAnimationController : MonoBehaviour
{

    [Header("Movement Smoothing")]
    [SerializeField] float _speedDampTime = 0.1f;
    Animator _animator;
    ComboController _combo;
    DamageReceiver _damageReceiver;

    float _currentSpeed;




    void Awake()
    {
        _animator = GetComponent<Animator>();
        _combo = GetComponent<ComboController>();
        _damageReceiver = GetComponent<DamageReceiver>();
    }

    void OnEnable()
    {
        _combo.OnAttackStarted += _handleAttackStarted;
        _combo.OnComboFinished += _handleComboFinished;

        if (_damageReceiver != null)
        {
            _damageReceiver.OnHealthChanged += _handleHealthChanged;
            _damageReceiver.OnDeath += _handleDeath;
        }

    }
    // ── Public API (gọi từ PlayerMovement hoặc PlayerController) ─────────────

    /// <summary>
    /// Cập nhật blend tree Speed từ movement input magnitude.
    /// Gọi từ PlayerMovement.Move() thay cho PlayerAnimator cũ.
    /// </summary>
    public void SetMovementSpeed(float speed)
    {
        _currentSpeed = Mathf.Lerp(_currentSpeed, speed, Time.deltaTime / _speedDampTime);
        _animator.SetFloat("Speed", _currentSpeed);
    }

    void _handleAttackStarted(AttackStep step, int comboCount)
    {
        // Convention: animationTrigger trên AttackStep phải trùng tên
        // parameter trong Animator Controller (ví dụ: "Attack_1", "Attack_Heavy")
        if (!string.IsNullOrEmpty(step.animationTrigger))
        {
            _animator.SetBool("IsAttacking", true);
            _animator.SetFloat("AttackSpeed", step.animationSpeed);
            // _animator.CrossFadeInFixedTime(step.animationTrigger, .05f, 0, 0f);
            _animator.SetTrigger(step.animationTrigger);
        }
    }

    void _handleComboFinished(int totalHits)
    {
        _animator.SetBool("IsAttacking", false);
        // Reset về locomotion — không cần làm gì thêm nếu
        // transition "Has Exit Time" được set đúng trong Animator Controller
        // Nếu cần force reset:
        // _animator.SetTrigger("ReturnToIdle");
    }

    void _handleHealthChanged(float current, float max)
    {

    }


    void _handleDeath()
    {

    }


    public float GetDurationForRecoveryPhase(AttackStep step)
    {
        AnimatorClipInfo[] clipInfos = _animator.GetCurrentAnimatorClipInfo(0);
        if (clipInfos.Length > 0)
        {
            float clipLength = clipInfos[0].clip.length / step.animationSpeed;
            float remainingDuration = Mathf.Max(clipLength - (step.activeDuration + step.startupDuration)); // only get time of phase recovery
            return Mathf.Max(step.recoveryDuration, remainingDuration);
        }
        return step.recoveryDuration;
    }
}
