using UnityEngine;


namespace Combat
{
    public interface IAnimationEventReceiver
    {
        void OnAttackActionEnd();
        void OnAttackHitFrame();

        void OnStartupEnd();
        void OnActiveEnd();
        void OnRecoveryEnd();
        
    }

}
