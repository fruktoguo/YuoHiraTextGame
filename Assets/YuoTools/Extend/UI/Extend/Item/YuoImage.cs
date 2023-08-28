using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

public class YuoImage : Image
{
    [SerializeField] private bool mUseYuoVertex;

    [SerializeField] private YuoImageVertexType mVertexType = YuoImageVertexType.Circle;

    public enum YuoImageVertexType
    {
        Circle,
        Polygon,
        SquareFillet,
    }

    protected override void Awake()
    {
        base.Awake();
        mPopulateMeshNums = 0;
    }

    //记录顶点重建次数
    private int mPopulateMeshNums;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if (mUseYuoVertex)
        {
            switch (mVertexType)
            {
                case YuoImageVertexType.Circle:
                    DrawCircle(vh);
                    break;
                case YuoImageVertexType.Polygon:
                    DrawPolygon(vh);
                    break;
                case YuoImageVertexType.SquareFillet:
                    DrawSquareFillet(vh);
                    break;
                default:
                    base.OnPopulateMesh(vh);
                    break;
            }
        }
        else
        {
            base.OnPopulateMesh(vh);
        }

        mPopulateMeshNums++;
        // Debug.Log("顶点重建");
    }

    #region Circle

    [Range(3, 256)] [SerializeField] private int mSegements = 40;
    [Range(0, 1f)] [SerializeField] private float mFillPercent = 1;

    protected void DrawCircle(VertexHelper vh)
    {
        vh.Clear();

        ComputeUV();

        var rect = rectTransform.rect;
        float outerRadius = 0.5f * Mathf.Min(rect.width, rect.height);
        float degreeDelta = 2 * Mathf.PI / mSegements;
        int curSegements = (int)(mSegements * mFillPercent);


        float curDegree = 0;

        var curVertice = Vector2.zero;
        var vertexCount = curSegements + 1;
        vh.AddVert(GetUIVertex(curVertice, true));

        for (int i = 1; i < vertexCount; i++)
        {
            curVertice = new Vector2(Mathf.Cos(curDegree) * outerRadius, Mathf.Sin(curDegree) * outerRadius);
            curDegree += degreeDelta;
            vh.AddVert(GetUIVertex(curVertice));
        }

        var triangleCount = curSegements * 3;
        for (int i = 0, vIdx = 1; i < triangleCount - 3; i += 3, vIdx++)
        {
            vh.AddTriangle(vIdx, 0, vIdx + 1);
        }

        if (Math.Abs(mFillPercent - 1) < 0.001f)
        {
            //首尾顶点相连
            vh.AddTriangle(vertexCount - 1, 0, 1);
        }
    }

    #endregion

    #region Polygon

    [SerializeField] private PolygonCollider2D mPolygonCollider2D;

    protected void DrawPolygon(VertexHelper vh)
    {
        //因UGUI的锚点系统，这里需要考虑pivot的差值，以避免因物体原点的改变导致图片歪了
        vh.Clear();

        ComputeUV();

        var curVertex = Vector2.zero;
        vh.AddVert(GetUIVertex(curVertex, true));
        foreach (var t in mPolygonCollider2D.points)
        {
            vh.AddVert(GetUIVertex(t));
        }

        for (int i = 1; i < vh.currentVertCount - 1; i++)
        {
            vh.AddTriangle(0, i, i + 1);
        }

        vh.AddTriangle(0, vh.currentVertCount - 1, 1);
    }

    #endregion

    #region SquareFillet

    [SerializeField] [Range(0, 1.0f)] private float mRadius = 1f;

    [SerializeField] [Range(1, 64)] private int mTriangleCount = 100;

    protected void DrawSquareFillet(VertexHelper vh)
    {
        vh.Clear();
        ComputeUV();

        //计算真实半径范围
        float realRadius;
        if (rectTransform.rect.height < rectTransform.rect.width)
            realRadius = 0.5f * rectTransform.rect.height * mRadius;
        else
            realRadius = 0.5f * rectTransform.rect.width * mRadius;
        //确定四个边角圆形圆心的坐标
        //左下角圆心
        var rect = rectTransform.rect;
        Vector2 leftBottomCenter = new Vector2(-0.5f * rect.width + realRadius,
            -0.5f * rect.height + realRadius);
        //左上角圆心
        Vector2 leftTopCenter = new Vector2(-0.5f * rect.width + realRadius,
            0.5f * rect.height - realRadius);
        //右上角圆心
        Vector2 rightTopCenter = new Vector2(0.5f * rect.width - realRadius,
            0.5f * rect.height - realRadius);
        //右下角圆心
        Vector2 rightBottomCenter = new Vector2(0.5f * rect.width - realRadius,
            -0.5f * rect.height + realRadius);
        //确定左右矩形顶点坐标
        var leftCorner0 =
            new Vector2(-0.5f * rect.width, -0.5f * rect.height + realRadius);
        var leftCorner1 = new Vector2(-0.5f * rect.width, 0.5f * rect.height - realRadius);
        var rightCorner2 = new Vector2(0.5f * rect.width, 0.5f * rect.height - realRadius);
        var rightCorner3 =
            new Vector2(0.5f * rect.width, -0.5f * rect.height + realRadius);
        //确定上下矩形顶点坐标
        var downCorner0 =
            new Vector2(-0.5f * rect.width + realRadius, -0.5f * rect.height);
        var upCorner1 = new Vector2(-0.5f * rect.width + realRadius, 0.5f * rect.height);
        var upCorner2 = new Vector2(0.5f * rect.width - realRadius, 0.5f * rect.height);
        var downCorner3 = new Vector2(0.5f * rect.width - realRadius, -0.5f * rect.height);


        //首先绘制四个边角圆形
        List<Vector2> circleCenters = new List<Vector2>
        {
            leftBottomCenter,
            leftTopCenter,
            rightTopCenter,
            rightBottomCenter
        };

        //三角形的弧度
        float angle = 90f / mTriangleCount * Mathf.Deg2Rad;
        vh.AddVert(GetUIVertex(Vector2.zero, true));
        for (int i = 0; i < circleCenters.Count; i++)
        {
            DrawCircle(vh, circleCenters[i], realRadius, mTriangleCount, angle, color, i * 90 + 180);
        }

        var count = vh.currentVertCount;
        //依次绘制各个顶点和图形中心的三角形
        for (int i = 1; i < count - 1; i++)
        {
            vh.AddTriangle(0, i, i + 1);
        }

        //最后一个三角形
        vh.AddTriangle(0, count - 1, 1);
    }

    public void DrawCircle(VertexHelper vh, Vector2 center, float r, int triangle, float angle, Color c,
        float offsetAngle)
    {
        //添加圆心点为第一个点
        //vh.AddVert(GetUIVertex(center, true));
        // var start = 25 * triangle / 100;
        //获取圆上的点
        offsetAngle = offsetAngle * Mathf.Deg2Rad;
        for (int i = 0; i < triangle + 1; i++)
        {
            float newAngle = angle * i + offsetAngle;
            //此时以圆心为0,0点,且从圆形顶部开始计算
            Vector2 borderXY = new Vector2(r * Mathf.Sin(newAngle), r * Mathf.Cos(newAngle));
            //映射到矩形上的点应为
            Vector2 borderPos = center + borderXY;
            vh.AddVert(GetUIVertex(borderPos));
        }

        //绘制圆中三角形
        // for (int i = 0; i < triangle; i++)
        // {
        //     // if (i == triangle - 1)
        //     //     vh.AddTriangle(triangleIdx, i + 1 + triangleIdx, 1 + triangleIdx);
        //     // else
        //     vh.AddTriangle(triangleIdx, i + 1 + triangleIdx, i + 2 + triangleIdx);
        // }

        // triangleIdx += mTriangleCount + 2;
    }

    public UIVertex[] GetRectangleQuad(params Vector2[] vertices)
    {
        UIVertex[] vs = new UIVertex[vertices.Length];
        for (int i = 0; i < vs.Length; i++)
        {
            vs[i] = GetUIVertex(vertices[i]);
        }

        return vs;
    }

    #endregion

    public bool ChangeUV;

    public bool RemapUV;

    [Range(0, 360)] public float RemapUVAngle;
    public float RemapUVDistance;

    public UIVertex GetUIVertex(Vector2 point, bool isCenter = false)
    {
        //根据RemapUVAngle旋转uv
        Vector2 uv = Vector2.zero;
        if (RemapUV)
        {
            float angle = RemapUVAngle * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) - Mathf.Sin(angle);
            float y = Mathf.Sin(angle) + Mathf.Cos(angle);
            Debug.Log((new Vector2(x, y),new Vector2(x, y).normalized.magnitude) );
            uv = (new Vector2(x, y).normalized) * RemapUVDistance;
        }

        UIVertex vertex = new UIVertex
        {
            color = color,
            position = point + mPivotVector,
            uv0 = !ChangeUV || isCenter ? new Vector2(point.x * uvScaleX, point.y * uvScaleY) + mCenter :
                !RemapUV ? Vector2.zero : uv
        };
        return vertex;
    }

    public void ComputeUV()
    {
        var rect = rectTransform.rect;
        var pivot = rectTransform.pivot;
        float tw = rect.width;
        float th = rect.height;
        mPivotVector = new Vector2(tw * (0.5f - pivot.x), th * (0.5f - pivot.y));
        mUv = overrideSprite != null ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;
        mCenter = new Vector2(mUv.x + mUv.z, mUv.y + mUv.w) * 0.5f;
        uvScaleX = (mUv.z - mUv.x) / tw;
        uvScaleY = (mUv.w - mUv.y) / th;
    }

    float uvScaleX;
    float uvScaleY;

    Vector2 mPivotVector;
    Vector2 mCenter;
    Vector4 mUv;
}