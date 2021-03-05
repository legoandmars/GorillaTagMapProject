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
    /*
    public bool customColors;
    public bool disableInPublicLobbies;
    */
}
