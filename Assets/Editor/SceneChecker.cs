using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class SceneChecker
{
    public static bool disabled = false;

    static SceneChecker()
    {
        EditorSceneManager.activeSceneChangedInEditMode += EditorSceneManager_activeSceneChanged;
    }

    private static void EditorSceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        if(!disabled && arg1 != null && arg1.name == "ExportScene")
        {
            EditorUtility.DisplayDialog("Error!", "You are currently in the Export Scene.\nThe Export Scene is used internally for exporting and should not be used.\n\nPlease go back to your actual scene unless you know what you're doing.", "OK");
        }
    }
}
