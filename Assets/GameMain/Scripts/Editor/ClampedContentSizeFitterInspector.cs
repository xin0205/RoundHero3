using UnityEditor;
using UnityEngine;

namespace RoundHero.Editor
{
    [CustomEditor(typeof(ClampedContentSizeFitter))]
    public class ClampedContentSizeFitterInspector : UnityEditor.UI.ContentSizeFitterEditor
    {
        SerializedProperty m_HorizontalFit;
        SerializedProperty m_VerticalFit;
        SerializedProperty m_MaxWidth;
        SerializedProperty m_MaxHeight;
 
        protected override void OnEnable()
        {
            base.OnEnable();
            m_HorizontalFit = serializedObject.FindProperty("m_HorizontalFit");
            m_VerticalFit = serializedObject.FindProperty("m_VerticalFit");
            m_MaxWidth = serializedObject.FindProperty("m_MaxWidth");
            m_MaxHeight = serializedObject.FindProperty("m_MaxHeight");
        }
 
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            if (m_HorizontalFit.enumValueIndex != 0)
                LayoutElementField(m_MaxWidth, 0, new GUIContent("最大宽度"));
            if (m_VerticalFit.enumValueIndex != 0)
                LayoutElementField(m_MaxHeight, 0, new GUIContent("最大高度"));
            serializedObject.ApplyModifiedProperties();
        }
 
        void LayoutElementField(SerializedProperty property, float defaultValue, GUIContent rawLabel)
        {
            LayoutElementField(property, _ => defaultValue, rawLabel);
        }
 
        void LayoutElementField(SerializedProperty property, System.Func<RectTransform, float> defaultValue, GUIContent rawLabel)
        {
            Rect position = EditorGUILayout.GetControlRect();
 
            // Label
            GUIContent label = EditorGUI.BeginProperty(position, rawLabel, property);
 
            // Rects
            Rect fieldPosition = EditorGUI.PrefixLabel(position, label);
 
            Rect toggleRect = fieldPosition;
            toggleRect.width = 16;
 
            Rect floatFieldRect = fieldPosition;
            floatFieldRect.xMin += 16;
 
            // Checkbox
            EditorGUI.BeginChangeCheck();
            bool enabled = EditorGUI.ToggleLeft(toggleRect, GUIContent.none, property.floatValue >= 0);
            if (EditorGUI.EndChangeCheck())
            {
                // This could be made better to set all of the targets to their initial width, but mimizing code change for now
                property.floatValue = (enabled ? defaultValue((target as ClampedContentSizeFitter).transform as RectTransform) : -1);
            }
 
            if (!property.hasMultipleDifferentValues && property.floatValue >= 0)
            {
                // Float field
                EditorGUIUtility.labelWidth = 4; // Small invisible label area for drag zone functionality
                EditorGUI.BeginChangeCheck();
                float newValue = EditorGUI.FloatField(floatFieldRect, new GUIContent(" "), property.floatValue);
                if (EditorGUI.EndChangeCheck())
                {
                    property.floatValue = Mathf.Max(0, newValue);
                }
                EditorGUIUtility.labelWidth = 0;
            }
 
            EditorGUI.EndProperty();
        }
    }

}