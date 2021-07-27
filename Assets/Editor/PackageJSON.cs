using VmodMonkeMapLoader.Helpers;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class SurfaceClimbSettingsJSON
{
    public bool SurfaceClimbSettings = true;
    public bool Unclimbable = false;
    public float slipPercentage = 0.03f;
}

[Serializable]
public class ObjectTriggerJSON
{
    public string ObjectTriggerName;
    public bool OnlyTriggerOnce = false;
    public bool DisableObject = false;
}

[Serializable]
public class RoundEndActionsJSON
{
    public bool RoundEndActions;
    public bool RespawnOnRoundEnd = false;
}

