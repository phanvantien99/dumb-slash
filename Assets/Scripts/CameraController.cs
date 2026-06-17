using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Transform targetToFollow;

    [SerializeField]
    float smoothTime;

    [SerializeField]
    Vector3 offset;

    [SerializeField]
    float angle = 45f;

    private Vector3 velocity = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _followTarget();
    }


    void _followTarget()
    {
        if (targetToFollow != null)
        {
            transform.rotation = Quaternion.Euler(angle, transform.rotation.y, transform.rotation.z);
            Vector3 targetPosition = targetToFollow.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }
}
