using System;
using System.Collections;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// Core combo state machine.  Attach to every combat character.
    ///
    /// Flow per step:
    ///   Startup → Active (hitbox ON) → Recovery → [next step or Idle]
    ///
    /// Responsibilities:
    ///   • Phase timing
    ///   • Input buffering
    ///   • Cancel detection
    ///   • Event dispatch (IComboEvents)
    ///   • Hitstop co-routine
    /// </summary>

    [RequireComponent(typeof(Animator))]
    public class ComboController : MonoBehaviour, IComboEvents
    {
        // Inpector
        [Header("Profile")]
        [SerializeField] CharacterComboProfile _profile;

        [Header("Debug")]
        [SerializeField] bool _showHitBox = true;

        // ICombo event
        public event Action<AttackStep, int> OnAttackStarted;
        public event Action<AttackStep> OnAttackActive;
        public event Action<AttackStep> OnAttackEnd;
        public event Action<AttackStep, GameObject> OnHitConfirmed;
        public event Action<int> OnComboFinished;
        public event Action<AttackInput> OnInputBuffered;
        public event Action<AttackStep, string> OnCancelTriggered;

        // State
        public ComboPhase CurrentPhase { get; private set; } = ComboPhase.None;
        public AttackStep CurrentStep { get; private set; }
        public ComboBranch CurrentBranch { get; private set; }
        public ComboData ActiveCombo { get; private set; }
        public int ComboCount { get; private set; }
        public bool IsAttacking => CurrentPhase != ComboPhase.None;

        //  Reference
        Animator _animator;
        HitBoxController _hitBox;
        InputBuffer _buffer;
        CancelSystem _cancelSystem;

        void Awake()
        {
            _animator = GetComponent<Animator>();
            _hitBox = new HitBoxController(transform);
            _buffer = new InputBuffer();
            _cancelSystem = new CancelSystem();

            _hitBox.OnHit += HandleHit;
        }

        void Update()
        {
            _buffer.Tick();
        }

        void FixedUpdate()
        {
            if (CurrentPhase == ComboPhase.Active)
            {
                _hitBox.Tick();
            }
        }

        void OnDrawGizmos()
        {
            if (_showHitBox && CurrentPhase == ComboPhase.Active)
            {
                _hitBox.DrawGizmos(Color.red);
            }
        }

        // public API
        public void ReceiveInput(AttackInput input)
        {
            if (!IsAttacking)
            {
                TryStartCombo(input);
                return;
            }

            string cancelTarget = input == AttackInput.Dodge ? "Dodge" : "Special";
            if (_cancelSystem.CanCancel(CurrentStep, CurrentPhase, cancelTarget))
            {
                OnCancelTriggered?.Invoke(CurrentStep, cancelTarget);
                StopAllCoroutines();
                ResetState();
                return;
            }

            if (CurrentPhase == ComboPhase.Recovery)
            {
                _buffer.Buffer(input, CurrentStep.bufferWindow);
                OnInputBuffered?.Invoke(input);
            }
        }

        public void SetProfile(CharacterComboProfile profile)
        {
            _profile = profile;
            StopAllCoroutines();
            ResetState();
        }

        // internal
        void TryStartCombo(AttackInput input)
        {
            if (_profile == null) return;
            foreach (ComboData combo in _profile.availableCombos)
            {
                if (_profile.characterId != "" && combo.characterId != "" && _profile.characterId != combo.characterId) continue;
                if (combo.root.step.requiredInput == input)
                {
                    ActiveCombo = combo;
                    CurrentBranch = combo.root;
                    ComboCount = 0;
                    StartCoroutine(ExecuteStep(combo.root.step));
                    return;
                }
            }
        }

        IEnumerator ExecuteStep(AttackStep step)
        {
            CurrentStep = step;
            ComboCount++;

            // Startup
            CurrentPhase = ComboPhase.Startup;
            if (!string.IsNullOrEmpty(step.animationTrigger))
            {
                _animator.SetTrigger(step.animationTrigger);
            }
            OnAttackStarted?.Invoke(step, ComboCount);
            yield return new WaitForSeconds(step.startupDuration);

            // Active attack
            CurrentPhase = ComboPhase.Active;
            _hitBox.Activate(step);
            OnAttackActive?.Invoke(step);
            yield return new WaitForSeconds(step.activeDuration);
            _hitBox.Deactive();

            // recovery attack
            CurrentPhase = ComboPhase.Recovery;
            OnAttackEnd?.Invoke(step);
            yield return new WaitForSeconds(step.recoveryDuration);

            //check buffer
            AttackInput? buffered = _buffer.Flush();
            if (buffered.HasValue && CurrentBranch != null)
            {
                ComboBranch next = ActiveCombo.GetNextComboBranch(CurrentBranch, buffered.Value);
                if (next != null)
                {
                    CurrentBranch = next;
                    StartCoroutine(ExecuteStep(next.step));
                    yield break;
                }
            }

            int finalCount = ComboCount;
            ResetState();
            OnComboFinished?.Invoke(finalCount);
            yield return null;
        }

        void HandleHit(AttackStep step, GameObject target)
        {
            OnHitConfirmed?.Invoke(step, target);
            if (step.hitStop > 0f)
            {
                StartCoroutine(DoHitstop(step.hitStop));
            }
        }


        IEnumerator DoHitstop(float duration)
        {
            float prev = Time.timeScale;
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(duration);
            Time.timeScale = prev;
        }

        void ResetState()
        {
            CurrentPhase = ComboPhase.None;
            CurrentStep = null;
            CurrentBranch = null;
            ActiveCombo = null;
            _buffer.Clear();
            _hitBox.Deactive();
        }
    }

}
