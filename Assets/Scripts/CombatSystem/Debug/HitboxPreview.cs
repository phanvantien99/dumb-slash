// HitBoxPreview.cs
using Combat;
using UnityEngine;

/// <summary>
/// Visualize hitbox của AttackStep trong Scene view.
/// Chỉ dùng trong Editor, không ảnh hưởng runtime.
/// </summary>
#if UNITY_EDITOR
public class HitBoxPreview : MonoBehaviour
{
    public AttackStep previewStep;
    public bool showPreview = true;
    public Color previewColor = new Color(1f, 0f, 0f, 0.3f);

    void OnDrawGizmosSelected()
    {
        if (!showPreview || previewStep == null) return;

        Vector3 worldCenter = transform.TransformPoint(previewStep.hitboxOffset); // ✅

        Gizmos.color = previewColor;
        Gizmos.matrix = Matrix4x4.TRS(worldCenter, transform.rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, previewStep.hitboxSize);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, previewStep.hitboxSize);
        Gizmos.matrix = Matrix4x4.identity;

        UnityEditor.Handles.Label(
            worldCenter + Vector3.up * 0.5f,
            $"{previewStep.stepName}\n" +
            $"Offset: {previewStep.hitboxOffset}\n" +
            $"Size: {previewStep.hitboxSize}"
        );
    }
#endif
}