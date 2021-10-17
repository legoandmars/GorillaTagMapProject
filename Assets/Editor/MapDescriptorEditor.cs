using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using VmodMonkeMapLoader.Helpers;

[CustomEditor(typeof(VmodMonkeMapLoader.Behaviours.MapDescriptor))]
public class MapDescriptorEditor : Editor
{
    Texture2D texture;
    bool textureGenerated = false;

    bool playerSettingsOpened = true;
    bool mapSettingsOpened = true;

    SerializedProperty gameModeProperty;
    int gameMode = 0;

    string[] allProperties;

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

    void OnEnable()
	{
        gameModeProperty = serializedObject.FindProperty("GameMode");
        if (gameModeProperty.stringValue.ToLower() == "casual")
		{
            gameMode = 1;
		}
        allProperties = typeof(VmodMonkeMapLoader.Behaviours.MapDescriptor).GetFields().Select(x => x.Name).Append("m_Script").ToArray();
	}

    void DrawProperties(params string[] properties)
	{
        DrawPropertiesExcluding(serializedObject, allProperties.Except(properties).ToArray());
	}

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        VmodMonkeMapLoader.Behaviours.MapDescriptor targetDescriptor = (VmodMonkeMapLoader.Behaviours.MapDescriptor)target;

        GUILayout.BeginVertical();

        DrawProperties("MapName", "AuthorName", "Description");

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        playerSettingsOpened = EditorGUILayout.Foldout(playerSettingsOpened, "Player Settings");
        if (playerSettingsOpened)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical(GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.9f));

            List<string> properties = new List<string> { "GravitySpeed", "SlowJumpLimit", "SlowJumpMultiplier"};
            if (gameMode != 1) properties.AddRange(new string[]{ "FastJumpLimit", "FastJumpMultiplier" });
            DrawProperties(properties.ToArray());
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Reset Properties"))
			{
                serializedObject.FindProperty("GravitySpeed").floatValue = SharedConstants.Gravity;
                serializedObject.FindProperty("SlowJumpLimit").floatValue = SharedConstants.SlowJumpLimit;
				serializedObject.FindProperty("FastJumpLimit").floatValue = SharedConstants.FastJumpLimit;
				serializedObject.FindProperty("SlowJumpMultiplier").floatValue = SharedConstants.SlowJumpMultiplier;
			    serializedObject.FindProperty("FastJumpMultiplier").floatValue = SharedConstants.FastJumpMultiplier;
			}
        }

        mapSettingsOpened = EditorGUILayout.Foldout(mapSettingsOpened, "Map Settings");
        if (mapSettingsOpened)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical(GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.9f));
            DrawProperties("SpawnPoints", "CustomSkybox", "ExportLighting", "RequiredPCModsId", "RequiredQuestModsId");
            gameMode = EditorGUILayout.Popup("Game Mode", gameMode, new string[] { "Default", "Casual" });
            gameModeProperty.stringValue = new string[] { "", "casual" }[gameMode]; 

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Thumbnail Preview");
        if(GUILayout.Button("Refresh Preview")) GeneratePreview();
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

                ExporterUtils.ExportPackage(noteObject, path, "Map");
            }
        }

        try
        {
            serializedObject.ApplyModifiedProperties();
        }
        catch
        {
            // serialized object doesn't exist. sometimes this happens when switching scenes.
        }
    }
}
