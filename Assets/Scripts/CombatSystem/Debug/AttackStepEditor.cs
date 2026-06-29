#if UNITY_EDITOR
using Combat;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AttackStep))]
public class AttackStepEditor : Editor
{
    AttackStep _step;
    HitBoxPreview _preview;

    void OnEnable()
    {
        _step = (AttackStep)target;
        _preview = FindAnyObjectByType<HitBoxPreview>();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("── Preview Tools ──", EditorStyles.boldLabel);

        if (_preview == null)
        {
            EditorGUILayout.HelpBox(
                "Không tìm thấy HitBoxPreview trong Scene.\n" +
                "Thêm component HitBoxPreview vào MainPlayer.",
                MessageType.Warning
            );
            return;
        }

        if (_preview.previewStep != _step)
        {
            _preview.previewStep = _step;
            EditorUtility.SetDirty(_preview);
        }

        if (GUILayout.Button("🎯 Focus on Player"))
        {
            Selection.activeGameObject = _preview.gameObject;
            SceneView.FrameLastActiveSceneView();
        }
    }

    void OnSceneGUI()
    {
        if (_step == null || _preview == null) return;

        Transform playerTransform = _preview.transform;

        // ── Draw Hitbox ───────────────────────────────────────────────────────
        Vector3 worldCenter = playerTransform.TransformPoint(_step.hitboxOffset); // ✅

        _drawWireCube(worldCenter, _step.hitboxSize, playerTransform.rotation);

        Handles.Label(
            worldCenter + Vector3.up * 0.6f,
            $"  {_step.stepName}\n" +
            $"  Offset: {_step.hitboxOffset}\n" +    // ✅
            $"  Size: {_step.hitboxSize}",
            EditorStyles.whiteLabel
        );

        // ── Position Handle ───────────────────────────────────────────────────
        EditorGUI.BeginChangeCheck();

        Vector3 newWorldCenter = Handles.PositionHandle(
            worldCenter,
            playerTransform.rotation
        );

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(_step, "Move Hitbox Offset");
            _step.hitboxOffset = playerTransform.InverseTransformPoint(newWorldCenter); // ✅
            EditorUtility.SetDirty(_step);
        }

        // ── Scale Handles ─────────────────────────────────────────────────────
        _drawScaleHandles(worldCenter, playerTransform);
    }

    void _drawScaleHandles(Vector3 worldCenter, Transform playerTransform)
    {
        Handles.color = Color.cyan;
        float handleSize = 0.08f;

        (Vector3 dir, int axis)[] handles = new[]
        {
            (playerTransform.right,    0),
            (-playerTransform.right,   0),
            (playerTransform.up,       1),
            (-playerTransform.up,      1),
            (playerTransform.forward,  2),
            (-playerTransform.forward, 2),
        };

        foreach (var (dir, axis) in handles)
        {
            float halfSize = axis == 0 ? _step.hitboxSize.x * 0.5f :
                             axis == 1 ? _step.hitboxSize.y * 0.5f :
                                         _step.hitboxSize.z * 0.5f;

            Vector3 handlePos = worldCenter + dir * halfSize;

            EditorGUI.BeginChangeCheck();

            Vector3 newPos = Handles.FreeMoveHandle(
                handlePos,
                handleSize,
                Vector3.zero,
                Handles.DotHandleCap
            );

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_step, "Resize Hitbox");

                float delta = Vector3.Dot(newPos - handlePos, dir) * 2f;
                Vector3 newSize = _step.hitboxSize;

                if (axis == 0) newSize.x = Mathf.Max(0.1f, newSize.x + delta);
                if (axis == 1) newSize.y = Mathf.Max(0.1f, newSize.y + delta);
                if (axis == 2) newSize.z = Mathf.Max(0.1f, newSize.z + delta);

                _step.hitboxSize = newSize;
                EditorUtility.SetDirty(_step);
            }
        }
    }

    void _drawWireCube(Vector3 center, Vector3 size, Quaternion rotation)
    {
        // Fill mờ
        Handles.color = new Color(1f, 0f, 0f, 0.15f);
        Matrix4x4 oldMatrix = Handles.matrix;
        Handles.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
        Handles.DrawSolidRectangleWithOutline(
            new Vector3[]
            {
                new Vector3(-size.x, -size.y,  size.z) * 0.5f,
                new Vector3( size.x, -size.y,  size.z) * 0.5f,
                new Vector3( size.x,  size.y,  size.z) * 0.5f,
                new Vector3(-size.x,  size.y,  size.z) * 0.5f,
            },
            new Color(1f, 0f, 0f, 0.1f),
            Color.red
        );

        // Wire
        Handles.color = Color.red;
        Handles.DrawWireCube(Vector3.zero, size);
        Handles.matrix = oldMatrix;
    }
}
#endif