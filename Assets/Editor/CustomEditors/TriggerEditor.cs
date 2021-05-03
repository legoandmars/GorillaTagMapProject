using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VmodMonkeMapLoader.Behaviours.ObjectTrigger))]
public class TriggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }

    public void OnSceneGUI()
    {
        VmodMonkeMapLoader.Behaviours.ObjectTrigger trigger = (VmodMonkeMapLoader.Behaviours.ObjectTrigger)target;

        DrawTrigger(trigger, true);
    }

    public static void DrawTrigger(VmodMonkeMapLoader.Behaviours.ObjectTrigger trigger, bool single = false)
    {
        if (single)
        {
            Handles.BeginGUI();
            GUILayout.BeginVertical(MapDescriptorConfig.textStyle, GUILayout.Width(MapDescriptorConfig.guiWidth * 1.5f));

            GUILayout.Label(trigger.gameObject.name, EditorStyles.boldLabel);

            trigger.DisableObject = EditorGUILayout.Toggle("Disable Object", trigger.DisableObject);
            trigger.OnlyTriggerOnce = EditorGUILayout.Toggle("Only Trigger Once", trigger.OnlyTriggerOnce);

            trigger.ObjectToTrigger = EditorGUILayout.ObjectField("Object to Trigger", trigger.ObjectToTrigger, typeof(GameObject), true) as GameObject;
            if (trigger.ObjectToTrigger != null)
            {
                if (trigger.ObjectToTrigger.scene != trigger.gameObject.scene)
                {
                    GUILayout.Label("Your Triggered Object is not in the same scene as your trigger!", EditorStyles.boldLabel);
                }
            }
            trigger.TouchType = (VmodMonkeMapLoader.Behaviours.GorillaTouchType)EditorGUILayout.EnumPopup("TouchType", trigger.TouchType);

            trigger.Delay = EditorGUILayout.FloatField("Delay", trigger.Delay);

            GUILayout.EndVertical();
            Handles.EndGUI();
        }
        if (trigger.ObjectToTrigger != null)
        {
            if (trigger.DisableObject)
            {
                Handles.color = (Color.white + Color.red) / 2;
            }
            else
            {
                Handles.color = Color.Lerp(Color.green, Color.cyan, 0.5f);
            }

            if (Handles.Button(trigger.ObjectToTrigger.transform.position, trigger.ObjectToTrigger.transform.rotation, 0.3f, 0.3f, Handles.SphereHandleCap))
            {
                MapDescriptorEditor.moveTo(trigger.ObjectToTrigger.transform);
            }
            

            if (trigger.OnlyTriggerOnce)
            {
                DrawOnceTrigger(trigger, single);
            }
            else
            {
                DrawMultiTrigger(trigger, single);
            }
            HandleHelpers.ResizeLabel(trigger.ObjectToTrigger.transform.position + Vector3.up, trigger.ObjectToTrigger.gameObject.name);

            Handles.color = Color.cyan;
        }
        else
        {
            Handles.color = Color.red;
        }

        if (single)
        {
            Handles.SphereHandleCap(0, trigger.transform.position, trigger.transform.rotation, 0.4f, EventType.Repaint);
        }
        else
        {
            HandleHelpers.ResizeLabel(trigger.transform.position + Vector3.up, trigger.gameObject.name);
            if (Handles.Button(trigger.transform.position, trigger.transform.rotation, 0.4f, 0.4f, Handles.SphereHandleCap))
            {
                MapDescriptorEditor.moveTo(trigger.transform);
            }
        }
    }

    public static void DrawOnceTrigger(VmodMonkeMapLoader.Behaviours.ObjectTrigger trigger, bool single = false)
    {
        if (single)
        {
            HandleHelpers.DrawArrow(trigger.transform.position, trigger.ObjectToTrigger.transform.position, 1.5f);
        }
        else
        {
            HandleHelpers.DrawLine(trigger.transform.position, trigger.ObjectToTrigger.transform.position, 1.5f);
        }

    }

    public static void DrawMultiTrigger(VmodMonkeMapLoader.Behaviours.ObjectTrigger trigger, bool single = false)
    {
        if (single)
        {
            HandleHelpers.DrawArrow(trigger.transform.position, trigger.ObjectToTrigger.transform.position, 2.0f);
        }
        else
        {
            HandleHelpers.DrawLine(trigger.transform.position, trigger.ObjectToTrigger.transform.position, 4.0f);
        }
    }
}
