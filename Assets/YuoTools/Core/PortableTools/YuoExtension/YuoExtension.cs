using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace YuoTools
{
    public static class YuoExtension
    {
        #region Color

        public static Color UpdateColorFade(this Color color, float fade)
        {
            fade = Mathf.Clamp(fade, 0, 1);
            color = new Color(color.r, color.g, color.b, fade);
            return color;
        }

        public static Color AddColorFade(this Color color, float fade)
        {
            float temp = fade + color.a;
            temp = Mathf.Clamp(temp, 0, 1);
            color = new Color(color.r, color.g, color.b, temp);
            return color;
        }

        public static Color Set(this ref Color color, float r, float g, float b, float a)
        {
            color.r = r;
            color.g = g;
            color.b = b;
            color.a = a;
            return color;
        }

        public static Color SetR(this ref Color color, float r)
        {
            color.r = r;
            return color;
        }

        public static Color SetG(this ref Color color, float g)
        {
            color.g = g;
            return color;
        }

        public static Color SetB(this ref Color color, float b)
        {
            color.b = b;
            return color;
        }

        public static Color RSetR(this Color color, float r)
        {
            color.r = r;
            return color;
        }

        public static Color RSetG(this Color color, float g)
        {
            color.g = g;
            return color;
        }

        public static Color RSetB(this Color color, float b)
        {
            color.b = b;
            return color;
        }

        public static Color RSetA(this Color color, float a)
        {
            color.a = a;
            return color;
        }

        public static void SetColorR(this Graphic image, float r)
        {
            image.color = image.color.RSetR(r);
        }

        public static void SetColorG(this Graphic image, float g)
        {
            image.color = image.color.RSetG(g);
        }

        public static void SetColorB(this Graphic image, float b)
        {
            image.color = image.color.RSetB(b);
        }

        public static void SetColorA(this Graphic image, float a)
        {
            image.color = image.color.RSetA(a);
        }

        public static void SetColorR(this SpriteRenderer renderer, float r)
        {
            renderer.color = renderer.color.RSetR(r);
        }

        public static void SetColorG(this SpriteRenderer renderer, float g)
        {
            renderer.color = renderer.color.RSetG(g);
        }

        public static void SetColorB(this SpriteRenderer renderer, float b)
        {
            renderer.color = renderer.color.RSetB(b);
        }

        public static void SetColorA(this SpriteRenderer renderer, float a)
        {
            renderer.color = renderer.color.RSetA(a);
        }

        #endregion Color

        #region Transform

        public static T GetOrAddComponent<T>(this Transform transform) where T : Component
        {
            T component = transform.GetComponent<T>();
            if (component == null)
            {
                component = transform.gameObject.AddComponent<T>();
            }

            return component;
        }

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            return GetOrAddComponent<T>(gameObject.transform);
        }

        public static void ResetTrans(this Transform tran)
        {
            tran.localPosition = Vector3.zero;
            tran.localEulerAngles = Vector3.zero;
            tran.localScale = Vector3.one;
        }

        public static void ResetTrans(this GameObject gameObject)
        {
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localEulerAngles = Vector3.zero;
            gameObject.transform.localScale = Vector3.one;
        }

        public static Transform Show(this Transform tran)
        {
            tran.gameObject.SetActive(true);
            return tran;
        }

        public static Transform Hide(this Transform tran)
        {
            tran.gameObject.SetActive(false);
            return tran;
        }

        public static Vector2 GetSquarePointByAngle_Round(this Rect rect, float angle)
        {
            angle = angle.Residual(360);
            float x = rect.width / 2;
            float y = rect.height / 2;
            float widthRatio = x > y ? 1 : x / y;
            float heightRatio = y > x ? 1 : y / x;

            float radius = x > y ? x : y;
            //总共有八个区域
            if (angle is >= 0 and < 45)
            {
                y = radius * Mathf.Tan(angle * Mathf.Deg2Rad);
                y *= heightRatio;
            }
            else if (angle is >= 45 and < 90)
            {
                angle = 90 - angle;
                x = radius * Mathf.Tan(angle * Mathf.Deg2Rad);
                x *= widthRatio;
            }
            else if (angle is >= 90 and < 135)
            {
                angle = angle - 90;
                x = -radius * Mathf.Tan(angle * Mathf.Deg2Rad);
                x *= widthRatio;
            }
            else if (angle is >= 135 and < 180)
            {
                angle = (180 - angle);
                y = radius * Mathf.Tan(angle * Mathf.Deg2Rad);
                x = -x;
                y *= heightRatio;
            }
            else if (angle is >= 180 and < 225)
            {
                angle = angle - 180;
                y = -radius * Mathf.Tan(angle * Mathf.Deg2Rad);
                x = -x;
                y *= heightRatio;
            }
            else if (angle is >= 225 and < 270)
            {
                angle = 270 - angle;
                x = -radius * Mathf.Tan(angle * Mathf.Deg2Rad);
                y = -y;
                x *= widthRatio;
            }
            else if (angle is >= 270 and < 315)
            {
                angle = angle - 270;
                x = radius * Mathf.Tan(angle * Mathf.Deg2Rad);
                y = -y;
                x *= widthRatio;
            }
            else if (angle is >= 315 and < 360)
            {
                angle = 360 - angle;
                y = -radius * Mathf.Tan(angle * Mathf.Deg2Rad);
                y *= heightRatio;
            }


            return new Vector2(x, y);
        }

        public static Vector2 GetSquarePointByAngle_Uniform(this Rect rect, float ratio)
        {
            ratio = ratio.Residual(1);

            float allLength = rect.width * 2 + rect.height * 2;
            float widthRatio = rect.width / allLength;
            float heightRatio = rect.height / allLength;

            float x = rect.width / 2;
            float y = rect.height / 2;

            if (ratio < widthRatio)
            {
                x = (ratio - widthRatio / 2) / widthRatio * rect.width;
            }
            else if (ratio < widthRatio + heightRatio)
            {
                y = (-ratio + widthRatio + heightRatio / 2) / heightRatio * rect.height;
            }
            else if (ratio < widthRatio * 2 + heightRatio)
            {
                x = (-ratio + widthRatio * 1.5f + heightRatio) / widthRatio * rect.width;
                y = -y;
            }
            else
            {
                y = (ratio - widthRatio * 2 - heightRatio * 1.5f) / heightRatio * rect.height;
                x = -x;
            }

            Temp.V2.Set(x, y);
            return Temp.V2;
        }


        public static void SetPos(this RectTransform tran, float x, float y)
        {
            Temp.V2.Set(x, y);
            tran.anchoredPosition = Temp.V2;
        }

        public static void SetPos(this Transform tran, float x, float y, float z)
        {
            Temp.V3.Set(x, y, z);
            tran.position = Temp.V3;
        }

        public static Vector3 RSet(this Vector3 pos, float x, float y, float z)
        {
            pos.x = x;
            pos.y = y;
            pos.z = z;
            return pos;
        }

        public static Vector2 RSet(this Vector2 pos, float x, float y)
        {
            pos.x = x;
            pos.y = y;
            return pos;
        }

        public static Vector3 SetX(this ref Vector3 v3, float x)
        {
            Temp.V3.Set(x, v3.y, v3.z);
            v3 = Temp.V3;
            return v3;
        }

        public static Vector3 SetY(this ref Vector3 v3, float y)
        {
            Temp.V3.Set(v3.x, y, v3.z);
            v3 = Temp.V3;
            return v3;
        }

        public static Vector3 SetZ(this ref Vector3 v3, float z)
        {
            Temp.V3.Set(v3.x, v3.y, z);
            v3 = Temp.V3;
            return v3;
        }

        public static Vector2 SetX(this ref Vector2 v2, float x)
        {
            Temp.V2.Set(x, v2.y);
            v2 = Temp.V2;
            return v2;
        }

        public static Vector2 SetY(this ref Vector2 v2, float y)
        {
            Temp.V2.Set(v2.x, y);
            v2 = Temp.V2;
            return v2;
        }

        public static Vector2 RSetX(this Vector2 v2, float x)
        {
            Temp.V2.Set(x, v2.y);
            v2 = Temp.V2;
            return v2;
        }

        public static Vector2 RSetY(this Vector2 v2, float y)
        {
            Temp.V2.Set(v2.x, y);
            v2 = Temp.V2;
            return v2;
        }

        public static Vector2 AddX(this ref Vector2 v2, float x)
        {
            return v2.SetX(v2.x + x);
        }

        public static Vector2 AddY(this ref Vector2 v2, float y)
        {
            return v2.SetY(v2.y + y);
        }

        public static Vector2 RAddX(this Vector2 v2, float x)
        {
            return v2.SetX(v2.x + x);
        }

        public static Vector2 RAddY(this Vector2 v2, float y)
        {
            return v2.SetY(v2.y + y);
        }

        public static Vector3 AddX(this ref Vector3 v3, float x)
        {
            return v3.SetX(v3.x + x);
        }

        public static Vector3 AddY(this ref Vector3 v3, float y)
        {
            return v3.SetY(v3.y + y);
        }

        public static Vector3 AddZ(this ref Vector3 v3, float z)
        {
            return v3.SetZ(v3.z + z);
        }

        public static Vector3 RAddX(this Vector3 v3, float x)
        {
            return v3.SetX(v3.x + x);
        }

        public static Vector3 RAddY(this Vector3 v3, float y)
        {
            return v3.SetY(v3.y + y);
        }

        public static Vector3 RAddZ(this Vector3 v3, float z)
        {
            return v3.SetZ(v3.z + z);
        }

        public static Vector3 RSetX(this Vector3 v3, float x)
        {
            Temp.V3.Set(x, v3.y, v3.z);
            v3 = Temp.V3;
            return v3;
        }

        public static Vector3 RSetY(this Vector3 v3, float y)
        {
            Temp.V3.Set(v3.x, y, v3.z);
            v3 = Temp.V3;
            return v3;
        }

        public static Vector3 RSetZ(this Vector3 v3, float z)
        {
            Temp.V3.Set(v3.x, v3.y, z);
            v3 = Temp.V3;
            return v3;
        }

        public static Vector3 SetPos(this ref Vector3 v3, float x, float y, float z)
        {
            Temp.V3.Set(x, y, z);
            v3 = Temp.V3;
            return v3;
        }

        #endregion Transform

        #region RectTransform

        public static void SetSize(this RectTransform rectTransform, float width, float height)
        {
            Temp.V2.Set(width, height);
            rectTransform.sizeDelta = Temp.V2;
        }

        public static void SetSize(this RectTransform rectTransform, Vector2 size)
        {
            rectTransform.sizeDelta = size;
        }

        public static void SetSizeX(this RectTransform rectTransform, float size)
        {
            rectTransform.sizeDelta = rectTransform.sizeDelta.RSetX(size);
        }

        public static void SetSizeY(this RectTransform rectTransform, float size)
        {
            rectTransform.sizeDelta = rectTransform.sizeDelta.RSetY(size);
        }

        public static void Copy(this RectTransform target, RectTransform from)
        {
            target.localScale = from.localScale;
            target.anchorMin = from.anchorMin;
            target.anchorMax = from.anchorMax;
            target.pivot = from.pivot;
            target.sizeDelta = from.sizeDelta;
            target.anchoredPosition3D = from.anchoredPosition3D;
        }

        public static void FullScreen(this RectTransform target, bool resetScaleToOne)
        {
            if (resetScaleToOne) target.ResetLocalScaleToOne();
            target.AnchorMinToZero();
            target.AnchorMaxToOne();
            target.CenterPivot();
            target.SizeDeltaToZero();
            target.ResetAnchoredPosition3D();
            target.ResetLocalPosition();
        }

        public static void Center(this RectTransform target, bool resetScaleToOne)
        {
            if (resetScaleToOne) target.ResetLocalScaleToOne();
            target.AnchorMinToCenter();
            target.AnchorMaxToCenter();
            target.CenterPivot();
            target.SizeDeltaToZero();
        }

        public static void ResetAnchoredPosition3D(this RectTransform target)
        {
            target.anchoredPosition3D = Vector3.zero;
        }

        public static void ResetLocalPosition(this RectTransform target)
        {
            target.localPosition = Vector3.zero;
        }

        public static void ResetLocalScaleToOne(this RectTransform target)
        {
            target.localScale = Vector3.one;
        }

        public static void AnchorMinToZero(this RectTransform target)
        {
            target.anchorMin = Vector2.zero;
        }

        public static void AnchorMinToCenter(this RectTransform target)
        {
            target.anchorMin = Vector2.one * 0.5f;
        }

        public static void AnchorMaxToOne(this RectTransform target)
        {
            target.anchorMax = Vector2.one;
        }

        public static void AnchorMaxToCenter(this RectTransform target)
        {
            target.anchorMax = Vector2.one * 0.5f;
        }

        public static void CenterPivot(this RectTransform target)
        {
            target.pivot = Vector2.one * 0.5f;
        }

        public static void SizeDeltaToZero(this RectTransform target)
        {
            target.sizeDelta = Vector2.zero;
        }

        public static float GetWidth(this RectTransform target)
        {
            return LayoutUtility.GetPreferredWidth(target);
        }

        public static float GetHeight(this RectTransform target)
        {
            return LayoutUtility.GetPreferredHeight(target);
        }

        public static Vector2 GetSize(this RectTransform target)
        {
            return Temp.V2.RSet(target.GetWidth(), target.GetHeight());
        }

        public static Vector3 SetAnchoredPosX(this RectTransform tran, float posX)
        {
            tran.anchoredPosition = Temp.V2.RSet(posX, tran.anchoredPosition.y);
            return tran.anchoredPosition;
        }

        public static Vector3 SetAnchoredPosY(this RectTransform tran, float posY)
        {
            tran.anchoredPosition = Temp.V2.RSet(tran.anchoredPosition.x, posY);
            return tran.anchoredPosition;
        }

        public static void ForceRebuildLayout(this RectTransform transform)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform);
        }

        #endregion

        #region Position

        public static Vector3 SetPosX(this Transform tran, float posX)
        {
            Temp.V3.Set(posX, tran.position.y, tran.position.z);
            tran.position = Temp.V3;
            return tran.position;
        }

        public static Vector3 SetPosY(this Transform tran, float posY)
        {
            Temp.V3.Set(tran.position.x, posY, tran.position.z);
            tran.position = Temp.V3;
            return tran.position;
        }

        public static Vector3 SetPosZ(this Transform tran, float PosZ)
        {
            Temp.V3.Set(tran.position.x, tran.position.y, PosZ);
            tran.position = Temp.V3;
            return tran.position;
        }

        public static Vector3 SetLocalPosX(this Transform tran, float PosX)
        {
            Temp.V3.Set(PosX, tran.localPosition.y, tran.localPosition.z);
            tran.localPosition = Temp.V3;
            return tran.localPosition;
        }

        public static Vector3 SetLocalPosY(this Transform tran, float PosY)
        {
            Temp.V3.Set(tran.localPosition.x, PosY, tran.localPosition.z);
            tran.localPosition = Temp.V3;
            return tran.localPosition;
        }

        public static Vector3 SetLocalPosZ(this Transform tran, float PosZ)
        {
            Temp.V3.Set(tran.localPosition.x, tran.localPosition.y, PosZ);
            tran.localPosition = Temp.V3;
            return tran.localPosition;
        }

        public static Vector3 AddPosX(this Transform tran, float posX)
        {
            Temp.V3.Set(tran.position.x + posX, tran.position.y, tran.position.z);
            tran.position = Temp.V3;
            return tran.position;
        }

        public static Vector3 AddPosY(this Transform tran, float posY)
        {
            Temp.V3.Set(tran.position.x, tran.position.y + posY, tran.position.z);
            tran.position = Temp.V3;
            return tran.position;
        }

        public static Vector3 AddPosZ(this Transform tran, float posZ)
        {
            Temp.V3.Set(tran.position.x, tran.position.y, tran.position.z + posZ);
            tran.position = Temp.V3;
            return tran.position;
        }

        public static Vector3 AddLocalPosX(this Transform tran, float posX)
        {
            Temp.V3.Set(tran.localPosition.x + posX, tran.localPosition.y, tran.localPosition.z);
            tran.localPosition = Temp.V3;
            return tran.localPosition;
        }

        public static Vector3 AddLocalPosY(this Transform tran, float posY)
        {
            Temp.V3.Set(tran.localPosition.x, tran.localPosition.y + posY, tran.localPosition.z);
            tran.localPosition = Temp.V3;
            return tran.localPosition;
        }

        public static Vector3 AddLocalPosZ(this Transform tran, float posZ)
        {
            Temp.V3.Set(tran.localPosition.x, tran.localPosition.y, tran.localPosition.z + posZ);
            tran.localPosition = Temp.V3;
            return tran.localPosition;
        }

        public static bool InRange(this Vector2Int pos, Vector2Int zero, int MaxWidth, int MinWidth, int MaxHeight,
            int MinHeight)
        {
            if (pos.x >= zero.x + MinWidth && pos.x < zero.x + MaxWidth && pos.y >= zero.y + MinHeight &&
                pos.y < zero.y + MaxHeight / 2)
                return true;
            return false;
        }

        public static bool iInRange(this Vector2Int pos, Vector2Int zero, int width, int height)
        {
            if ((pos.x >= zero.x - width / 2 && pos.x < zero.x + width / 2) &&
                (pos.y >= zero.y - height / 2 && pos.y < zero.y + height / 2))
                return true;
            return false;
        }

        #endregion Position

        #region GameObject

        public static GameObject Show(this GameObject gameObject)
        {
            if (!gameObject)
            {
                Debug.LogError("物体不存在");
                return gameObject;
            }

            if (!gameObject.activeSelf) gameObject.SetActive(true);
            return gameObject;
        }

        public static GameObject Hide(this GameObject gameObject)
        {
            if (!gameObject)
            {
                Debug.LogError("物体不存在");
                return gameObject;
            }

            if (gameObject.activeSelf) gameObject.SetActive(false);
            return gameObject;
        }

        public static T Hide<T>(this T gameObject) where T : Component
        {
            gameObject.gameObject.Hide();
            return gameObject;
        }

        public static List<T> HideAll<T>(this List<T> list) where T : Component
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i].gameObject.Hide();
            }

            return list;
        }

        public static List<GameObject> HideAll(this List<GameObject> list)
        {
            foreach (var item in list)
            {
                item.Hide();
            }

            return list;
        }


        public static T Show<T>(this T gameObject) where T : Component
        {
            gameObject.gameObject.Show();
            return gameObject;
        }

        public static List<T> ShowAll<T>(this List<T> list) where T : Component
        {
            foreach (var item in list)
            {
                item.Show();
            }

            return list;
        }

        public static List<GameObject> ShowAll(this List<GameObject> list)
        {
            foreach (var item in list)
            {
                item.Show();
            }

            return list;
        }

        public static List<T> EnableAll<T>(this List<T> list) where T : Behaviour
        {
            foreach (var item in list)
            {
                item.enabled = true;
            }

            return list;
        }

        public static List<T> DisableAll<T>(this List<T> list) where T : Behaviour
        {
            foreach (var item in list)
            {
                item.enabled = false;
            }

            return list;
        }

        public static GameObject ReShow(this GameObject gameObject)
        {
            if (!gameObject)
            {
                Debug.LogError("物体不存在");
                return gameObject;
            }

            gameObject.SetActive(false);
            gameObject.SetActive(true);
            return gameObject;
        }

        public static List<Transform> GetChildren(this Transform transform)
        {
            List<Transform> list = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                list.Add(transform.GetChild(i));
            }

            return list;
        }

        public static bool TryDestroy(this Component component)
        {
            if (component == null)
            {
                return false;
            }

            Object.Destroy(component);
            return true;
        }

        public static GameObject Instantiate(this GameObject gameObject, Transform parent)
        {
            return Object.Instantiate(gameObject, parent);
        }

        public static GameObject Instantiate(this GameObject gameObject, Transform parent, bool worldPositionStays)
        {
            return Object.Instantiate(gameObject, parent, worldPositionStays);
        }

        public static GameObject Instantiate(this GameObject gameObject, Vector3 position, Quaternion rotation)
        {
            return Object.Instantiate(gameObject, position, rotation);
        }

        #endregion GameObject

        #region Animator

        public static float GetClipLength(this Animator animator, string clip)
        {
            if (null == animator || string.IsNullOrEmpty(clip) || null == animator.runtimeAnimatorController)
                return 0;
            RuntimeAnimatorController ac = animator.runtimeAnimatorController;
            AnimationClip[] tAnimationClips = ac.animationClips;
            if (null == tAnimationClips || tAnimationClips.Length <= 0) return 0;
            AnimationClip tAnimationClip;
            for (int tCounter = 0, tLen = tAnimationClips.Length; tCounter < tLen; tCounter++)
            {
                tAnimationClip = ac.animationClips[tCounter];
                if (null != tAnimationClip && tAnimationClip.name == clip)
                    return tAnimationClip.length;
            }

            return 0F;
        }

        #endregion Animator

        #region Enum

        // public static System.Array GetAll<T>(this T _enum) where T : System.Enum
        // {
        //     return System.Enum.GetValues(typeof(T));
        // }

        public static T[] GetAll<T>(this T @enum) where T : System.Enum
        {
            var array = System.Enum.GetValues(typeof(T));
            T[] newArray = new T[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                newArray[i] = (T)array.GetValue(i);
            }

            return newArray;
        }

        #endregion Enum

        #region Main

        public static void Adds<T>(this List<T> list, params T[] t)
        {
            list.AddRange(t);
        }
        
        public static void DisposableAll(this List<System.IDisposable> list)
        {
            foreach (var t in list)
            {
                t.Dispose();
            }
        }
        
        public static void DisposeAll(this System.IDisposable[] array)
        {
            foreach (var t in array)
            {
                t.Dispose();
            }
        }

        #endregion
    }
}