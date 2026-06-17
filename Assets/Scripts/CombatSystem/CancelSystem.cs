using System.Collections.Generic;
using UnityEngine;


namespace Combat
{
    /// <summary>
    /// Determines whether the current AttackStep can be cancelled into a target action.
    /// Add cancel rules via RegisterRule() or override CanCancel() in a subclass.
    /// </summary>
    public class CancelSystem : MonoBehaviour
    {

        public class CancelRule
        {
            /// <summary>Step name pattern to match ("*" = any).</summary>
            public string stepNamePattern = "*";
            /// <summary>Phase in which the cancel is allowed.</summary>
            public ComboPhase allowedPhase = ComboPhase.Active;
            /// <summary>What action the cancel leads to ("Special", "Dodge", etc.).</summary>
            public string cancelTarget = "";
            /// <summary>Optional: only allow if step flag canCancelIntoSpecial is true.</summary>
            public bool requiredStepFlag = false;

        }

        readonly List<CancelRule> _rules = new();

        public CancelSystem()
        {
            _rules.Add(new CancelRule { stepNamePattern = "*", allowedPhase = ComboPhase.Active, cancelTarget = "Dodge" });
            _rules.Add(new CancelRule { stepNamePattern = "*", allowedPhase = ComboPhase.Recovery, cancelTarget = "Dodge" });
            _rules.Add(new CancelRule { stepNamePattern = "*", allowedPhase = ComboPhase.Active, cancelTarget = "Special", requiredStepFlag = true });
        }

        // ── API ───────────────────────────────────────────────────────────────────
        public void RegisterRule(CancelRule rule) => _rules.Add(rule);

        /// <summary>Returns true if the cancel is permitted given current context.</summary>
        public bool CanCancel(AttackStep currentStep, ComboPhase comboPhase, string cancelTarget)
        {
            foreach (CancelRule rule in _rules)
            {
                if (rule.cancelTarget != cancelTarget) continue;
                if (rule.allowedPhase != comboPhase) continue;
                bool patterMatch = rule.stepNamePattern == "*" || currentStep.stepName.Contains(rule.stepNamePattern);
                if (!patterMatch) continue;
                if (rule.requiredStepFlag && !currentStep.canCancelIntoSpecial) continue;
                return true;
            }
            return false;
        }
        
    }

}
