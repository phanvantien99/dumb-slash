using Combat;
using UnityEngine;

/// <summary>
/// Shared state của Player.
/// Các system đọc/ghi vào đây, không cần biết về nhau.
/// </summary>


[RequireComponent(typeof(ComboController), typeof(PlayerMovement))]
public class PlayerState : MonoBehaviour
{
    [Header("Combat")]
    public bool IsAttacking { get; set; }
    public bool IsDodging { get; set; }
    public bool IsStunned { get; set; }
    public bool IsDead { get; set; }

    [Header("Movement")]
    public bool IsMoving { get; set; }
    public bool IsGrounded { get; set; }
    public bool IsOnSlope { get; set; }

    // Helper: có bị lock movement không
    public bool IsMovementLocked => IsAttacking ||
                                    IsDodging ||
                                    IsStunned ||
                                    IsDead;

    // Helper: có bị lock action không
    public bool IsActionLocked => IsStunned || IsDead;

}

