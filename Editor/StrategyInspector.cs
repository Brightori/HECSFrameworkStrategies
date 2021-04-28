using HECSFrameWork;
using UnityEditor;
using UnityEngine;

namespace HECSFrameWork.Helpers
{
    [CustomEditor(typeof(Strategy))]
    public class StrategyInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Открыть редактирование стратегии", GUILayout.Height(150)))
            {
                var window = (StrategyGraphViewWIndow)EditorWindow.GetWindow(typeof(StrategyGraphViewWIndow));
                var path = AssetDatabase.GetAssetPath(target);

                window.titleContent = new GUIContent((target as Strategy).name);

                window.Show();
                window.OnInit(target as Strategy, path);
                
                Debug.Log("full path " + path);
            }

            base.OnInspectorGUI();
        }
    }
}

