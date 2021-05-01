using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VmodMonkeMapLoader.Behaviours.Teleporter))]
public class TeleporterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        VmodMonkeMapLoader.Behaviours.Teleporter targetTele = (VmodMonkeMapLoader.Behaviours.Teleporter)target;

        DrawDefaultInspector();
    }

    public void OnSceneGUI()
    {
        VmodMonkeMapLoader.Behaviours.Teleporter targetTele = (VmodMonkeMapLoader.Behaviours.Teleporter)target;

        DrawTeleport(targetTele, true);
    }

    static public void DrawTeleport(VmodMonkeMapLoader.Behaviours.Teleporter targetTele, bool single = false)
    {
        Color theColor = Color.green;
        if (targetTele.TagOnTeleport) theColor = Color.Lerp(Color.red, Color.yellow, .5f);
        Handles.color = theColor;

        if (targetTele.TeleportPoints == null || targetTele.TeleportPoints.Count == 0)
        {
            Handles.SphereHandleCap(0, targetTele.transform.position, targetTele.transform.rotation, 1.0f, EventType.Repaint);
            if (!single) HandleHelpers.ResizeLabel(targetTele.transform.position + Vector3.up, "TreeTeleporter");
        }
        else
        {
            for (int i = 0; i < targetTele.TeleportPoints.Count; i++)
            {
                Transform point = targetTele.TeleportPoints[i];
                if (point == null)
                {
                    targetTele.TeleportPoints.RemoveAt(i);
                    continue;
                }
                DrawPoint(point, targetTele.transform, theColor, single);
            }

            Handles.color = theColor;

            if (!single)
            {
                HandleHelpers.ResizeLabel(targetTele.transform.position + Vector3.up, targetTele.gameObject.name);
                if (Handles.Button(targetTele.transform.position, targetTele.transform.rotation, 1.0f, 1.0f, Handles.SphereHandleCap))
                {
                    MapDescriptorEditor.moveTo(targetTele.transform);
                }
            }
            else
            {
                Handles.SphereHandleCap(0, targetTele.transform.position, targetTele.transform.rotation, 1.0f, EventType.Repaint);
            }
        }

        if (single)
        {
            Vector3 position = targetTele.transform.position + targetTele.transform.forward * 2;
            Quaternion rotation = SceneView.lastActiveSceneView.rotation;
            //Handles.Rectang;

            Handles.BeginGUI();

            Rect vertical = EditorGUILayout.BeginVertical(MapDescriptorConfig.textStyle, GUILayout.Width(MapDescriptorConfig.guiWidth));
            GUILayout.Label(targetTele.gameObject.name, EditorStyles.boldLabel);
            
            if (GUILayout.Button("Add New Teleport Point"))
            {
                AddPoint(targetTele);
            }

            targetTele.TagOnTeleport = EditorGUILayout.Toggle("Tag on Teleport", targetTele.TagOnTeleport);

            targetTele.TouchType = (VmodMonkeMapLoader.Behaviours.GorillaTouchType)EditorGUILayout.EnumPopup("TouchType", targetTele.TouchType);

            targetTele.Delay = EditorGUILayout.FloatField("Delay", targetTele.Delay);
            EditorGUILayout.EndVertical();
            Handles.EndGUI();
        }
    }

    static public void DrawPoint(Transform point, Transform teleporter, Color color, bool single = false)
    {
        if (point == null) return;
        Handles.color = Color.white;

        if (Handles.Button(point.position, point.rotation, 0.8f, 0.8f, Handles.SphereHandleCap))
        {
            MapDescriptorEditor.moveTo(point);
        }

        Handles.color = color;
        HandleHelpers.DrawArrow(teleporter.position, point.position, 2.0f);
        Handles.color = Color.blue;
        HandleHelpers.DrawArrow(point.position, point.position + point.forward * 1.75f, 1.5f);

        if (single)
        {
            HandleHelpers.Label(point.position + Vector3.up, point.gameObject.name);
        }
        else
        {
            HandleHelpers.ResizeLabel(point.position + Vector3.up, point.gameObject.name);
        }
    }

    static public void AddPoint(VmodMonkeMapLoader.Behaviours.Teleporter teleporter)
    {
        Transform point = Object.Instantiate(MapDescriptorConfig.TeleportPoint, teleporter.transform.parent).transform;

        teleporter.TeleportPoints.Add(point);

        point.transform.position = teleporter.transform.position;
        Selection.activeObject = point.gameObject;
    }

    public static List<Transform> GetAllTeleportPoints(GameObject root)
    {
        List<Transform> teleportPoints = new List<Transform>();

        var teleporter = root.GetComponent<VmodMonkeMapLoader.Behaviours.Teleporter>();
        if (teleporter != null)
        {
            foreach (var t in teleporter.TeleportPoints)
            {
                teleportPoints.Add(t);
            }
        }

        foreach (var renderer in root.GetComponentsInChildren<Renderer>())
        {
            if (teleportPoints.Contains(renderer.transform)) continue;
            bool foundMat = false;
            foreach (var mat in renderer.sharedMaterials)
            {
                if (mat.name == "Teleport Point")
                {
                    foundMat = true;
                    break;
                }
            }

            if (foundMat)
            {
                teleportPoints.Add(renderer.transform);
            }
        }

        return teleportPoints;
    }
}
