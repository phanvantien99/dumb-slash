using System;
using Combat;
using UnityEditor.Animations;
using UnityEngine;


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ComboController))]
public class PlayerAnimationController : MonoBehaviour, IAnimationEventReceiver
{

    [Header("Movement Smoothing")]
    [SerializeField] float _speedDampTime = 0.1f;
    Animator _animator;
    ComboController _combo;
    DamageReceiver _damageReceiver;

    float _currentSpeed;

    public event Action OnAttackActionEndEvent;
    public event Action OnAttackHitFrameEvent;

    public event Action OnStartupEndEvent;
    public event Action OnActiveEndEvent;
    public event Action OnRecoveryEndEvent;


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
            // _animator.SetTrigger(step.animationTrigger);
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
