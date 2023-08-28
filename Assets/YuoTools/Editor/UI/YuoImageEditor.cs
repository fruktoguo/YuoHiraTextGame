namespace UnityEditor.UI
{
    /// <summary>
    /// Editor class used to edit UI Sprites.
    /// </summary>
    [CustomEditor(typeof(YuoImage), true)]
    [CanEditMultipleObjects]
    public class YuoImageEditor : ImageEditor
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            mSegements = serializedObject.FindProperty("mSegements");
            mFillPercent = serializedObject.FindProperty("mFillPercent");
            mUseYuoVertex = serializedObject.FindProperty("mUseYuoVertex");
            mVertexType = serializedObject.FindProperty("mVertexType");
            mPolygonCollider2D = serializedObject.FindProperty("mPolygonCollider2D");
            mRadius = serializedObject.FindProperty("mRadius");
            mTriangleCount = serializedObject.FindProperty("mTriangleCount");
            ChangeUV = serializedObject.FindProperty("ChangeUV");
            RemapUV = serializedObject.FindProperty("RemapUV");
            RemapUVAngle = serializedObject.FindProperty("RemapUVAngle");
            RemapUVDistance = serializedObject.FindProperty("RemapUVDistance");
        }

        SerializedProperty mSegements;
        SerializedProperty mFillPercent;
        SerializedProperty mUseYuoVertex;
        SerializedProperty mVertexType;
        SerializedProperty mPolygonCollider2D;
        SerializedProperty mRadius;
        SerializedProperty mTriangleCount;
        SerializedProperty ChangeUV;
        SerializedProperty RemapUV;
        SerializedProperty RemapUVAngle;
        SerializedProperty RemapUVDistance;

        public override void OnInspectorGUI()
        {
            if (!mUseYuoVertex.boolValue)
            {
                EditorGUILayout.PropertyField(mUseYuoVertex);
                serializedObject.ApplyModifiedProperties();
                base.OnInspectorGUI();
            }
            else
            {
                EditorGUILayout.PropertyField(mUseYuoVertex);
                EditorGUILayout.PropertyField(ChangeUV);
                EditorGUILayout.PropertyField(RemapUV);
                EditorGUILayout.PropertyField(RemapUVAngle);
                EditorGUILayout.PropertyField(RemapUVDistance);
                SpriteGUI();
                AppearanceControlsGUI();
                RaycastControlsGUI();
                MaskableControlsGUI();
                NativeSizeButtonGUI();
                EditorGUILayout.PropertyField(mVertexType);
                switch ((YuoImage.YuoImageVertexType)mVertexType.enumValueIndex)
                {
                    case YuoImage.YuoImageVertexType.Circle:
                        EditorGUILayout.PropertyField(mSegements);
                        EditorGUILayout.PropertyField(mFillPercent);

                        break;
                    case YuoImage.YuoImageVertexType.Polygon:
                        EditorGUILayout.PropertyField(mPolygonCollider2D);
                        break;

                    case YuoImage.YuoImageVertexType.SquareFillet:
                        EditorGUILayout.PropertyField(mRadius);
                        EditorGUILayout.PropertyField(mTriangleCount);

                        break;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}