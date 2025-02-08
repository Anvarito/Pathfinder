using TacticalBattle;
using UnityEditor;
using UnityEngine;

namespace MyEditor
{
    [CustomEditor(typeof(PathNode))]
    public class PathNodeEditor : Editor
    {
        SerializedProperty wallPrefab;  // Ссылка на поле префаба

        private void OnEnable()
        {
            // Получаем доступ к полю префаба в скрипте MyScript
            wallPrefab = serializedObject.FindProperty("WallPrefab");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(wallPrefab);
            DrawDefaultInspector();
            if (GUILayout.Button("Создать стену"))
            {
                CreateChildObject();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void CreateChildObject()
        {
            PathNode myScript = (PathNode)target;

            if (myScript.WallPrefab != null)
            {
                Wall neWall = Instantiate(myScript.WallPrefab, myScript.transform, true);
                myScript.AddWall(neWall);
            }
            else
            {
                Debug.LogWarning("Префаб не выбран!");
            }
        }
    }
}
