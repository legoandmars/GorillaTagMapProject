using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VmodMonkeMapLoader.Behaviours;

[CustomEditor(typeof(MapDescriptor))]
public class MapDescriptorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapDescriptor targetDescriptor = (MapDescriptor)target;

        DrawDefaultInspector();

        GUILayout.Space(10);

        if (GUILayout.Button("Export Map"))
        {
            GameObject noteObject = targetDescriptor.gameObject;
            string path = EditorUtility.SaveFilePanel("Save map file", "", targetDescriptor.MapName + ".gtmap", "gtmap");
            Debug.Log(path == "");

            if (path != "")
            {
                EditorUtility.SetDirty(targetDescriptor);

                if (noteObject.transform.Find("ThumbnailCamera") != null)
                {
                    //noteObject = Instantiate(noteObject);
                    // do stuff 
                    //try
                    //{
                        ExporterUtils.ExportPackage(noteObject, path, "Map", ExporterUtils.MapDescriptorToJSON(targetDescriptor));
                        EditorUtility.DisplayDialog("Exportation Successful!", "Exportation Successful!", "OK");
                    //}
                   //catch (System.Exception e)
                    //{
                    //   EditorUtility.DisplayDialog("Error!", e.Message, "OK");
                    //    DestroyImmediate(noteObject);
                    //}
                }
                else
                {
                    EditorUtility.DisplayDialog("Exportation Failed!", "No thumbnail camera.", "OK");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Exportation Failed!", "Path is invalid.", "OK");
            }
            Debug.Log("YOO");
        }
    }
}
