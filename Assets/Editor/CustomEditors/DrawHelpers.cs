using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class HandleHelpers
{
    public static void DrawArrow(Vector3 start, Vector3 end, float size = 1.0f)
    {
        if (start == end || (start - end).magnitude < .2f) return;
        Vector3 dir = end - start;
        dir.Normalize();
        DrawLine(start, end, size * 2);

        Quaternion arrowDir = Quaternion.LookRotation(dir);
        Handles.ArrowHandleCap(0, end - (dir * size * 1.15f), arrowDir, size, EventType.Repaint);
    }
    public static void DrawLine(Vector3 start, Vector3 end, float width = 1.0f)
    {
        if (start == end || (start - end).magnitude < .2f) return;
        Handles.DrawBezier(start, end, start, end, Handles.color, Texture2D.whiteTexture, width);
    }

    public static void ResizeLabel (Vector3 position, GUIContent content, bool displayNames = false)
    {
        if (!displayNames && !MapDescriptorConfig.DisplayNames) return; 

        GUIStyle resizeStyle = new GUIStyle(MapDescriptorConfig.textStyle);
        int newSize = (int)((float)resizeStyle.fontSize / HandleUtility.GetHandleSize(position)) * 2;
        resizeStyle.fontSize = Mathf.Min(resizeStyle.fontSize, newSize);

        Handles.Label(position, content, resizeStyle);
    }

    public static void ResizeLabel(Vector3 position, string content, bool displayNames = false)
    {
        ResizeLabel(position, new GUIContent(content), displayNames);
    }
    public static void Label(Vector3 position, GUIContent content, bool displayNames = false)
    {
        if (!displayNames && !MapDescriptorConfig.DisplayNames) return; 
        Handles.Label(position, content, MapDescriptorConfig.textStyle);
    }

    public static void Label(Vector3 position, string content, bool displayNames = false)
    {
        Label(position, new GUIContent(content), displayNames);
    }

}
