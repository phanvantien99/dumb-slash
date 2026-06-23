using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    float speed = 0f;
    PlayerAnimationController _playerAnimator;

    GroundChecker groundChecker;

    private RaycastHit _slopeHit;

    private PlayerRotate _playerRotation;

    private RaycastHit _wallHit;
    [SerializeField]
    private LayerMask hitLayerMask;
    Rigidbody _rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _playerAnimator = GetComponent<PlayerAnimationController>();
        groundChecker = GetComponentInChildren<GroundChecker>();
        _playerRotation = GetComponent<PlayerRotate>();
    }

    public void Move(Vector2 input)
    {
        if (input == Vector2.zero && groundChecker.IsGrounded)
        {
            _rb.linearVelocity = new Vector3(0f, 0f, 0f);
            _playerAnimator.SetMovementSpeed(0f);
            return; // exit if no input
        }
        Vector3 newMovement = new Vector3(input.x, 0f, input.y);
        _playerRotation.setDirection(newMovement); // update direction for rotation


        // calculate velocity
        Vector3 velocity = _getPlayerVelocity(newMovement);


        if (_onSlope()) // for the slope()
        {
            Vector3 direction = _getSlopeMoveDirection(velocity);
            _rb.linearVelocity = direction * speed;

            if (input == Vector2.zero)
            {
                _rb.linearVelocity = Vector3.zero;
            }
        }
        else
        {
            // transform.Translate(newMovement * speed * Time.deltaTime, Space.World);
            _rb.linearVelocity = new Vector3(velocity.x * speed,
                                            _rb.linearVelocity.y,
                                            velocity.z * speed);

        }
        _playerAnimator.SetMovementSpeed(newMovement.magnitude);
    }

    bool _onSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, 1.5f))
        {
            float angle = Vector3.Angle(_slopeHit.normal, Vector3.up);
            return angle > 0 && angle < 45f;
        }
        return false;
    }

    Vector3 _getSlopeMoveDirection(Vector3 movement)
    {
        return Vector3.ProjectOnPlane(movement, _slopeHit.normal).normalized;
    }

    Vector3 _getPlayerVelocity(Vector3 movementInput)
    {
        if (!IsHittingWall(movementInput)) return movementInput;

        Vector3 slideDirection = Vector3.ProjectOnPlane(movementInput, _wallHit.normal);
        slideDirection.y = 0f;
        return slideDirection.normalized;
    }

    bool IsHittingWall(Vector3 direction)
    {

        Vector3[] rayOrigins = new Vector3[]
        {
            transform.position + Vector3.up * 0.1f,  // shoot ray cast at foot
            transform.position + Vector3.up * 0.5f,  // Giữa người  
            transform.position + Vector3.up * 1.0f,  // shoot from head đầu
        };

        foreach (Vector3 origin in rayOrigins)
        {
            if (Physics.Raycast(origin, direction.normalized, out _wallHit, .5f, hitLayerMask))
            {
                return true;
            }
        }
        return false;
    }
}
