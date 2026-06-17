using UnityEngine;


namespace Combat
{
    /// <summary>
    /// Stores at most ONE pending input during an attack's buffer window.
    /// Call Flush() to consume it; it auto-expires after <see cref="TTL"/> seconds.
    /// </summary>
    public class InputBuffer : MonoBehaviour
    {
        public AttackInput? Pending { get; private set; }
        public bool HasInput => Pending.HasValue;

        float _expireAt;
        public float TTL { get; set; } = .2f; // can be tuned per AttackStep

        // ── API ──────────────────────────────────────────────────────────────────
        public void Buffer(AttackInput input, float customTTL = -1f)
        {
            Pending = input;
            _expireAt = Time.time + (customTTL > 0 ? customTTL : TTL);
        }

        public AttackInput? Flush()
        {
            if (!Pending.HasValue) return null;
            if (Time.time > _expireAt)
            {
                Clear();
                return null;
            }

            AttackInput? result = Pending;
            Clear();
            return result;
        }

        public void Clear()
        {
            this.Pending = null;
        }

        /// <summary>Call every frame to expire stale inputs automatically.</summary
        public void Tick()
        {
            if (Pending.HasValue && Time.time > _expireAt)
            {
                Clear();
            }
        }

    }

}
