using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using VmodMonkeMapLoader.Behaviours;
using VmodMonkeMapLoader.Helpers;
using VmodMonkeMapLoader.Models;

public static class ExporterUtils
{
    static bool DebugPrefabs = false;
    static bool exporting = false;

    public static bool BuildTargetInstalled(BuildTarget target)
    {
        // Unity, why do I have to use reflection for this?
        // Why?
        // Wrapped in a try catch because it can be wacky and give errors sometimes
        try
        {
            var moduleManager = System.Type.GetType("UnityEditor.Modules.ModuleManager, UnityEditor.dll");
            var isPlatformSupportLoaded = moduleManager.GetMethod("IsPlatformSupportLoaded", BindingFlags.Static | BindingFlags.NonPublic);
            var getTargetStringFromBuildTarget = moduleManager.GetMethod("GetTargetStringFromBuildTarget", BindingFlags.Static | BindingFlags.NonPublic);

            return (bool)isPlatformSupportLoaded.Invoke(null, new object[] { (string)getTargetStringFromBuildTarget.Invoke(null, new object[] { target }) });
        }
        catch
        {
            Debug.Log("Checking for build target failed, reverting to true");
            return true;
        }
    }

    public static MapPackageInfo MapDescriptorToJSON(MapDescriptor mapDescriptor)
    {
        MapPackageInfo packageInfo = new MapPackageInfo
        {
            Config = new MapConfig(),
            Descriptor = new VmodMonkeMapLoader.Models.Descriptor()
        };

        packageInfo.Descriptor.Author = mapDescriptor.AuthorName;
        packageInfo.Descriptor.Name = mapDescriptor.MapName;
        packageInfo.Descriptor.Description = mapDescriptor.Description;

        packageInfo.Config.ImagePath = null;
        packageInfo.Config.Gravity = mapDescriptor.GravitySpeed;
        packageInfo.Config.SlowJumpLimit = mapDescriptor.SlowJumpLimit;
        packageInfo.Config.SlowJumpMultiplier = mapDescriptor.SlowJumpMultiplier;
        packageInfo.Config.FastJumpLimit = mapDescriptor.FastJumpLimit;
        packageInfo.Config.FastJumpMultiplier = mapDescriptor.FastJumpMultiplier;
        packageInfo.Config.GameMode = mapDescriptor.GameMode;
        packageInfo.Config.CustomData = mapDescriptor.CustomData;
        packageInfo.Config.RequiredPCModIDs = mapDescriptor.RequiredPCModsId;
        packageInfo.Config.RequiredQuestModIDs = mapDescriptor.RequiredQuestModsId;

        return packageInfo;
    }

    public static void ExportPackage(GameObject gameObject, string path, string typeName)
    {
        if (exporting) return;
        string fileName = Path.GetFileName(path);
        string folderPath = Path.GetDirectoryName(path);
        string androidFileName = Path.GetFileNameWithoutExtension(path) + "_android";
        string pcFileName = Path.GetFileNameWithoutExtension(path) + "_pc";

        string assetBundleScenePath = $"Assets/Editor/ExportScene.unity";

        string oldScenePath = gameObject.scene.path;
        if (oldScenePath != null) EditorSceneManager.SaveScene(gameObject.scene);
        try
        {
            // Disable scene checking because it'll be loading the scene to export
            SceneChecker.disabled = true;
            exporting = true;

            Selection.activeObject = gameObject;
            MapDescriptor mapDescriptor = gameObject.GetComponent<MapDescriptor>();

            if (!mapDescriptor.ExportLighting)
            {
                Lightmapping.Clear();
                Lightmapping.ClearLightingDataAsset();
            }

            //Check if editor folder exists just in case, since some people move it accidentally or something
            if (!AssetDatabase.IsValidFolder("Assets/Editor"))
            {
                Debug.LogWarning("The Editor folder does not exist. You may have installed the project incorrectly, or dragged the Editor folder into a subfolder.");
                Debug.LogWarning("If you experience problems with exporting, make sure your Editor folder is in the right place. If it still doesn't work, reinstall this unity project and make sure to follow the install instructions.");
                Debug.LogWarning("Trying to export anyways...");
                AssetDatabase.CreateFolder("Assets", "Editor");
            }
            EditorSceneManager.SaveScene(gameObject.scene, assetBundleScenePath, true);

            EditorSceneManager.OpenScene(assetBundleScenePath);

            // Destroy other maps that exist
            MapDescriptor[] descriptorList = Object.FindObjectsOfType<MapDescriptor>();
            foreach (MapDescriptor descriptor in descriptorList)
            {
                if (descriptor.MapName != mapDescriptor.MapName)
                {
                    Object.DestroyImmediate(descriptor.gameObject);
                }
                else
                {
                    mapDescriptor = descriptor;
                    gameObject = descriptor.gameObject;
                    Selection.activeObject = gameObject;
                }
            }

            // Move objects that aren't in the map parent to the map parent
            foreach (GameObject sceneRootObject in EditorSceneManager.GetActiveScene().GetRootGameObjects())
            {
                if (sceneRootObject != gameObject)
                {
                    sceneRootObject.transform.SetParent(gameObject.transform);
                }
            }

            // Unpack all prefabs
            foreach (GameObject subObject in GameObject.FindObjectsOfType<GameObject>())
            {
                if (PrefabUtility.GetPrefabInstanceStatus(subObject) != PrefabInstanceStatus.NotAPrefab)
                {
                    PrefabUtility.UnpackPrefabInstance(PrefabUtility.GetOutermostPrefabInstanceRoot(subObject), PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                }
            }

            // Skybox Stuff

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
                Material skyboxMaterial = new Material(Shader.Find("Bobbie/Outer"));
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

            // Remove meshes from prefabs/triggers
            if (!DebugPrefabs)
            {
                StripMeshes<ObjectTrigger>(gameObject);
                StripMeshes<Teleporter>(gameObject);
                StripMeshes<TagZone>(gameObject);
                foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>()) if (renderer.sharedMaterial != null && renderer.sharedMaterial.name.StartsWith("Teleport Point")) Object.DestroyImmediate(renderer);
                StripMeshes<ObjectTrigger>(gameObject);
            }

            foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
            {
                if (renderer.sharedMaterial != null && renderer.sharedMaterial.name.StartsWith("Spawn Point"))
                {
                    bool foundTransform = false;
                    foreach (Transform spawnPoint in mapDescriptor.SpawnPoints)
                    {
                        if (spawnPoint == renderer.gameObject.transform) foundTransform = true;
                    }
                    if (!foundTransform)
                    {
                        List<Transform> spawnPointArray = new List<Transform>(mapDescriptor.SpawnPoints);
                        spawnPointArray.Add(renderer.gameObject.transform);
                        mapDescriptor.SpawnPoints = spawnPointArray.ToArray();
                    }
                    if (!DebugPrefabs)
                    {
                        Collider spawnPointCollider = renderer.gameObject.GetComponent<Collider>();
                        if (spawnPointCollider) Object.DestroyImmediate(spawnPointCollider);
                        Object.DestroyImmediate(renderer);
                    }
                }
            }

            if (mapDescriptor.SpawnPoints.Length == 0) throw new System.Exception("No spawn points found! Add some spawn points to your map.");

            // Run Custom OnBuilds
            foreach (IBuildEvents component in gameObject.GetComponents<IBuildEvents>())
			{
                component.OnBuild(mapDescriptor);
			}

            // Compute Required Versions
            System.Version pcRequiredVersion = ComputePcVersion(mapDescriptor);
            System.Version androidRequiredVersion = ComputeAndroidVersion(mapDescriptor);

            MapPackageInfo packageJSON = MapDescriptorToJSON(mapDescriptor);

            // Take Screenshots with the thumbnail camera
            Camera thumbnailCamera = GameObject.Find("ThumbnailCamera")?.GetComponent<Camera>();
            if (thumbnailCamera != null)
            {
                // Normal Screenshot
                Texture2D screenshot = CaptureScreenshot(thumbnailCamera, 512, 512);
                byte[] screenshotPNG = ImageConversion.EncodeToPNG(screenshot);
                File.WriteAllBytes(Application.temporaryCachePath + "/preview.png", screenshotPNG);
                packageJSON.Config.ImagePath = "preview.png";

                // Cubemap Screenshot
                Texture2D screenshotCubemap = CaptureCubemap(thumbnailCamera, 1024, 1024);
                byte[] screenshotCubemapPNG = ImageConversion.EncodeToPNG(screenshotCubemap);
                File.WriteAllBytes(Application.temporaryCachePath + "/preview_cubemap.png", screenshotCubemapPNG);
                packageJSON.Config.CubemapImagePath = "preview_cubemap.png";

                packageJSON.Config.MapColor = AverageColor(screenshotCubemap);
            }
            else throw new System.Exception("ThumbnailCamera is missing! Make sure to add a ThumbnailCamera to your map.");
            Object.DestroyImmediate(thumbnailCamera.gameObject);

            // Pre-Process stuff for both platforms - PC and Android.
            GameObject spawnPointContainer = new GameObject("SpawnPointContainer");
            spawnPointContainer.transform.SetParent(gameObject.transform);
            spawnPointContainer.transform.localPosition = Vector3.zero;
            spawnPointContainer.transform.localRotation = Quaternion.identity;
            spawnPointContainer.transform.localScale = Vector3.one;

            List<string> spawnPointNames = new List<string>();

            for (int i = 0; i < mapDescriptor.SpawnPoints.Length; i++)
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
            packageJSON.Config.SpawnPoints = spawnPointNames.ToArray();


            // Replace tiling materials with baked versions
            foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
            {
                if (renderer.sharedMaterial != null && renderer.sharedMaterial.shader.name.Contains("Standard Tiling") && !renderer.sharedMaterial.shader.name.Contains("Baked"))
                {
                    renderer.sharedMaterial = Object.Instantiate(renderer.sharedMaterial);
                    renderer.sharedMaterial.shader = Shader.Find("Standard Tiling Baked");
                    Transform rendererTransform = renderer.gameObject.transform;
                    Vector3 newVector = RotatePointAroundPivot(rendererTransform.lossyScale, new Vector3(0, 0, 0), rendererTransform.rotation.eulerAngles);
                    renderer.sharedMaterial.SetVector("_ScaleVector", newVector);
                }
            }

            // Destroy all non-render cameras because people keep accidentally exporting them
            foreach (Camera camera in gameObject.GetComponentsInChildren<Camera>())
            {
                if (camera.targetTexture == null && camera.gameObject != null) Object.DestroyImmediate(camera.gameObject);
            }

            //Destroy Audio listeners too since they can break sound
            foreach (AudioListener listener in gameObject.GetComponentsInChildren<AudioListener>())
            {
                if (listener != null) Object.DestroyImmediate(listener);
            }

            // Lighting stuff. Make sure to set light stuff up and make it bigger, and bake
            if (mapDescriptor.ExportLighting)
            {
                foreach (Light light in Object.FindObjectsOfType<Light>())
                {
                    // needs to be redone, colors are inconsistent and can probably be made better
                    if (light.type == LightType.Directional)
                    {
                        light.intensity *= 11.54f;
                        light.color = new Color(0.3215f, 0.3215f, 0.3215f);
                    }
                    else
                    {
                        light.intensity *= 10f;
                    }
                }

                bool baking = Lightmapping.BakeAsync();
                if (baking)
                {
                    Lightmapping.bakeCompleted -= PostBake;
                    Lightmapping.bakeCompleted += PostBake;
                }
                else throw new System.Exception("Couldn't start lightmapping.");
            }
            else
            {
                foreach (Light light in Object.FindObjectsOfType<Light>()) Object.DestroyImmediate(light);
                Lightmapping.Clear();
                Lightmapping.ClearLightingDataAsset();
                PostBake();
            }

            void PostBake(){
                gameObject.transform.localPosition = new Vector3(0, 5000, 0);

                // Save as prefab and build
                // PrefabUtility.SaveAsPrefabAsset(Selection.activeObject as GameObject, $"Assets/_{typeName}.prefab");
                EditorSceneManager.SaveScene(gameObject.scene);
                AssetBundleBuild assetBundleBuild = default;
                assetBundleBuild.assetNames = new string[] { assetBundleScenePath };
                assetBundleBuild.assetBundleName = pcFileName;

                // Build for PC

                BuildPipeline.BuildAssetBundles(Application.temporaryCachePath, new AssetBundleBuild[] { assetBundleBuild }, 0, BuildTarget.StandaloneWindows64);

                // Do Android specific stuff here. Stripping MonoBehaviours and converting them to TextAssets, etc.

                // first we need to redo this bit of code because sometimes gameObject unreferences itself
                MapDescriptor[] descriptorList2 = Object.FindObjectsOfType<MapDescriptor>();
                foreach (MapDescriptor descriptor in descriptorList2)
                {
                    if (descriptor.MapName != mapDescriptor.MapName)
                    {
                        Object.DestroyImmediate(descriptor.gameObject);
                    }
                    else
                    {
                        mapDescriptor = descriptor;
                        gameObject = descriptor.gameObject;
                        Selection.activeObject = gameObject;
                    }
                }

                StripPrefabsForQuest(gameObject);

                Object.DestroyImmediate(mapDescriptor);

                // Do it again for Android
                EditorSceneManager.SaveScene(gameObject.scene);
                // PrefabUtility.SaveAsPrefabAsset(Selection.activeObject as GameObject, $"Assets/_{typeName}.prefab"); // are these next 2 lines necessary? idk. probably test it.
                assetBundleBuild.assetNames = new string[] { assetBundleScenePath };
                assetBundleBuild.assetBundleName = androidFileName;
                BuildPipeline.BuildAssetBundles(Application.temporaryCachePath, new AssetBundleBuild[] { assetBundleBuild }, 0, BuildTarget.Android);

                EditorPrefs.SetString("currentBuildingAssetBundlePath", folderPath);

                // JSON stuff
                packageJSON.AndroidFileName = androidFileName;
                packageJSON.PcFileName = pcFileName;
                packageJSON.Descriptor.PcRequiredVersion = pcRequiredVersion.ToString();
                packageJSON.Descriptor.AndroidRequiredVersion = androidRequiredVersion.ToString();
                string json = JsonConvert.SerializeObject(packageJSON, Formatting.Indented, new JsonSerializerSettings() {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Converters = new List<JsonConverter>() { new ColorConverter() }
                });
                File.WriteAllText(Application.temporaryCachePath + "/package.json", json);
                // AssetDatabase.DeleteAsset($"Assets/_{typeName}.prefab");

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
                foreach (string file in files) if (File.Exists(file)) File.Delete(file);

                // Move the ZIP and finalize
                File.Move(Application.temporaryCachePath + "/tempZip.zip", path);
                //Object.DestroyImmediate(gameObject);
                AssetDatabase.Refresh();

                // Open scene again
                EditorSceneManager.OpenScene(oldScenePath);

                // Re-enable scene checking
                SceneChecker.disabled = false;
                exporting = false;

                EditorUtility.DisplayDialog("Exportation Successful!", "Exportation Successful!", "OK");
                EditorUtility.RevealInFinder(path);
            }
        }
        catch(System.Exception e)
        {
            exporting = false;
            SceneChecker.disabled = false;
            Debug.Log("Something went wrong... let's load the old scene.");
            EditorUtility.DisplayDialog("Error!", e.Message, "OK");
            if (oldScenePath != null)
            {
                EditorSceneManager.OpenScene(oldScenePath);
            }
            else throw new System.Exception("Something went wrong and you don't have your work saved in a scene! PLEASE save your work in a scene before trying to export.");
            throw e;
        }
    }

    public static void StripPrefabsForQuest(GameObject gameObject)
    {

        foreach (TagZone zone in gameObject.GetComponentsInChildren<TagZone>(true))
        {
            if (zone != null && zone.gameObject != null)
            {
                CreateQuestText("{\"TagZone\": true}", zone.gameObject);
                Object.DestroyImmediate(zone);
            }
        }

        foreach (SurfaceClimbSettings surfaceClimbSettings in gameObject.GetComponentsInChildren<SurfaceClimbSettings>(true))
        {
            if (surfaceClimbSettings != null && surfaceClimbSettings.gameObject != null)
            {
                SurfaceClimbSettingsJSON settingsJson = new SurfaceClimbSettingsJSON();
                settingsJson.Unclimbable = surfaceClimbSettings.Unclimbable;
                settingsJson.slipPercentage = surfaceClimbSettings.slipPercentage;

                CreateQuestText(JsonUtility.ToJson(settingsJson), surfaceClimbSettings.gameObject);
                Object.DestroyImmediate(surfaceClimbSettings);
            }
        }

        int triggerCount = 1;
        foreach (ObjectTrigger objectTrigger in gameObject.GetComponentsInChildren<ObjectTrigger>(true))
        {
            if (objectTrigger != null && objectTrigger.gameObject != null)
            {
                string objectName = "ObjectTrigger" + triggerCount;
                if (objectTrigger.ObjectToTrigger != null)
                {
                    CreateQuestText("{\"TriggeredBy\": \"" + objectName + "\"}", objectTrigger.ObjectToTrigger);
                }
                ObjectTriggerJSON triggerJSON = new ObjectTriggerJSON();
                triggerJSON.ObjectTriggerName = objectName;
                triggerJSON.OnlyTriggerOnce = objectTrigger.OnlyTriggerOnce;
                triggerJSON.DisableObject = objectTrigger.DisableObject;

                CreateQuestText(JsonUtility.ToJson(triggerJSON), objectTrigger.gameObject);
                Object.DestroyImmediate(objectTrigger);
                triggerCount++;
            }
        }

        int teleporterCount = 1;
        foreach (Teleporter teleporter in gameObject.GetComponentsInChildren<Teleporter>(true))
        {
            if (teleporter != null && teleporter.gameObject != null)
            {
                string teleporterName = "Teleporter" + teleporterCount;
                foreach (Transform teleportPoint in teleporter.TeleportPoints)
                {
                    if (teleportPoint != null && teleportPoint.gameObject != null)
                    {
                        CreateQuestText("{\"TeleportPoint\": \"" + teleporterName + "\"}", teleportPoint.gameObject);
                    }
                }
                teleporter.TeleportPoints = null;
                string teleporterJSON = JsonUtility.ToJson(teleporter);
                teleporterJSON = teleporterJSON.Replace("\"TeleportPoints\":[],", "\"TeleporterName\": \"" + teleporterName + "\",");

                CreateQuestText(teleporterJSON, teleporter.gameObject);
                Object.DestroyImmediate(teleporter);
                teleporterCount++;
            }
        }

        RoundEndActions roundEndActions = gameObject.GetComponentInChildren<RoundEndActions>(true);
        if (roundEndActions != null && roundEndActions.gameObject != null)
        {
            foreach (GameObject roundEndActionObject in roundEndActions.ObjectsToEnable)
            {
                if (roundEndActionObject != null)
                {
                    CreateQuestText("{\"RoundEndAction\": \"Enable\"}", roundEndActionObject);
                }
            }
            foreach (GameObject roundEndActionObject in roundEndActions.ObjectsToDisable)
            {
                if (roundEndActionObject != null)
                {
                    CreateQuestText("{\"RoundEndAction\": \"Disable\"}", roundEndActionObject);
                }
            }
            RoundEndActionsJSON actionsJSON = new RoundEndActionsJSON();
            actionsJSON.RoundEndActions = true;
            actionsJSON.RespawnOnRoundEnd = roundEndActions.RespawnOnRoundEnd;

            CreateQuestText(JsonUtility.ToJson(actionsJSON), roundEndActions.gameObject);
            Object.DestroyImmediate(roundEndActions);
        }
    }

    public static void StripMeshes<T>(GameObject container)
    {
        foreach(T instance in container.GetComponentsInChildren<T>())
        {
            Component component = instance as Component;
            if (component.gameObject.GetComponent<Renderer>() != null) Object.DestroyImmediate(component.gameObject.GetComponent<Renderer>());
            if (component.gameObject.GetComponent<MeshFilter>() != null) Object.DestroyImmediate(component.gameObject.GetComponent<MeshFilter>());
        }
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
        cam = UnityEngine.Object.Instantiate(cam.gameObject, cam.transform.parent).GetComponent<Camera>();
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
        cam = UnityEngine.Object.Instantiate(cam.gameObject, cam.transform.parent).GetComponent<Camera>();
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

    public static System.Version ComputePcVersion(MapDescriptor mapDescriptor)
	{
        System.Version requiredVersion = new System.Version("1.0.0");
        System.Version casualUpdate = new System.Version("1.1.0");
        if (mapDescriptor.SlowJumpLimit != SharedConstants.SlowJumpLimit
            || mapDescriptor.SlowJumpMultiplier != SharedConstants.SlowJumpMultiplier
            || mapDescriptor.FastJumpLimit != SharedConstants.FastJumpLimit
            || mapDescriptor.FastJumpMultiplier != SharedConstants.FastJumpMultiplier
            || (mapDescriptor.GameMode != "" && mapDescriptor.GameMode.ToLower() != "default" )
            || (mapDescriptor.RequiredPCModsId != null & mapDescriptor.RequiredPCModsId.Count > 0)
            || (mapDescriptor.RequiredQuestModsId != null & mapDescriptor.RequiredQuestModsId.Count > 0) )
		{
            requiredVersion = MaxVersion(requiredVersion, casualUpdate);
		}
        return requiredVersion;
	}

    public static System.Version ComputeAndroidVersion(MapDescriptor mapDescriptor)
	{
        System.Version requiredVersion = new System.Version("1.0.0");
        System.Version casualUpdate = new System.Version("1.2.0");
        if (mapDescriptor.SlowJumpLimit != SharedConstants.SlowJumpLimit
            || mapDescriptor.SlowJumpMultiplier != SharedConstants.SlowJumpMultiplier
            || mapDescriptor.FastJumpLimit != SharedConstants.FastJumpLimit
            || mapDescriptor.FastJumpMultiplier != SharedConstants.FastJumpMultiplier
            || (mapDescriptor.GameMode != "" && mapDescriptor.GameMode.ToLower() != "default" )
            || (mapDescriptor.RequiredPCModsId != null & mapDescriptor.RequiredPCModsId.Count > 0)
            || (mapDescriptor.RequiredQuestModsId != null & mapDescriptor.RequiredQuestModsId.Count > 0) )
		{
            requiredVersion = MaxVersion(requiredVersion, casualUpdate);
		}
        return requiredVersion;
	}

    public static Color AverageColor(Texture2D tex)
    {
        Color[] colors = tex.GetPixels();
        Color averaged = new Color(0, 0, 0);
        foreach (Color color in colors) averaged += color;
        averaged /= colors.Length;
        averaged.a = 1;
        return averaged;
    }

    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }

    public static System.Version MaxVersion(System.Version a, System.Version b)
	{
        return (a.CompareTo(b) > 0) ? a : b;
	}
}