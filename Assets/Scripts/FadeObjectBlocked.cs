using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeObjectBlocked : MonoBehaviour
{
    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    Transform target;
    [SerializeField]
    Camera mainCamera;
    [SerializeField]
    [Range(0, 1f)]
    float fadedAlpha = .33f;
    [SerializeField]
    bool retainShadow = true;
    [SerializeField]
    Vector3 targetPositionOffset = Vector3.up;
    [SerializeField]
    float fadeSpeed = 1;

    [SerializeField]
    float checkPerSecond = 10f;

    [Header("Read data only")]
    [SerializeField]
    List<Fading> objectsBlockingView = new List<Fading>();
    Dictionary<Fading, Coroutine> runningCoroutine = new Dictionary<Fading, Coroutine>();

    RaycastHit[] hitObject = new RaycastHit[10];

    void Start()
    {
        StartCoroutine(_checkForObject());
    }

    IEnumerator _checkForObject()
    {
        WaitForSeconds ratioForCheck = new WaitForSeconds(1f / checkPerSecond);

        while (true)
        {
            int hits = Physics.RaycastNonAlloc(mainCamera.transform.position, (target.position + targetPositionOffset - mainCamera.transform.position).normalized
            , hitObject, (target.position + targetPositionOffset - mainCamera.transform.position).magnitude, layerMask); // get all object hit  from camera to target return data to "hits" and "hitObject"

            if (hits > 0)
            {
                for (int i = 0; i < hits; i++)
                {
                    Fading fadingObject = _getFadingObjectFromHit(hitObject[i]);
                    if (fadingObject != null && !objectsBlockingView.Contains(fadingObject))
                    {
                        if (runningCoroutine.ContainsKey(fadingObject))
                        {
                            if (runningCoroutine[fadingObject] != null)
                            {
                                StopCoroutine(runningCoroutine[fadingObject]);
                            }
                            runningCoroutine.Remove(fadingObject);
                        }
                        runningCoroutine.Add(fadingObject, StartCoroutine(_fadeObjectOut(fadingObject)));
                        objectsBlockingView.Add(fadingObject);
                    }
                }
            }

            _fadeObjectNoLongerBeingHit();
            _clearHits();
            yield return ratioForCheck;
        }
    }

    Fading _getFadingObjectFromHit(RaycastHit hit)
    {
        return hit.collider != null ? hit.collider.GetComponent<Fading>() : null;
    }

    IEnumerator _fadeObjectOut(Fading fadingObject)
    {
        foreach (Material material in fadingObject.Materials)
        {
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.SetInt("_Surface", 1);

            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

            material.SetShaderPassEnabled("DeptOnly", false);
            material.SetShaderPassEnabled("SHADOWCASTER", retainShadow);

            material.SetOverrideTag("RenderType", "Transparent");

            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        }

        float time = 0;
        while (fadingObject.Materials[0].color.a > fadedAlpha)
        {
            foreach (Material material in fadingObject.Materials)
            {
                if (material.HasProperty("_BaseColor"))
                {
                    material.color = new Color(
                        material.color.r,
                        material.color.g,
                        material.color.b,
                        Mathf.Lerp(fadingObject.InitialAlpha, fadedAlpha, time * fadeSpeed)
                    );
                }
            }

            time += Time.deltaTime;
            yield return null;
        }

        if (runningCoroutine.ContainsKey(fadingObject))
        {
            StopCoroutine(runningCoroutine[fadingObject]);
            runningCoroutine.Remove(fadingObject);
        }
    }

    IEnumerator _fadeObjectIn(Fading fadingObject)
    {

        float time = 0;
        while (fadingObject.Materials[0].color.a < fadingObject.InitialAlpha)
        {
            foreach (Material material in fadingObject.Materials)
            {
                if (material.HasProperty("_BaseColor"))
                {
                    material.color = new Color(
                        material.color.r,
                        material.color.g,
                        material.color.b,
                        Mathf.Lerp(fadedAlpha, fadingObject.InitialAlpha, time * fadeSpeed)
                    );
                }
            }

            time += Time.deltaTime;
            yield return null;
        }


        foreach (Material material in fadingObject.Materials)
        {
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.SetInt("_Surface", 0);

            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;

            material.SetShaderPassEnabled("DeptOnly", true);
            material.SetShaderPassEnabled("SHADOWCASTER", true);

            material.SetOverrideTag("RenderType", "Opaque");

            material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        }

        if (runningCoroutine.ContainsKey(fadingObject))
        {
            StopCoroutine(runningCoroutine[fadingObject]);
            runningCoroutine.Remove(fadingObject);
        }
    }

    void _fadeObjectNoLongerBeingHit()
    {
        List<Fading> objectToRemove = new List<Fading>(objectsBlockingView.Count);

        foreach (Fading fading in objectsBlockingView)
        {
            bool objectIsBeingHit = false;
            for (int i = 0; i < hitObject.Length; i++)
            {
                Fading hitFadingObject = _getFadingObjectFromHit(hitObject[i]);
                if (hitObject != null && fading == hitFadingObject)
                {
                    objectIsBeingHit = true;
                    break;
                }
            }

            if (!objectIsBeingHit)
            {
                if (runningCoroutine.ContainsKey(fading))
                {
                    if (runningCoroutine[fading] != null)
                    {
                        StopCoroutine(runningCoroutine[fading]);
                    }
                    runningCoroutine.Remove(fading);
                }

                runningCoroutine.Add(fading, StartCoroutine(_fadeObjectIn(fading)));
                objectToRemove.Add(fading);
            }
        }

        foreach (Fading removeObject in objectToRemove)
        {
            objectsBlockingView.Remove(removeObject);
        }
    }
    void _clearHits()
    {
        System.Array.Clear(hitObject, 0, hitObject.Length);
    }
}
