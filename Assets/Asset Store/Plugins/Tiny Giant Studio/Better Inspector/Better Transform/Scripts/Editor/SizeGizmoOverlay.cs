using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TinyGiantStudio.BetterInspector
{
    [InitializeOnLoad]
    public static class SizeGizmoOverlay
    {
        static BetterTransformSettings editorSettings;

        static SizeGizmoOverlay()
        {
            EditorApplication.delayCall += () =>
            {
                SceneView.duringSceneGui -= OnSceneGUI;
                SceneView.duringSceneGui += OnSceneGUI;
            };
        }


        static void OnSceneGUI(SceneView sceneView)
        {
            if (editorSettings == null) editorSettings = BetterTransformSettings.instance;

            DrawSize();
        }

        static void DrawSize()
        {
            if (Selection.transforms.Length != 1) return;


            if (editorSettings.ShowSizeLabelGizmo && handleLabelStyle == null)
            {
                handleLabelStyle = new(EditorStyles.largeLabel)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = editorSettings.SizeGizmoLabelSize
                };
            }

            float unitSizeMultiplier = ScalesManager.instance.CurrentUnitValue();
            int selectedUnit = ScalesManager.instance.SelectedUnit;
            string[] availableUnits = ScalesManager.instance.GetAvailableUnits();
            string unit;
            if (availableUnits.Length > selectedUnit)
                unit = availableUnits[selectedUnit];
            else
                return;

            if (editorSettings.ShowSizeLabelGizmo)
                UpdateLabelStyles();

            Type transformInspectorType = typeof(BetterTransformEditor);
            ActiveEditorTracker editorTracker = ActiveEditorTracker.sharedTracker;
            Editor[] editors = editorTracker.activeEditors;

            foreach (Editor editor in editors)
                if (editor.GetType() == transformInspectorType)
                {
                    BetterTransformEditor transformInspector = editor as BetterTransformEditor;
                    if (transformInspector == null) return;
                    Transform transform = transformInspector.transform;
                    if (transform == null) return;

                    //Get proper bounds
                    Bounds gizmoBounds = transformInspector.currentBound;
                    gizmoBounds.center = Divide(gizmoBounds.center, transform.lossyScale);
                    gizmoBounds.size = Divide(gizmoBounds.size, transform.lossyScale);

                    //Get transform matrix : position rotation and scale
                    Handles.matrix = Matrix4x4.TRS(transform.position,
                        editorSettings.CurrentWorkSpace == BetterTransformSettings.WorkSpace.World
                            ? Quaternion.identity
                            : transform.rotation, transform.lossyScale); //New

                    if (editorSettings.ShowSizeLabelGizmo)
                    {
                        if (gizmoBounds.extents.x != 0)
                            DrawXLabel(transformInspector.currentBound, gizmoBounds, unitSizeMultiplier, unit,
                                editorSettings.GizmoMaximumDecimalPoints, transform);

                        if (gizmoBounds.extents.y != 0)
                            DrawYLabel(transformInspector.currentBound, gizmoBounds, unitSizeMultiplier, unit,
                                editorSettings.GizmoMaximumDecimalPoints, transform);

                        if (gizmoBounds.extents.z != 0)
                            DrawZLabel(transformInspector.currentBound, gizmoBounds, unitSizeMultiplier, unit,
                                editorSettings.GizmoMaximumDecimalPoints, transform);
                    }

                    if (editorSettings.ShowSizeOutlineGizmo)
                        DrawAxisColoredWireCube(gizmoBounds);
                }
        }

        static void UpdateLabelStyles()
        {
            if (labelBackgroundTextureX == null)
            {
                labelBackgroundTextureColorX = editorSettings.SizeGizmoLabelBackgroundColorX;
                labelBackgroundTextureX = MakeFlatTexture(labelBackgroundTextureColorX);
            }

            if (labelBackgroundTextureColorX != editorSettings.SizeGizmoLabelBackgroundColorX)
            {
                if (labelBackgroundTextureX)
                    Object.DestroyImmediate(labelBackgroundTextureX);
                labelBackgroundTextureColorX = editorSettings.SizeGizmoLabelBackgroundColorX;
                labelBackgroundTextureX = MakeFlatTexture(labelBackgroundTextureColorX);
            }

            if (labelBackgroundTextureY == null)
            {
                labelBackgroundTextureColorY = editorSettings.SizeGizmoLabelBackgroundColorY;
                labelBackgroundTextureY = MakeFlatTexture(labelBackgroundTextureColorY);
            }

            if (labelBackgroundTextureColorY != editorSettings.SizeGizmoLabelBackgroundColorY)
            {
                if (labelBackgroundTextureY)
                    Object.DestroyImmediate(labelBackgroundTextureY);
                labelBackgroundTextureColorY = editorSettings.SizeGizmoLabelBackgroundColorY;
                labelBackgroundTextureY = MakeFlatTexture(labelBackgroundTextureColorY);
            }

            if (labelBackgroundTextureZ == null)
            {
                labelBackgroundTextureColorZ = editorSettings.SizeGizmoLabelBackgroundColorZ;
                labelBackgroundTextureZ = MakeFlatTexture(labelBackgroundTextureColorZ);
            }

            if (labelBackgroundTextureColorZ == editorSettings.SizeGizmoLabelBackgroundColorZ) return;
            if (labelBackgroundTextureZ)
                Object.DestroyImmediate(labelBackgroundTextureZ);
            labelBackgroundTextureColorZ = editorSettings.SizeGizmoLabelBackgroundColorZ;
            labelBackgroundTextureZ = MakeFlatTexture(labelBackgroundTextureColorZ);
        }

        #region Label

        static GUIStyle handleLabelStyle;
        static Texture2D labelBackgroundTextureX;
        static Color labelBackgroundTextureColorX;
        static Texture2D labelBackgroundTextureY;
        static Color labelBackgroundTextureColorY;
        static Texture2D labelBackgroundTextureZ;
        static Color labelBackgroundTextureColorZ;

        static void DrawXLabel(Bounds currentBound, Bounds gizmoBounds, float unitSizeMultiplier, string unitName,
            int gizmoMaximumDecimalPoints, Transform transform)
        {
            handleLabelStyle.normal.textColor = editorSettings.SizeGizmoLabelColorX;
            handleLabelStyle.normal.background = labelBackgroundTextureX;

            float size = currentBound.size.x * unitSizeMultiplier;

            string xLabel = "";
            if (editorSettings.ShowAxisOnLabel)
                xLabel += "X: ";

            if (size is > 0 and < 0.000001f)
                xLabel += " Almost 0";
            else
                xLabel += (float)Math.Round(size, gizmoMaximumDecimalPoints);

            if (editorSettings.ShowUnitOnLabel)
                xLabel += " " + unitName;

            GUIContent label = new(xLabel);
            Vector3 position1;
            if (editorSettings.PositionLabelAtCenter)
                position1 = gizmoBounds.center + new Vector3(0, 0, gizmoBounds.extents.z);
            else
                position1 = gizmoBounds.center + new Vector3(0, -gizmoBounds.extents.y - editorSettings.LabelOffset,
                    gizmoBounds.extents.z);

            Vector3 position2;
            if (editorSettings.PositionLabelAtCenter)
                position2 = gizmoBounds.center + new Vector3(0, 0, -gizmoBounds.extents.z);
            else
                position2 = gizmoBounds.center + new Vector3(0, gizmoBounds.extents.y + editorSettings.LabelOffset,
                    -gizmoBounds.extents.z);

            if (editorSettings.ShowSizeGizmoLabelOnBothSide &&
                currentBound.size.x >= editorSettings.MinimumSizeForDoubleSidedLabel)
            {
                Handles.Label(position1, label, handleLabelStyle);
                Handles.Label(position2, label, handleLabelStyle);
            }
            else
            {
                if (!editorSettings.PositionLabelAtCenter)
                {
                    Vector3 position3 = gizmoBounds.center + new Vector3(0,
                        gizmoBounds.extents.y + editorSettings.LabelOffset,
                        gizmoBounds.extents.z);
                    Vector3 position4 = gizmoBounds.center + new Vector3(0,
                        -gizmoBounds.extents.y - editorSettings.LabelOffset, -gizmoBounds.extents.z);

                    Handles.Label(
                        editorSettings.PositionLabelAtCornerAxis
                            ? GetClosestToSceneCamera(new[] { position1, position4 }, transform)
                            : GetClosestToSceneCamera(new[] { position1, position2, position3, position4 }, transform),
                        label,
                        handleLabelStyle);
                }
                else
                {
                    Handles.Label(GetClosestToSceneCamera(new[] { position1, position2 }, transform), label,
                        handleLabelStyle);
                }
            }
        }

        static void DrawYLabel(Bounds currentBound, Bounds gizmoBounds, float unitSizeMultiplier, string unitName,
            int gizmoMaximumDecimalPoints, Transform transform)
        {
            handleLabelStyle.normal.textColor = editorSettings.SizeGizmoLabelColorY;
            handleLabelStyle.normal.background = labelBackgroundTextureY;

            float size = currentBound.size.y * unitSizeMultiplier;

            string labelString = "";
            if (editorSettings.ShowAxisOnLabel)
                labelString += "Y: ";

            if (size is > 0 and < 0.000001f)
                labelString += " Almost 0";
            else
                labelString += (float)Math.Round(size, gizmoMaximumDecimalPoints);

            if (editorSettings.ShowUnitOnLabel)
                labelString += " " + unitName;

            GUIContent label = new(labelString);

            Vector3 position1;
            if (editorSettings.PositionLabelAtCenter)
                position1 = gizmoBounds.center + new Vector3(0, gizmoBounds.extents.y, 0);
            else
                position1 = gizmoBounds.center + new Vector3(gizmoBounds.extents.x + editorSettings.LabelOffset, 0,
                    gizmoBounds.extents.z);

            Vector3 position2;
            if (editorSettings.PositionLabelAtCenter)
                position2 = gizmoBounds.center + new Vector3(0, -gizmoBounds.extents.y, 0);
            else
                position2 = gizmoBounds.center + new Vector3(-gizmoBounds.extents.x - editorSettings.LabelOffset, 0,
                    -gizmoBounds.extents.z);

            if (editorSettings.ShowSizeGizmoLabelOnBothSide &&
                currentBound.size.y >= editorSettings.MinimumSizeForDoubleSidedLabel)
            {
                Handles.Label(position1, label, handleLabelStyle);
                Handles.Label(position2, label, handleLabelStyle);
            }
            else
            {
                if (!editorSettings.PositionLabelAtCenter)
                {
                    Vector3 position3 = gizmoBounds.center + new Vector3(
                        -gizmoBounds.extents.x - editorSettings.LabelOffset,
                        0, gizmoBounds.extents.z);
                    Vector3 position4 = gizmoBounds.center + new Vector3(
                        gizmoBounds.extents.x + editorSettings.LabelOffset, 0,
                        -gizmoBounds.extents.z);

                    Handles.Label(
                        editorSettings.PositionLabelAtCornerAxis
                            ? GetLeftmostFromSceneCamera(new[] { position1, position2, position3, position4 },
                                transform)
                            : GetClosestToSceneCamera(new[] { position1, position2, position3, position4 }, transform),
                        label, handleLabelStyle);
                }
                else
                {
                    Handles.Label(GetClosestToSceneCamera(new[] { position1, position2 }, transform), label,
                        handleLabelStyle);
                }
            }
        }

        static void DrawZLabel(Bounds currentBound, Bounds gizmoBounds, float unitSizeMultiplier, string unit,
            int gizmoMaximumDecimalPoints, Transform transform)
        {
            handleLabelStyle.normal.textColor = editorSettings.SizeGizmoLabelColorZ;
            handleLabelStyle.normal.background = labelBackgroundTextureZ;

            float size = currentBound.size.z * unitSizeMultiplier;

            string zLabel = "";
            if (editorSettings.ShowAxisOnLabel)
                zLabel += "Z: ";

            if (size is > 0 and < 0.000001f)
                zLabel += " Almost 0";
            else
                zLabel += (float)Math.Round(size, gizmoMaximumDecimalPoints);

            if (editorSettings.ShowUnitOnLabel)
                zLabel += " " + unit;

            GUIContent label = new(zLabel);

            Vector3 position1;
            if (editorSettings.PositionLabelAtCenter)
                position1 = gizmoBounds.center + new Vector3(gizmoBounds.extents.x, 0, 0);
            else
                position1 = gizmoBounds.center + new Vector3(gizmoBounds.extents.x,
                    -gizmoBounds.extents.y - editorSettings.LabelOffset, 0);

            Vector3 position2;
            if (editorSettings.PositionLabelAtCenter)
                position2 = gizmoBounds.center + new Vector3(-gizmoBounds.extents.x, 0, 0);
            else
                position2 = gizmoBounds.center + new Vector3(-gizmoBounds.extents.x,
                    gizmoBounds.extents.y + editorSettings.LabelOffset, 0);

            if (editorSettings.ShowSizeGizmoLabelOnBothSide &&
                currentBound.size.x >= editorSettings.MinimumSizeForDoubleSidedLabel)
            {
                Handles.Label(position1, label, handleLabelStyle);
                Handles.Label(position2, label, handleLabelStyle);
            }
            else
            {
                if (!editorSettings.PositionLabelAtCenter)
                {
                    Vector3 position3 = gizmoBounds.center + new Vector3(gizmoBounds.extents.x,
                        gizmoBounds.extents.y + editorSettings.LabelOffset, 0);
                    Vector3 position4 = gizmoBounds.center + new Vector3(-gizmoBounds.extents.x,
                        -gizmoBounds.extents.y - editorSettings.LabelOffset, 0);

                    Handles.Label(
                        editorSettings.PositionLabelAtCornerAxis
                            ? GetClosestToSceneCamera(new[] { position1, position4 }, transform)
                            : GetClosestToSceneCamera(new[] { position1, position2, position3, position4 }, transform),
                        label,
                        handleLabelStyle);
                }
                else
                {
                    Handles.Label(GetClosestToSceneCamera(new[] { position1, position2 }, transform), label,
                        handleLabelStyle);
                }
            }
        }

        static Vector3 GetClosestToSceneCamera(Vector3[] positions, Transform transform)
        {
            Camera cam = SceneView.lastActiveSceneView?.camera;
            if (cam == null || positions == null || positions.Length == 0)
                return Vector3.zero;

            int index = 0;
            float maxZ = cam.worldToCameraMatrix.MultiplyPoint(transform.TransformPoint(positions[0])).z;

            for (int i = 1; i < positions.Length; i++)
            {
                float z = cam.worldToCameraMatrix.MultiplyPoint(transform.TransformPoint(positions[i])).z;
                if (!(z > maxZ)) continue; // higher z in camera space = visually closer
                maxZ = z;
                index = i;
            }

            return positions[index];
        }

        static Vector3 GetLeftmostFromSceneCamera(Vector3[] positions, Transform transform)
        {
            Camera cam = SceneView.lastActiveSceneView?.camera;
            if (cam == null || positions == null || positions.Length == 0)
                return Vector3.zero;

            int index = 0;
            float minX = cam.worldToCameraMatrix.MultiplyPoint(transform.TransformPoint(positions[0])).x;

            for (int i = 1; i < positions.Length; i++)
            {
                float x = cam.worldToCameraMatrix.MultiplyPoint(transform.TransformPoint(positions[i])).x;
                if (!(x < minX)) continue;
                minX = x;
                index = i;
            }

            return positions[index];
        }

        static Texture2D MakeFlatTexture(Color color)
        {
            Texture2D tex = new(1, 1, TextureFormat.RGBA32, false);
            tex.SetPixel(0, 0, color);
            tex.Apply();
            return tex;
        }

        static Vector3 Divide(Vector3 first, Vector3 second)
        {
            return new(NanFixed(first.x / second.x), NanFixed(first.y / second.y), NanFixed(first.z / second.z));
        }

        static float NanFixed(float value)
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
                return 1;

            return value;
        }

        #endregion Size

        #region Outline

        static void DrawAxisColoredWireCube(Bounds bounds)
        {
            Vector3 c = bounds.center;
            Vector3 e = bounds.extents;

            // Calculate all 8 corners of the cube
            Vector3[] corners = new Vector3[8];
            corners[0] = c + new Vector3(-e.x, -e.y, -e.z);
            corners[1] = c + new Vector3(e.x, -e.y, -e.z);
            corners[2] = c + new Vector3(e.x, -e.y, e.z);
            corners[3] = c + new Vector3(-e.x, -e.y, e.z);
            corners[4] = c + new Vector3(-e.x, e.y, -e.z);
            corners[5] = c + new Vector3(e.x, e.y, -e.z);
            corners[6] = c + new Vector3(e.x, e.y, e.z);
            corners[7] = c + new Vector3(-e.x, e.y, e.z);

            // Draw bottom square (Y is same)
            DrawColoredLine(corners[0], corners[1], editorSettings.SizeGizmoOutlineColorX); // X axis
            DrawColoredLine(corners[1], corners[2], editorSettings.SizeGizmoOutlineColorZ); // Z axis
            DrawColoredLine(corners[2], corners[3], editorSettings.SizeGizmoOutlineColorX); // X axis
            DrawColoredLine(corners[3], corners[0], editorSettings.SizeGizmoOutlineColorZ); // Z axis

            // Draw top square (Y is same)
            DrawColoredLine(corners[4], corners[5], editorSettings.SizeGizmoOutlineColorX); // X axis
            DrawColoredLine(corners[5], corners[6], editorSettings.SizeGizmoOutlineColorZ); // Z axis
            DrawColoredLine(corners[6], corners[7], editorSettings.SizeGizmoOutlineColorX); // X axis
            DrawColoredLine(corners[7], corners[4], editorSettings.SizeGizmoOutlineColorZ); // Z axis

            // Draw vertical lines (Y axis)
            DrawColoredLine(corners[0], corners[4], editorSettings.SizeGizmoOutlineColorY);
            DrawColoredLine(corners[1], corners[5], editorSettings.SizeGizmoOutlineColorY);
            DrawColoredLine(corners[2], corners[6], editorSettings.SizeGizmoOutlineColorY);
            DrawColoredLine(corners[3], corners[7], editorSettings.SizeGizmoOutlineColorY);
        }

        static void DrawColoredLine(Vector3 start, Vector3 end, Color color)
        {
            Handles.color = color;
            Handles.DrawLine(start, end, editorSettings.SizeGizmoOutlineThickness);
        }

        #endregion
    }
}