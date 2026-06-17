using System;
using UnityEngine;

namespace Combat
{
    public interface IComboEvents
    {
        /// <summary>A new attack step has started (Startup phase begins).</summary>
        event Action<AttackStep, int /* comboCount */> OnAttackStarted;

        /// <summary>Hitbox is now active — deal damage this frame window.</summary>
        event Action<AttackStep> OnAttackActive;

        /// <summary>Attack step ended (Recovery finished or was cancelled).</summary>
        event Action<AttackStep> OnAttackEnd;

        /// <summary>Attack successfully hit a target.</summary>
        event Action<AttackStep, GameObject /* target */> OnHitConfirmed;

        /// <summary>An entire combo string finished (no follow-up in time).</summary>
        event Action<int /* total hits */> OnComboFinished;

        /// <summary>Player input was buffered for a future step.</summary>
        event Action<AttackInput> OnInputBuffered;

        /// <summary>Cancel system: current step was cancelled into another action.</summary>
        event Action<AttackStep, string /* cancelTo */> OnCancelTriggered;
    }

}
