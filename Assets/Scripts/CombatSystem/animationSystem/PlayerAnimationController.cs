using System;
using Combat;
using UnityEditor.Animations;
using UnityEngine;


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ComboController))]
public class PlayerAnimationController : MonoBehaviour, IAnimationEventReceiver
{
    static readonly int _hashSpeed = Animator.StringToHash("Speed");
    static readonly int _hashIsAttacking = Animator.StringToHash("IsAttacking");
    static readonly int _hashAttackSpeed = Animator.StringToHash("AttackSpeed");
    static readonly int _hashIdle = Animator.StringToHash("Idle");

    [Header("Movement Smoothing")]
    [SerializeField] float _speedDampTime = 0.1f;
    Animator _animator;
    ComboController _combo;
    DamageReceiver _damageReceiver;

    public event Action OnAttackActionEndEvent;
    public event Action OnAttackHitFrameEvent;

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
    void OnDisable() // ✅ thêm OnDisable
    {
        _combo.OnAttackStarted -= _handleAttackStarted;
        _combo.OnComboFinished -= _handleComboFinished;

        if (_damageReceiver != null)
        {
            _damageReceiver.OnHealthChanged -= _handleHealthChanged;
            _damageReceiver.OnDeath -= _handleDeath;
        }
    }
    // ── Public API (gọi từ PlayerMovement hoặc PlayerController) ─────────────

    /// <summary>
    /// Cập nhật blend tree Speed từ movement input magnitude.
    /// Gọi từ PlayerMovement.Move() thay cho PlayerAnimator cũ.
    /// </summary>
    public void SetMovementSpeed(float speed)
    {
        // _currentSpeed = Mathf.Lerp(_currentSpeed, speed, Time.deltaTime / _speedDampTime);
        _animator.SetFloat(_hashSpeed, speed);
    }

    void _handleAttackStarted(AttackStep step, int comboCount)
    {
        if (!string.IsNullOrEmpty(step.animationTrigger))
        {
            _animator.SetBool(_hashIsAttacking, true);
            _animator.SetFloat(_hashAttackSpeed, step.animationSpeed);
            _animator.CrossFadeInFixedTime(
                step.animationTrigger,
                .05f,  // 0.05s blend
                0,     // layer 0
                0f     // bắt đầu từ frame 0 của Attack_2
            );
        }
    }

    void _handleComboFinished(int totalHits)
    {
        _animator.SetBool(_hashIsAttacking, false);
        _animator.CrossFadeInFixedTime(_hashIdle, .1f, 0, 0f);
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


    public void OnAttackActionEnd()
    {
        OnAttackActionEndEvent?.Invoke();
    }

    public void OnAttackHitFrame()
    {
        OnAttackHitFrameEvent?.Invoke();
    }

    public void OnStartupEnd()
    {
        _combo.NotifyStartupEnd();
    }

    public void OnActiveEnd()
    {
        _combo.NotifyActiveEnd();
    }

    public void OnRecoveryEnd()
    {
        _combo.NotifyRecoveryEnd();
    }

}
