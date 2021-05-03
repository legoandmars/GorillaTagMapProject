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

    public void OnSceneGUI()
    {
        VmodMonkeMapLoader.Behaviours.MapDescriptor targetDescriptor = (VmodMonkeMapLoader.Behaviours.MapDescriptor)target;
        GameObject gameObject = targetDescriptor.gameObject;

        Handles.BeginGUI();
        GUILayout.BeginVertical(MapDescriptorConfig.textStyle, GUILayout.Width(MapDescriptorConfig.guiWidth));

        GUILayout.Label(targetDescriptor.MapName, EditorStyles.boldLabel);

        MapDescriptorConfig.DisplayTeleporters = GUILayout.Toggle(MapDescriptorConfig.DisplayTeleporters, "Display Teleporters");
        MapDescriptorConfig.DisplaySpawnPoints = GUILayout.Toggle(MapDescriptorConfig.DisplaySpawnPoints, "Display Spawn Points");
        MapDescriptorConfig.DisplayTagZones = GUILayout.Toggle(MapDescriptorConfig.DisplayTagZones, "Display Tag Zones");
        MapDescriptorConfig.DisplayObjectTriggers = GUILayout.Toggle(MapDescriptorConfig.DisplayObjectTriggers, "Display Object Triggers");
        MapDescriptorConfig.DisplaySurfaceSettings = GUILayout.Toggle(MapDescriptorConfig.DisplaySurfaceSettings, "Display Surface Settings");
        MapDescriptorConfig.DisplayRoundEndActions = GUILayout.Toggle(MapDescriptorConfig.DisplayRoundEndActions, "Display Round End Action Objects");

        GUILayout.Space(5.0f);
        MapDescriptorConfig.DisplayNames = GUILayout.Toggle(MapDescriptorConfig.DisplayNames, "Display Object Names");

        GUILayout.EndVertical();
        Handles.EndGUI();

        if (MapDescriptorConfig.DisplaySpawnPoints)
        {
            targetDescriptor.SpawnPoints = GetAllSpawnPoints(gameObject).ToArray();
            foreach (var point in targetDescriptor.SpawnPoints)
            {
                DrawSpawnPoint(point);
            }
        }

        if (MapDescriptorConfig.DisplayTeleporters)
        {
            foreach (var tele in gameObject.GetComponentsInChildren<VmodMonkeMapLoader.Behaviours.Teleporter>(true))
            {
                TeleporterEditor.DrawTeleport(tele);
            }
        }

        if (MapDescriptorConfig.DisplayTagZones)
        {
            foreach (var zone in gameObject.GetComponentsInChildren<VmodMonkeMapLoader.Behaviours.TagZone>(true))
            {
                TagZoneEditor.DrawTagZone(zone);
            }
        }

        if (MapDescriptorConfig.DisplayObjectTriggers)
        {
            foreach (var trigger in gameObject.GetComponentsInChildren<VmodMonkeMapLoader.Behaviours.ObjectTrigger>(true))
            {
                TriggerEditor.DrawTrigger(trigger);
            }
        }

        if (MapDescriptorConfig.DisplaySurfaceSettings)
        {
            foreach (var surface in gameObject.GetComponentsInChildren<VmodMonkeMapLoader.Behaviours.SurfaceClimbSettings>(true))
            {
                SurfaceClimbSettingsEditor.DrawSurface(surface);
            }
        }

        if (MapDescriptorConfig.DisplayRoundEndActions)
        {
            foreach (var roundEndActions in gameObject.GetComponentsInChildren<VmodMonkeMapLoader.Behaviours.RoundEndActions>(true))
            {
                RoundEndActionsEditor.DrawRoundEnd(roundEndActions);
            }
        }
    }

    public static void DrawSpawnPoint(Transform point)
    {
        Handles.color = Color.magenta;

        if (Handles.Button(point.position, point.rotation, 1.0f, 1.0f, Handles.CubeHandleCap))
        {
            moveTo(point);
        }

        HandleHelpers.Label(point.position + Vector3.up * point.lossyScale.y, new GUIContent(point.gameObject.name));
    }

    public static void moveTo(Transform point)
    {
        SceneView.lastActiveSceneView.pivot = point.position;
        SceneView.lastActiveSceneView.rotation = Quaternion.LookRotation((point.position - SceneView.lastActiveSceneView.camera.transform.position).normalized * .5f);
        Selection.activeGameObject = point.gameObject;

    }

    public static List<Transform> GetAllSpawnPoints(GameObject root)
    {
        List<Transform> spawnPoints = new List<Transform>();

        var descriptor = root.GetComponent<VmodMonkeMapLoader.Behaviours.MapDescriptor>();
        if (descriptor != null)
        {
            foreach (var t in descriptor.SpawnPoints)
            {
                spawnPoints.Add(t);
            }
        }

        foreach (var renderer in root.GetComponentsInChildren<Renderer>())
        {
            if (spawnPoints.Contains(renderer.transform)) continue;
            bool foundMat = false;
            foreach (var mat in renderer.sharedMaterials)
            {
                if (mat.name == "Spawn Point")
                {
                    foundMat = true;
                    break;
                }
            }

            if (foundMat)
            {
                spawnPoints.Add(renderer.transform);
            }

        }

        return spawnPoints;
    }
}
