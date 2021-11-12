using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TagAttribute))]
public class TagDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.LabelField(position, "The property has to be a tag for LayerAttribute to work!");
            return;
        }

        if (System.String.IsNullOrEmpty(property.stringValue))
        {
            property.stringValue = "Untagged";
        }

        property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
    }
}