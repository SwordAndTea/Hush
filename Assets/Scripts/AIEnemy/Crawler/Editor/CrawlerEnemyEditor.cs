using UnityEditor;
using UnityEngine;

namespace AIEnemy.Crawler
{
    [CustomEditor(typeof(CrawlerEnemy))]
    public class CrawlerEnemyEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField("Play mode tests", EditorStyles.boldLabel);

            var crawler = (CrawlerEnemy)target;

            EditorGUI.BeginDisabledGroup(!Application.isPlaying);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Start patrol"))
                crawler.StartPatrol();
            if (GUILayout.Button("Stop patrol"))
                crawler.StopPatrol();
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Grab"))
                crawler.Grab();

            EditorGUI.EndDisabledGroup();

            if (!Application.isPlaying)
                EditorGUILayout.HelpBox("Enter Play Mode to use the test buttons.", MessageType.Info);
        }
    }
}
