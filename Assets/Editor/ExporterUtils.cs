using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System.IO.Compression;
using VmodMonkeMapLoader.Behaviours;

public static class ExporterUtils
{
    public static PackageJSON MapDescriptorToJSON(MapDescriptor mapDescriptor)
    {
        PackageJSON packageJSON = new PackageJSON();
        packageJSON.descriptor = new Descriptor();
        packageJSON.config = new Config();
        packageJSON.descriptor.author = mapDescriptor.AuthorName;
        packageJSON.descriptor.objectName = mapDescriptor.MapName;
        packageJSON.descriptor.description = mapDescriptor.Description;
        packageJSON.config.imagePath = null;
        // do config stuff here
        return packageJSON;
    }

    public static void ExportPackage(GameObject gameObject, string path, string typeName, PackageJSON packageJSON)
    {
        string fileName = Path.GetFileName(path);
        string folderPath = Path.GetDirectoryName(path);
        string androidFileName = Path.GetFileNameWithoutExtension(path) + "_android";
        string pcFileName = Path.GetFileNameWithoutExtension(path) + "_pc";

        Selection.activeObject = gameObject;
        EditorSceneManager.MarkSceneDirty(gameObject.scene);
        EditorSceneManager.SaveScene(gameObject.scene);

        // Skybox Stuff
        MapDescriptor mapDescriptor = gameObject.GetComponent<MapDescriptor>();

        if (mapDescriptor.CustomSkybox != null)
        {
            // Create Fake Skybox
            GameObject skybox = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            skybox.name = "Skybox";
            skybox.transform.SetParent(gameObject.transform);
            skybox.transform.localScale = new Vector3(1000, 1000, 1000);
            skybox.transform.localPosition = Vector3.zero;
            skybox.transform.localRotation = Quaternion.identity;
            Object.DestroyImmediate(skybox.GetComponent<Collider>());
            Material skyboxMaterial = new Material(Shader.Find("Skybox/Cubemap"));
            skyboxMaterial.SetTexture("_Tex", mapDescriptor.CustomSkybox);
            skybox.GetComponent<Renderer>().material = skyboxMaterial;
            skybox.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
        else
        {
            // Create Fake Skybox that represents the game's
            GameObject skyboxObject = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/FakeSkybox.prefab", typeof(GameObject)) as GameObject;
            GameObject fakeSkybox = PrefabUtility.InstantiatePrefab(skyboxObject) as GameObject;
            fakeSkybox.transform.SetParent(gameObject.transform);
            fakeSkybox.transform.localPosition = Vector3.zero;
        }

        // Take Screenshots with the thumbnail camera
        Camera thumbnailCamera = gameObject.transform.Find("ThumbnailCamera")?.GetComponent<Camera>();
        if(thumbnailCamera != null)
        {
            // Normal Screenshot
            Texture2D screenshot = CaptureScreenshot(thumbnailCamera, 512, 512);
            byte[] screenshotPNG = ImageConversion.EncodeToPNG(screenshot);
            File.WriteAllBytes(Application.temporaryCachePath + "/preview.png", screenshotPNG);
            packageJSON.config.imagePath = "preview.png";

            // Cubemap Screenshot
            Texture2D screenshotCubemap = CaptureCubemap(thumbnailCamera, 1024, 1024);
            byte[] screenshotCubemapPNG = ImageConversion.EncodeToPNG(screenshotCubemap);
            File.WriteAllBytes(Application.temporaryCachePath + "/preview_cubemap.png", screenshotCubemapPNG);
            packageJSON.config.cubemapImagePath = "preview_cubemap.png";

            /* quest stuff (disabled for now)
            byte[] screenshotRaw = screenshot.GetRawTextureData();
            File.WriteAllBytes(Application.temporaryCachePath + "/preview_quest", screenshotRaw);
            */
        }
        Object.DestroyImmediate(thumbnailCamera.gameObject);

        // Pre-Process stuff for both platforms - PC and Android.
        GameObject spawnPointContainer = new GameObject("SpawnPointContainer");
        spawnPointContainer.transform.SetParent(gameObject.transform);
        spawnPointContainer.transform.localPosition = Vector3.zero;
        spawnPointContainer.transform.localRotation = Quaternion.identity;
        spawnPointContainer.transform.localScale = Vector3.one;
         
        List<string> spawnPointNames = new List<string>();

        for(int i = 0; i < mapDescriptor.SpawnPoints.Length; i++)
        {
            Transform spawnPointTransform = mapDescriptor.SpawnPoints[i].gameObject.transform;
            Vector3 oldPos = spawnPointTransform.position;
            var oldRotation = spawnPointTransform.rotation;

            spawnPointTransform.SetParent(spawnPointContainer.transform);
            spawnPointTransform.rotation = oldRotation;
            spawnPointTransform.position = oldPos;

            string nameString = "SpawnPoint" + (i < 10 ? "0" + i : i.ToString());
            mapDescriptor.SpawnPoints[i].gameObject.name = nameString;
            spawnPointNames.Add(nameString);
        }
        packageJSON.config.spawnPoints = spawnPointNames.ToArray();

        PrefabUtility.SaveAsPrefabAsset(Selection.activeObject as GameObject, $"Assets/_{typeName}.prefab");
        AssetBundleBuild assetBundleBuild = default;
        assetBundleBuild.assetNames = new string[] { $"Assets/_{typeName}.prefab" };
        assetBundleBuild.assetBundleName = pcFileName;
        
        // Build for PC
        BuildTargetGroup selectedBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        BuildTarget activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;

        BuildPipeline.BuildAssetBundles(Application.temporaryCachePath, new AssetBundleBuild[] { assetBundleBuild }, 0, BuildTarget.StandaloneWindows64);

        // Do Android specific stuff here. Stripping MonoBehaviours and converting them to TextAssets, etc.
        foreach (TagZone zone in gameObject.GetComponentsInChildren<TagZone>())
        {
            CreateQuestText("{\"TagZone\": true}", zone.gameObject);
            Object.DestroyImmediate(zone);
        }

        foreach(SurfaceClimbSettings surfaceClimbSettings in gameObject.GetComponentsInChildren<SurfaceClimbSettings>())
        {
            SurfaceClimbSettingsJSON settingsJson = new SurfaceClimbSettingsJSON();
            settingsJson.Unclimbable = surfaceClimbSettings.Unclimbable;
            settingsJson.slipPercentage = surfaceClimbSettings.slipPercentage;

            CreateQuestText(JsonUtility.ToJson(settingsJson), surfaceClimbSettings.gameObject);
            Object.DestroyImmediate(surfaceClimbSettings);
        }

        int triggerCount = 1;
        foreach (ObjectTrigger objectTrigger in gameObject.GetComponentsInChildren<ObjectTrigger>())
        {
            string objectName = "ObjectTrigger" + triggerCount;
            if(objectTrigger.ObjectToTrigger != null)
            {
                CreateQuestText("{\"TriggeredBy\": \"" + objectName + "\"}", objectTrigger.ObjectToTrigger);
            }
            ObjectTriggerJSON triggerJSON = new ObjectTriggerJSON();
            triggerJSON.ObjectTriggerName = objectName;
            triggerJSON.OnlyTriggerOnce = objectTrigger.OnlyTriggerOnce;

            CreateQuestText(JsonUtility.ToJson(triggerJSON), objectTrigger.gameObject);
            Object.DestroyImmediate(objectTrigger);
            triggerCount++;
        }

        int teleporterCount = 1;
        foreach (Teleporter teleporter in gameObject.GetComponentsInChildren<Teleporter>())
        {
            string teleporterName = "Teleporter" + teleporterCount;
            foreach(Transform teleportPoint in teleporter.TeleportPoints)
            {
                CreateQuestText("{\"TeleportPoint\": \"" + teleporterName + "\"}", teleportPoint.gameObject);
            }
            teleporter.TeleportPoints = null;
            string teleporterJSON = JsonUtility.ToJson(teleporter);
            teleporterJSON = teleporterJSON.Replace("\"TeleportPoints\":[],", "\"TeleporterName\": \""+ teleporterName +"\",");

            CreateQuestText(teleporterJSON, teleporter.gameObject);
            Object.DestroyImmediate(teleporter);
            teleporterCount++;
        }

        gameObject.GetComponent<MapDescriptor>().enabled = false;
        Object.DestroyImmediate(gameObject.GetComponent<MapDescriptor>());

        // Do it again for Android
        PrefabUtility.SaveAsPrefabAsset(Selection.activeObject as GameObject, $"Assets/_{typeName}.prefab"); // are these next 2 lines necessary? idk. probably test it.
        assetBundleBuild.assetNames = new string[] { $"Assets/_{typeName}.prefab" };
        assetBundleBuild.assetBundleName = androidFileName;
        BuildPipeline.BuildAssetBundles(Application.temporaryCachePath, new AssetBundleBuild[] { assetBundleBuild }, 0, BuildTarget.Android);

        EditorPrefs.SetString("currentBuildingAssetBundlePath", folderPath);

        // JSON stuff
        packageJSON.androidFileName = androidFileName;
        packageJSON.pcFileName = pcFileName;
        string json = JsonUtility.ToJson(packageJSON, true);
        File.WriteAllText(Application.temporaryCachePath + "/package.json", json);
        AssetDatabase.DeleteAsset($"Assets/_{typeName}.prefab");

        // Delete the zip if it already exists and re-zip
        List<string> files = new List<string> {
            Application.temporaryCachePath + "/" + pcFileName,
            Application.temporaryCachePath + "/" + androidFileName,
            Application.temporaryCachePath + "/package.json",
            Application.temporaryCachePath + "/preview.png",
            Application.temporaryCachePath + "/preview_cubemap.png"
        };

        if (File.Exists(Application.temporaryCachePath + "/tempZip.zip")) File.Delete(Application.temporaryCachePath + "/tempZip.zip");
 
        CreateZipFile(Application.temporaryCachePath + "/tempZip.zip", files);

        // After zipping, clear some assets from the temp folder
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        foreach(string file in files) if (File.Exists(file)) File.Delete(file);

        // Move the ZIP and finalize
        File.Move(Application.temporaryCachePath + "/tempZip.zip", path);
        //Object.DestroyImmediate(gameObject);
        AssetDatabase.Refresh();
    }

    public static void CreateQuestText(string textToAdd, GameObject gameObject)
    {
        Text newText = gameObject.GetComponent<Text>();
        if(newText == null) newText = gameObject.AddComponent<Text>();
        if(newText.text != null && newText.text != "")
        {
            newText.text += ", ";
        }
        newText.text += textToAdd;
        newText.fontSize = 0;
    }

    public static void CreateZipFile(string fileName, IEnumerable<string> files)
    {
        // Create and open a new ZIP file
        var zip = ZipFile.Open(fileName, ZipArchiveMode.Create);
        foreach (var file in files)
        {
            // Add the entry for each file
            zip.CreateEntryFromFile(file, Path.GetFileName(file), System.IO.Compression.CompressionLevel.Optimal);
        }
        // Dispose of the object when we are done
        zip.Dispose();
    }

    public static Texture2D CaptureCubemap(Camera cam, int width, int height)
    {
        cam = UnityEngine.Object.Instantiate(cam.gameObject).GetComponent<Camera>();
        var render_texture = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32);
        var equi_texture = RenderTexture.GetTemporary(width, height, 16, RenderTextureFormat.ARGB32);
        var tex = new Texture2D(width, height, TextureFormat.ARGB32, false);

        render_texture.dimension = UnityEngine.Rendering.TextureDimension.Cube;

        cam.RenderToCubemap(render_texture);
        render_texture.ConvertToEquirect(equi_texture);
        RenderTexture.active = equi_texture;
        tex.ReadPixels(new Rect(0, 0, equi_texture.width, equi_texture.height), 0, 0);

        RenderTexture.active = null;
        Object.DestroyImmediate(cam.gameObject);
        Object.DestroyImmediate(render_texture);
        RenderTexture.ReleaseTemporary(equi_texture);
        return tex;
    }

    public static Texture2D CaptureScreenshot(Camera cam, int width, int height)
    {
        cam = UnityEngine.Object.Instantiate(cam.gameObject).GetComponent<Camera>();

        RenderTexture renderTex = RenderTexture.GetTemporary(width, height, 16, RenderTextureFormat.ARGB32);
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        RenderTexture.active = renderTex;
        cam.targetTexture = renderTex;
        cam.Render();
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);

        RenderTexture.active = null;
        Object.DestroyImmediate(cam.gameObject);
        RenderTexture.ReleaseTemporary(renderTex);
        return tex;
    }
}
