using UnityEngine;

namespace YuoTools.Extend.Helper
{
    /// <summary>
    /// 坐标系转换帮助类
    /// </summary>
    public static class CoordinateConversionHelper
    {
        public static Vector3 LocalToWorld(Vector3 localPosition, Vector3 parentPosition, Quaternion parentRotation) => LocalToWorld(localPosition, parentPosition, parentRotation, Vector3.one);

        /// <summary>
        /// 将本地坐标转换为世界坐标
        /// </summary>
        public static Vector3 LocalToWorld(Vector3 localPosition, Vector3 parentPosition, Quaternion parentRotation,
            Vector3 parentScale)
        {
            Matrix4x4 transformationMatrix = Matrix4x4.TRS(parentPosition, parentRotation, parentScale);
            Vector4 localPositionWithHomogeneous = localPosition;
            localPositionWithHomogeneous.w = 1f;
            Vector4 worldPositionWithHomogeneous = transformationMatrix * localPositionWithHomogeneous;
            Vector3 worldPosition = worldPositionWithHomogeneous;
            return worldPosition;
        }

        public static Vector3 WorldToLocal(Vector3 worldPosition, Vector3 parentPosition, Quaternion parentRotation) => WorldToLocal(worldPosition, parentPosition, parentRotation, Vector3.one);

        /// <summary>
        /// 将世界坐标转换为本地坐标
        /// </summary>
        public static Vector3 WorldToLocal(Vector3 worldPosition, Vector3 parentPosition, Quaternion parentRotation,
            Vector3 parentScale)
        {
            Matrix4x4 transformationMatrix = Matrix4x4.TRS(parentPosition, parentRotation, parentScale);
            Matrix4x4 inverseTransformationMatrix = transformationMatrix.inverse;
            Vector4 worldPositionWithHomogeneous = worldPosition;
            worldPositionWithHomogeneous.w = 1f;
            Vector4 localPositionWithHomogeneous = inverseTransformationMatrix * worldPositionWithHomogeneous;
            Vector3 localPosition = localPositionWithHomogeneous;
            return localPosition;
        }
    }
}