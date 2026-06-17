using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    // ─── Input Types ────────────────────────────────────────────────────────────
    public enum AttackInput { Light, Heavy, Special, Dodge }
    public enum ComboPhase { None, Startup, Active, Recovery }

    [System.Serializable]
    public class AttackStep
    {
        [Header("Identity")]
        public string stepName = "Attack_1";
        public AttackInput requiredInput = AttackInput.Light;

        [Header("Timing (Second)")]
        public float startupDuration = .1f;
        public float activeDuration = .2f;
        public float recoveryDuration = .3f;

        [Header("Input Buffer")]
        [Tooltip("Window in Recovery phase where next input is accepted")]
        public float bufferWindow = .15f;

        [Header("Combat")]
        public float damage = 10f;
        public float hitStop = .05f;
        public Vector3 impulse = new Vector3(0, 0, 2f);
        public bool canCancelIntoSpecial = false;

        [Header("Animation")]
        public string animationTrigger = "";
        public AnimationClip clip;

        [Header("VFX / SFX")]
        public GameObject hitVfx;
        public AudioClip swingSfx;
        public AudioClip hitSfx;

        [Header("Hitbox")]
        public Vector3 hitboxOffset = new Vector3(0, 0, 1f);
        public Vector3 hitboxSize = Vector3.one;
        public LayerMask layerMask;
    }

    // ─── Combo Branch ────────────────────────────────────────────────────────────
    /// <summary>
    /// A node in the combo tree.  Each branch holds one AttackStep
    /// and a list of possible continuations keyed by AttackInput.
    /// </summary>
    [System.Serializable]
    public class ComboBranch
    {
        public AttackStep step;
        public List<ComboBranchLink> branches = new();
    }

    [System.Serializable]
    public class ComboBranchLink
    {
        public AttackInput input;
        public ComboBranch next;
    }

    // ─── Combo ScriptableObject ──────────────────────────────────────────────────
    [CreateAssetMenu(fileName = "New ComboData", menuName = "HackAndSlash/Combo Data")]
    public class ComboData : ScriptableObject
    {
        [Header("Combo identity")]
        public string comboName = "Basic combo";
        public string characterId = "";
        [Header("Combo Tree root (first attack)")]
        public ComboBranch root;

        [Header("Flags")]
        public bool requiredGround = true;
        public bool canBeInterrupted = false;

        // ── Helpers ──────────────────────────────────────────────────────────────

        /// <summary>Walk the tree: given the current branch and an input, return next branch or null.</summary>
        public ComboBranch GetNextComboBranch(ComboBranch current, AttackInput input)
        {
            if (current == null) return null;
            foreach (ComboBranchLink branch in current.branches)
            {
                if (branch.input == input) return branch.next;
            }
            return null;
        }
    }
}