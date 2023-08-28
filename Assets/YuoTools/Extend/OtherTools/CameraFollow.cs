using UnityEngine;

namespace YuoTools
{
    public class CameraFollow : SingletonMono<CameraFollow>
    {
        public float speed = 1;
        public Transform Player;
        public SpriteRenderer BgSprite;
        public bool StartFollow;
        private V4 Range = new V4();

        public class V4
        {
            public float MinX = 0;
            public float MaxX = 0;
            public float MinY = 0;
            public float MaxY = 0;
        }

        private void Start()
        {
        }

        public void SetDefBg(SpriteRenderer spriteRenderer)
        {
            BgSprite = spriteRenderer;
            Range.MaxX = BgSprite.size.x / 2 * BgSprite.transform.localScale.x + BgSprite.transform.position.x;
            Range.MaxY = BgSprite.size.y / 2 * BgSprite.transform.localScale.y + BgSprite.transform.position.y;
            Range.MinX = -BgSprite.size.x / 2 * BgSprite.transform.localScale.x + BgSprite.transform.position.x;
            Range.MinY = -BgSprite.size.y / 2 * BgSprite.transform.localScale.y + BgSprite.transform.position.y;
            if (Range.MaxX < 0)
                Range.MaxX.Swap(ref Range.MinX);
            if (Range.MaxY < 0)
                Range.MaxY.Swap(ref Range.MinY);

            Range.MaxX -= 5f * ((float)Screen.width / Screen.height);
            Range.MaxY -= 5;
            Range.MinX += 5f * ((float)Screen.width / Screen.height);
            Range.MinY += 5;
            Range.MaxX.Clamp(0, Range.MaxX);
            Range.MaxY.Clamp(0, Range.MaxY);
            Range.MinX.Clamp(Range.MinX, 0);
            Range.MinY.Clamp(Range.MinY, 0);
        }

        public void SetBg(float MaxX, float MinX, float MaxY, float MinY)
        {
            Range.MaxX = MaxX;
        }

        private void LateUpdate()
        {
            Follow();
        }

        public void FollowStop()
        {
            Player = null;
        }

        private void Follow()
        {
            if (Player != null && StartFollow)
            {
                Temp.V3.Set(Player.position.x, Player.position.y, Camera.main.transform.position.z);
                Temp.V3 = Vector3.Lerp(Camera.main.transform.position, Temp.V3, Time.unscaledDeltaTime * speed);
                Camera.main.transform.SetPos(Temp.V3.x.Clamp(Range.MinX, Range.MaxX), Temp.V3.y.Clamp(Range.MinY, Range.MaxY), Camera.main.transform.position.z);
            }
        }
    }
}