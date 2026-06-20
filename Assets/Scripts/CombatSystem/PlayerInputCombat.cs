using UnityEngine;
using UnityEngine.InputSystem;

namespace Combat
{
    /// <summary>
    /// Bridges Unity's New Input System to ComboController.
    /// Attach alongside ComboController on the player GameObject.
    ///
    /// Input Action Asset setup:
    ///   ActionMap: "Combat"
    ///     Action: "LightAttack"   → Button
    ///     Action: "HeavyAttack"   → Button
    ///     Action: "SpecialAttack" → Button
    ///     Action: "Dodge"         → Button
    /// </summary>
    [RequireComponent(typeof(ComboController))]
    public class PlayerInputCombat : MonoBehaviour
    {
        // Inspector
        [Header("Input Action")]
        [SerializeField] InputActionAsset _actionAsset;
        [SerializeField] string _actionMapName = "Combat";

        // Reference
        ComboController _combo;
        InputActionMap _map;

        InputAction _lightAttack;
        InputAction _heavyAttack;
        InputAction _specialAttack;
        InputAction _dodge;

        void Awake()
        {
            _combo = GetComponent<ComboController>();
            _map = _actionAsset.FindActionMap(_actionMapName, throwIfNotFound: true);
            _lightAttack = _map.FindAction("LightAttack", true);
            _heavyAttack = _map.FindAction("HeavyAttack", true);
            _specialAttack = _map.FindAction("SpecialAttack", true);
            _dodge = _map.FindAction("Dodge", true);
        }

        void OnEnable()
        {
            _map.Enable();

            _lightAttack.performed += OnLightAttack;
            _heavyAttack.performed += OnHeavyAttack;
            _specialAttack.performed += OnSpecialAttack;
            _dodge.performed += OnDodge;
        }

        void OnDisable()
        {
            _lightAttack.performed -= OnLightAttack;
            _heavyAttack.performed -= _ => _combo.ReceiveInput(AttackInput.Heavy);
            _specialAttack.performed -= OnSpecialAttack;
            _dodge.performed -= _ => _combo.ReceiveInput(AttackInput.Dodge);

            _map.Disable();
        }

        void OnLightAttack(InputAction.CallbackContext ctx) => _combo.ReceiveInput(AttackInput.Light);
        void OnHeavyAttack(InputAction.CallbackContext ctx) => _combo.ReceiveInput(AttackInput.Heavy);
        void OnSpecialAttack(InputAction.CallbackContext ctx) => _combo.ReceiveInput(AttackInput.Special);
        void OnDodge(InputAction.CallbackContext ctx) => _combo.ReceiveInput(AttackInput.Dodge);
    }

}

