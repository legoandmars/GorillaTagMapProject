using UnityEngine;
using UnityEditor;

static class MapDescriptorConfig
{
    static public GUIStyle textStyle = new GUIStyle()
    {
        richText = true,
        font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Editor/CustomEditors/thintel.ttf"),
        fontStyle = FontStyle.Normal,
        fontSize = 20,
        alignment = TextAnchor.MiddleLeft,
        normal = new GUIStyleState()
        {
            background = Texture2D.whiteTexture
        }
    };

    static public GameObject TeleportPoint = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/MapPrefabs/TeleportPoint.prefab");

    static public bool DisplayTeleporters = false;
    static public bool DisplaySpawnPoints = false;
    static public bool DisplayTagZones = false;
    static public bool DisplayObjectTriggers = false;
    static public bool DisplaySurfaceSettings = false;
    static public bool DisplayRoundEndActions = false;
    static public bool DisplayNames = false;

    static public int guiWidth = 200;
}