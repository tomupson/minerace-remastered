using UnityEditor;

[CustomEditor(typeof(EffectItem), editorForChildClasses: true)]
public class EffectItemEditor : Editor
{
    private SerializedProperty permanent;
    private SerializedProperty duration;

    private void OnEnable()
    {
        permanent = serializedObject.FindProperty(nameof(EffectItem.permanent));
        duration = serializedObject.FindProperty(nameof(EffectItem.duration));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty property = serializedObject.GetIterator();
        property.NextVisible(enterChildren: true);

        while (property.NextVisible(enterChildren: true))
        {
            if (!SerializedProperty.EqualContents(property, duration) || !permanent.boolValue)
            {
                EditorGUILayout.PropertyField(property, includeChildren: true);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
