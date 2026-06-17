
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{


    private Vector2 movement;


    private bool isJumpTrigger = false;

    PlayerMovement playerMovement;
    Ray movementRay;


    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
        movementRay.direction = movement;

    }

    public void OnJump(InputAction.CallbackContext context)
    {
        isJumpTrigger = context.ReadValueAsButton();
    }


    void FixedUpdate()
    {
        playerMovement.Move(movement);
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, new Vector3(movement.x, 0f, movement.y));
    }

}
