using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TopDownCharacterControl))]
public class TopDownCharacterControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(10f);
        EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);

        TopDownCharacterControl controller = (TopDownCharacterControl)target;

        if (GUILayout.Button("Trigger Fail Event"))
        {
            controller.TriggerFailEvent();
            EditorUtility.SetDirty(controller);
        }

        if (GUILayout.Button("Trigger Win Event"))
        {
            controller.TriggerWinEvent();
            EditorUtility.SetDirty(controller);
        }
    }
}
