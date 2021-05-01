using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VmodMonkeMapLoader.Behaviours.TagZone))]
public class TagZoneEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }

    public void OnSceneGUI()
    {
        VmodMonkeMapLoader.Behaviours.TagZone zone = (VmodMonkeMapLoader.Behaviours.TagZone)target;
        DrawTagZone(zone, true);
    }

    public static void DrawTagZone(VmodMonkeMapLoader.Behaviours.TagZone zone, bool single = false)
    {
        Handles.color = Color.red;

        if (Handles.Button(zone.transform.position, zone.transform.rotation, 1.0f, 1.0f, Handles.CubeHandleCap))
        {
            MapDescriptorEditor.moveTo(zone.transform);
        }
        HandleHelpers.Label(zone.transform.position + Vector3.up, new GUIContent(zone.gameObject.name));
    }
}
