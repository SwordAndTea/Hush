using UnityEditor;
using UnityEngine;

namespace AIEnemy.Spider
{
    [CustomEditor(typeof(SpiderEnemy))]
    public class SpiderEnemyEditor : Editor
    {
        Vector2 _testWebWorldTarget;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField("Play mode tests", EditorStyles.boldLabel);

            var spider = (SpiderEnemy)target;

            EditorGUI.BeginDisabledGroup(!Application.isPlaying);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Start patrol"))
                spider.StartPatrol();
            if (GUILayout.Button("Stop patrol"))
                spider.StopPatrol();
            EditorGUILayout.EndHorizontal();

            _testWebWorldTarget = EditorGUILayout.Vector2Field("Test web target (world)", _testWebWorldTarget);
            if (GUILayout.Button("Shoot web"))
                spider.ShootWeb(_testWebWorldTarget);

            EditorGUI.EndDisabledGroup();

            if (!Application.isPlaying)
                EditorGUILayout.HelpBox("Enter Play Mode to use the test buttons.", MessageType.Info);
        }
    }
}
