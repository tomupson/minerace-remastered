using UnityEngine;
using UnityEditor;

public class GameObjectActive : EditorWindow
{
    [MenuItem("Custom/GameObject State Editor")]
    public static void ShowWindow()
    {
        GameObjectActive window = GetWindow<GameObjectActive>("GameObject State Editor");
        window.position = new Rect(window.position.position, new Vector2(window.position.size.x, 0));
    }

    private void OnGUI()
    {
        if (GUILayout.Button("HIDE"))
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                obj.SetActive(false);
            }
        }

        if (GUILayout.Button("SHOW"))
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                obj.SetActive(true);
            }
        }
    }
}
