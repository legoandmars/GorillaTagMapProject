using VmodMonkeMapLoader.Helpers;

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
    public string androidRequiredVersion;
    public string pcRequiredVersion;
}

[System.Serializable]
public class Config
{
    public string imagePath;
    public string cubemapImagePath;
    public string[] spawnPoints;
    public UnityEngine.Color mapColor;
    public float gravity = SharedConstants.Gravity;
    public float slowJumpLimit = SharedConstants.SlowJumpLimit;
    public float fastJumpLimit = SharedConstants.FastJumpLimit;
    public float slowJumpMultiplier = SharedConstants.SlowJumpMultiplier;
    public float fastJumpMultiplier = SharedConstants.FastJumpMultiplier;
    public string gameMode = "";
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
