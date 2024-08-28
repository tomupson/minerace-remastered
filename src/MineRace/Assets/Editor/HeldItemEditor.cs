using System.Linq;
using UnityEditor;

[CustomEditor(typeof(HeldItem), editorForChildClasses: true)]
public class HeldItemEditor : Editor
{
    private SerializedProperty stackable;
    private SerializedProperty[] stackableProperties;

    private void OnEnable()
    {
        stackable = serializedObject.FindProperty(nameof(HeldItem.stackable));
        stackableProperties = new[]
        {
            serializedObject.FindProperty(nameof(HeldItem.stackSize)),
            serializedObject.FindProperty(nameof(HeldItem.maxStackSize))
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty property = serializedObject.GetIterator();
        property.NextVisible(enterChildren: true);

        while (property.NextVisible(enterChildren: true))
        {
            if (!stackableProperties.Any(prop => SerializedProperty.EqualContents(prop, property)) || stackable.boolValue)
            {
                EditorGUILayout.PropertyField(property, includeChildren: true);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
