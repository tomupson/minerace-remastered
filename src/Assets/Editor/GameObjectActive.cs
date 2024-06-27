using UnityEngine;
using UnityEditor;

public class GameObjectActive : EditorWindow
{
    [MenuItem("Window/GameObject State Editor")]
    public static void ShowWindow()
    {
        GetWindow<GameObjectActive>("GameObject State Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("SELECT DESIRED GAMEOBJECTS");

        if (GUILayout.Button("HIDE OBJECTS"))
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                obj.SetActive(false);
            }
        }

        if (GUILayout.Button("SHOW OBJECTS"))
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                obj.SetActive(true);
            }
        }
    }
}
