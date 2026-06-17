using System;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// Spawns a physics overlap check each FixedUpdate while Active.
    /// Prevents hitting the same target twice per step.
    /// </summary>
    public class HitBoxController : MonoBehaviour
    {
        public event Action<AttackStep, GameObject> OnHit;
        readonly HashSet<Collider> _hitThisStep = new();
        readonly Transform _origin;
        AttackStep _currentStep;
        bool _isActive;
        public HitBoxController(Transform origin)
        {
            _origin = origin;
        }

        public void Activate(AttackStep step)
        {
            _currentStep = step;
            _isActive = true;
            _hitThisStep.Clear();
        }


        public void Deactive()
        {
            _isActive = false;
            _currentStep = null;
        }

        public void Tick()
        {
            if (!_isActive || _currentStep == null) return;

            Vector3 center = _origin.TransformPoint(_currentStep.hitboxOffset);
            Collider[] hits = Physics.OverlapBox(center,
            _currentStep.hitboxSize * .5f, _origin.rotation, _currentStep.layerMask);

            foreach (Collider collider in hits)
            {
                if (_hitThisStep.Contains(collider)) continue;
                _hitThisStep.Add(collider);
                OnHit.Invoke(_currentStep, collider.gameObject);
            }
        }


        public void DrawGizmos(Color color)
        {
            if (_currentStep == null) return;
            Gizmos.color = color;
            Vector3 center = _origin.TransformPoint(_currentStep.hitboxOffset);
            Gizmos.matrix = Matrix4x4.TRS(center, _origin.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, _currentStep.hitboxSize);
            Gizmos.matrix = Matrix4x4.identity;
        }

    }

}
