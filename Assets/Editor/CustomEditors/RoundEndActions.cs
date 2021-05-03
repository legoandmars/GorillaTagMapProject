using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VmodMonkeMapLoader.Behaviours.RoundEndActions))]
public class RoundEndActionsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }

    public void OnSceneGUI()
    {
        VmodMonkeMapLoader.Behaviours.RoundEndActions targetRoundEndActions = (VmodMonkeMapLoader.Behaviours.RoundEndActions)target;

        DrawRoundEnd(targetRoundEndActions, true);
    }

    public static void DrawRoundEnd(VmodMonkeMapLoader.Behaviours.RoundEndActions roundEndActions, bool single = false)
    {
        Handles.color = (Color.green + Color.black) / 2;

        float size = 10.0f;
        foreach (var point in roundEndActions.ObjectsToEnable)
        {
            if (Handles.Button(point.transform.position, point.transform.rotation, size, size, Handles.SphereHandleCap))
            {
                MapDescriptorEditor.moveTo(point.transform);
            }
            HandleHelpers.Label(point.transform.position + Vector3.up * (size * .5f) + Vector3.up, new GUIContent(point.name), single);
        }

        Handles.color = (Color.red + Color.black) / 2;

        foreach (var point in roundEndActions.ObjectsToDisable)
        {
            if (Handles.Button(point.transform.position, point.transform.rotation, size, size, Handles.SphereHandleCap))
            {
                MapDescriptorEditor.moveTo(point.transform);
            }
            HandleHelpers.Label(point.transform.position + Vector3.up * (size * .5f) + Vector3.up, new GUIContent(point.name), single);
        }
    }
}
