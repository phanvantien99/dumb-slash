using UnityEngine;

public class CutoutObject : MonoBehaviour
{

    [SerializeField]
    Transform targetOject;

    [SerializeField]
    LayerMask wallMask;

    Camera mainCamera;

    void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 cutoutPos = mainCamera.WorldToViewportPoint(targetOject.position);
        // cutoutPos.y /= (Screen.width / Screen.height);

        Vector3 direction = targetOject.position - transform.position; // return direction from camera to target
        RaycastHit[] hitObjects = Physics.RaycastAll(transform.position, direction, direction.magnitude, wallMask); // direction.magnitude return distance

        for (int i = 0; i < hitObjects.Length; i++)
        {
            Material[] materials = hitObjects[i].transform.GetComponent<Renderer>().materials;
            for (int matIndex = 0; matIndex < materials.Length; matIndex++)
            {
                materials[matIndex].SetVector("_CutoutPosition", cutoutPos);
                materials[matIndex].SetFloat("_CutoutSize", 10f);
                materials[matIndex].SetFloat("_FalloffSize", 1f);
            }
        }
    }
}
