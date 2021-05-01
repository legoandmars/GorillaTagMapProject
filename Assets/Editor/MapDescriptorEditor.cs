using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VmodMonkeMapLoader.Behaviours.MapDescriptor))]
public class MapDescriptorEditor : Editor
{
    Texture2D texture;
    bool textureGenerated = false;

    bool playerSettingsOpened = true;
    bool mapSettingsOpened = true;

    void GeneratePreview()
    {
        VmodMonkeMapLoader.Behaviours.MapDescriptor targetDescriptor = (VmodMonkeMapLoader.Behaviours.MapDescriptor)target;
        GameObject previewSource = targetDescriptor.gameObject;
        if (previewSource == null) return;
        bool foundCamera = false;
        foreach(Camera cam in targetDescriptor.gameObject.GetComponentsInChildren<Camera>())
        {
            if(cam != null && cam.gameObject.name == "ThumbnailCamera")
            {
                foundCamera = true;
                textureGenerated = true;
                texture = ExporterUtils.CaptureScreenshot(cam, 512, 512);
                texture.Apply();
            }
        }
        if (!foundCamera)
        {
            textureGenerated = true;
            try
            {
                texture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/TemplatePrefabs/NoThumbnailCamera.png");
                texture.Apply();
            }
            catch
            {
                // error loading texture. probably doesn't exist. oh well
            }
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        VmodMonkeMapLoader.Behaviours.MapDescriptor targetDescriptor = (VmodMonkeMapLoader.Behaviours.MapDescriptor)target;

        GUILayout.BeginVertical();
        DrawPropertiesExcluding(serializedObject, "GravitySpeed", "SlowJumpLimit", "FastJumpLimit", "SlowJumpMultiplier", "FastJumpMultiplier", "SpawnPoints", "CustomSkybox", "ExportLighting", "m_Script");
        // Don't like these hardcoded values. Maybe find a more automatic solution
        // DrawDefaultInspector();

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        playerSettingsOpened = EditorGUILayout.Foldout(playerSettingsOpened, "Player Settings");
        if (playerSettingsOpened)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical(GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.9f));
            DrawPropertiesExcluding(serializedObject, "MapName", "AuthorName", "Description", "SpawnPoints", "CustomSkybox", "ExportLighting", "m_Script");
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

        }

        mapSettingsOpened = EditorGUILayout.Foldout(mapSettingsOpened, "Map Settings");
        if (mapSettingsOpened)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical(GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.9f));
            DrawPropertiesExcluding(serializedObject, "MapName", "AuthorName", "Description", "m_Script", "GravitySpeed", "SlowJumpLimit", "FastJumpLimit", "SlowJumpMultiplier", "FastJumpMultiplier");
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Thumbnail Preview");
        GUILayout.Button("Refresh Preview");
        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        if (textureGenerated == false) GeneratePreview();

        float smallest = EditorGUIUtility.currentViewWidth;
        float width = smallest * .9f;
        float widthCenter = (EditorGUIUtility.currentViewWidth - (width < 512 ? width : 512)) / 2;
        float heightDifference = smallest * .02f;

        GUILayout.EndVertical();
        float lastSpace = GUILayoutUtility.GetLastRect().height;
        // Debug.Log(lastSpace);
        GUILayout.Space(width);

        GUI.Label(new Rect(widthCenter + 7, lastSpace, width, width), texture);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        if (GUILayout.Button("Export Map"))
        {
            if (!ExporterUtils.BuildTargetInstalled(BuildTarget.Android) && EditorUtility.DisplayDialog("Android Build Support missing", "You don't have Android Build Support installed for this Unity version. Please install it and do NOT continue unless you know for sure what you're doing.", "Cancel", "Continue Anyways")) return;

            GameObject noteObject = targetDescriptor.gameObject;
            string path = EditorUtility.SaveFilePanel("Save map file", "", targetDescriptor.MapName + ".gtmap", "gtmap");

            if (path != "")
            {
                EditorUtility.SetDirty(targetDescriptor);

                ExporterUtils.ExportPackage(noteObject, path, "Map", ExporterUtils.MapDescriptorToJSON(targetDescriptor));
                EditorUtility.DisplayDialog("Exportation Successful!", "Exportation Successful!", "OK");
                EditorUtility.RevealInFinder(path);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
