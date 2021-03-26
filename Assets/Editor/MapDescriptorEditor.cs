using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VmodMonkeMapLoader.Behaviours.MapDescriptor))]
public class MapDescriptorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        VmodMonkeMapLoader.Behaviours.MapDescriptor targetDescriptor = (VmodMonkeMapLoader.Behaviours.MapDescriptor)target;

        DrawDefaultInspector();

        GUILayout.Space(10);

        if (GUILayout.Button("Export Map"))
        {
            if (!ExporterUtils.BuildTargetInstalled(BuildTarget.Android) && EditorUtility.DisplayDialog("Android Build Support missing", "You don't have Android Build Support installed for this Unity version. Please install it and do NOT continue unless you know for sure what you're doing.", "Cancel", "Continue Anyways")) return;

            GameObject noteObject = targetDescriptor.gameObject;
            string path = EditorUtility.SaveFilePanel("Save map file", "", targetDescriptor.MapName + ".gtmap", "gtmap");

            if (path != "")
            {
                EditorUtility.SetDirty(targetDescriptor);

                if (noteObject.transform.Find("ThumbnailCamera") != null)
                {
                    ExporterUtils.ExportPackage(noteObject, path, "Map", ExporterUtils.MapDescriptorToJSON(targetDescriptor));
                    EditorUtility.DisplayDialog("Exportation Successful!", "Exportation Successful!", "OK");
                }
                else
                {
                    EditorUtility.DisplayDialog("Exportation Failed!", "No thumbnail camera.", "OK");
                }
            }
            Debug.Log("YOO");
        }
    }
}
