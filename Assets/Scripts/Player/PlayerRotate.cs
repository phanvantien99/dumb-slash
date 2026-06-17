using UnityEngine;

public class PlayerRotate : MonoBehaviour
{

    private Rigidbody _rb;
    private bool _shouldRotate;
    private Vector3 direction;

    [SerializeField]
    private float _rotationSpeed;

    [SerializeField]
    private LayerMask wallLayerMask;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        _applyRotation();
    }
    public void setDirection(Vector3 direction)
    {
        if (direction.magnitude > .1f) // if actually has input
        {
            _shouldRotate = true;
            this.direction = direction;
        }
        else
        {
            _shouldRotate = false;
        }
    }
    private void _applyRotation()
    {
        if (!_shouldRotate || direction == Vector3.zero) return;

        Quaternion targetQuaternion = Quaternion.LookRotation(direction);
        _rb.MoveRotation(Quaternion.RotateTowards(_rb.rotation,
            targetQuaternion,
            _rotationSpeed * Time.fixedDeltaTime * 100
        ));
    }

    // private bool IsBlockedByWall(Vector3 direction)
    // {
    //     Vector3[] checkDirections = new Vector3[]
    //     {
    //         direction.normalized,
    //         Quaternion.Euler(0,45,0) * direction.normalized,
    //         Quaternion.Euler(0,-45,0) * direction.normalized,

    //     };

    //     foreach (Vector3 dir in checkDirections)
    //     {
    //         if (Physics.Raycast(transform.position, dir, .5f, wallLayerMask))
    //         {
    //             return true;
    //         }
    //     }
    //     return false;
    // }

}
