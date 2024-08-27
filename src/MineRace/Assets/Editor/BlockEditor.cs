using System.Linq;
using UnityEditor;

[CustomEditor(typeof(Block))]
public class BlockEditor : Editor
{
    private SerializedProperty blockType;
    private SerializedProperty[] resourceOnlyProperties;

    private void OnEnable()
    {
        blockType = serializedObject.FindProperty(nameof(Block.blockType));
        resourceOnlyProperties = new[]
        {
            serializedObject.FindProperty(nameof(Block.rarity)),
            serializedObject.FindProperty(nameof(Block.pointsValue)),
            serializedObject.FindProperty(nameof(Block.basePercentage)),
            serializedObject.FindProperty(nameof(Block.percentageMultiplier))
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty property = serializedObject.GetIterator();
        property.NextVisible(enterChildren: true);

        while (property.NextVisible(enterChildren: true))
        {
            if (!resourceOnlyProperties.Any(prop => SerializedProperty.EqualContents(prop, property)) || blockType.intValue == (int)BlockType.Resource)
            {
                EditorGUILayout.PropertyField(property, includeChildren: true);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
