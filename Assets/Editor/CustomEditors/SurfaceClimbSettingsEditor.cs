using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VmodMonkeMapLoader.Behaviours.SurfaceClimbSettings))]
public class SurfaceClimbSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

    }

    public void OnSceneGUI()
    {
        VmodMonkeMapLoader.Behaviours.SurfaceClimbSettings targetSurface = (VmodMonkeMapLoader.Behaviours.SurfaceClimbSettings)target;

        DrawSurface(targetSurface, true);
    }

    static public void DrawSurface(VmodMonkeMapLoader.Behaviours.SurfaceClimbSettings surface, bool single = false)
    {
        Handles.color = surface.Unclimbable ? Color.white : (Color.cyan + Color.white) / 2.0f;
        foreach (var collider in surface.gameObject.GetComponents<Collider>())
        {
            if (collider is MeshCollider)
            {
                Vector3[] verts = (collider as MeshCollider).sharedMesh.vertices;
                List<Vector3> positions = new List<Vector3>();
                foreach (var vert in verts)
                {
                    if (!positions.Contains(vert)) positions.Add(vert);
                }

                foreach (var vert in positions)
                {
                    Vector3 temp = new Vector3(vert.x * collider.transform.lossyScale.x, vert.y * collider.transform.lossyScale.y, vert.z * collider.transform.lossyScale.z) + collider.transform.position;
                    if (single)
                    {
                        Handles.SphereHandleCap(0, temp, Quaternion.identity, .5f, EventType.Repaint);
                    }
                    else if (Handles.Button(temp, Quaternion.identity, .5f, .5f, Handles.SphereHandleCap))
                    {
                        MapDescriptorEditor.moveTo(collider.transform);
                    }
                }
            }
            else if (collider is SphereCollider)
            {
                float size = (collider as SphereCollider).radius * collider.transform.lossyScale.magnitude;
                Vector3 pos = collider.transform.position;

                if (single)
                {
                    Handles.SphereHandleCap(0, pos, Quaternion.identity, size, EventType.Repaint);
                }
                else if (Handles.Button(pos, Quaternion.identity, size, size, Handles.SphereHandleCap))
                {
                    MapDescriptorEditor.moveTo(collider.transform);
                }
            }
            else
            {
                if (single)
                {
                    Handles.CubeHandleCap(0, collider.transform.position, Quaternion.identity, 0.5f, EventType.Repaint);
                }
                else if (Handles.Button(collider.transform.position, Quaternion.identity, 0.5f, 0.5f, Handles.CubeHandleCap))
                {
                    MapDescriptorEditor.moveTo(collider.transform);
                }
            }
        }

        if (single)
        {
            Handles.BeginGUI();
            GUILayout.BeginVertical(MapDescriptorConfig.textStyle, GUILayout.Width(MapDescriptorConfig.guiWidth));

            GUILayout.Label(surface.gameObject.name, EditorStyles.boldLabel);

            surface.Unclimbable = EditorGUILayout.ToggleLeft("Unclimbable", surface.Unclimbable);
            if (!surface.Unclimbable)
            {
                surface.slipPercentage = EditorGUILayout.FloatField("Slip Percentage", surface.slipPercentage);
            }

            GUILayout.EndVertical();
            Handles.EndGUI();
        }
    }
}
