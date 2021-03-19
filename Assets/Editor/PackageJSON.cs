[System.Serializable]
public class PackageJSON
{
    public string androidFileName;
    public string pcFileName;
    public Descriptor descriptor;
    public Config config;
}

[System.Serializable]
public class Descriptor
{
    public string objectName;
    public string author;
    public string description;
}

[System.Serializable]
public class Config
{
    public string imagePath;
    public string cubemapImagePath;
    public string[] spawnPoints;
    public UnityEngine.Color mapColor;
    public float gravity = -9.8f;
}

[System.Serializable]
public class SurfaceClimbSettingsJSON
{
    public bool SurfaceClimbSettings = true;
    public bool Unclimbable = false;
    public float slipPercentage = 0.03f;
}

[System.Serializable]
public class ObjectTriggerJSON
{
    public string ObjectTriggerName;
    public bool OnlyTriggerOnce = false;
    public bool DisableObject = false;
}

[System.Serializable]
public class RoundEndActionsJSON
{
    public bool RoundEndActions;
    public bool RespawnOnRoundEnd = false;
}
