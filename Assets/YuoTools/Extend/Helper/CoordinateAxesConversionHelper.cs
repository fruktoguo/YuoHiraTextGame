using UnityEngine;

public static class CoordinateAxesConversionHelper
{
    // 将本地坐标转换为世界坐标
    public static Vector3 LocalToWorld(Vector3 localCoordinates, Vector3 parentPos, Quaternion parentRot,
        Vector3 parentScale)
    {
        Matrix4x4 worldTransformMatrix = Matrix4x4.TRS(parentPos, parentRot, parentScale);
        Matrix4x4 worldToLocalMatrix = worldTransformMatrix;
        Vector4 localCoordinatesWithHomogeneous = localCoordinates;
        localCoordinatesWithHomogeneous.w = 1f;
        Vector4 worldCoordinatesWithHomogeneous = worldToLocalMatrix * localCoordinatesWithHomogeneous;
        Vector3 worldCoordinates = worldCoordinatesWithHomogeneous;
        return worldCoordinates;
    }

    // 将世界坐标转换为本地坐标
    public static Vector3 WorldToLocal(Vector3 worldCoordinates, Vector3 parentPos, Quaternion parentRot,
        Vector3 parentScale)
    {
        Matrix4x4 worldTransformMatrix = Matrix4x4.TRS(parentPos, parentRot, parentScale);
        Matrix4x4 localToWorldMatrix = worldTransformMatrix.inverse;
        Vector4 worldCoordinatesWithHomogeneous = worldCoordinates;
        worldCoordinatesWithHomogeneous.w = 1f;
        Vector4 localCoordinatesWithHomogeneous = localToWorldMatrix * worldCoordinatesWithHomogeneous;
        Vector3 localCoordinates = localCoordinatesWithHomogeneous;
        return localCoordinates;
    }
}
