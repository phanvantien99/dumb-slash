using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField]
    float checkRadius = .2f;

    [SerializeField]
    LayerMask groundLayer;

    public bool IsGrounded { get; set; }

    void Update()
    {
        IsGrounded = Physics.CheckSphere(transform.position, checkRadius, groundLayer);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = IsGrounded ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
