using System;
using System.Collections.Generic;
using UnityEngine;

public class Fading : MonoBehaviour, IEquatable<Fading>
{

    [SerializeField]
    List<Renderer> renderers = new List<Renderer>();
    [SerializeField]
    Vector3 position;
    [SerializeField]
    List<Material> materials = new List<Material>();

    [HideInInspector]
    float initialAlpha;

    public List<Material> Materials { get => materials; set => materials = value; }
    public float InitialAlpha { get => initialAlpha; set => initialAlpha = value; }

    void Awake()
    {
        position = transform.position;
        if (renderers.Count == 0) renderers.AddRange(GetComponentsInChildren<Renderer>());
        foreach (Renderer renderer in renderers)
        {
            Materials.AddRange(renderer.materials);
        }
        InitialAlpha = Materials[0].color.a;
    }

    public bool Equals(Fading other)
    {
        return position.Equals(other.position);
    }

    public override int GetHashCode()
    {
        return position.GetHashCode();
    }
}
