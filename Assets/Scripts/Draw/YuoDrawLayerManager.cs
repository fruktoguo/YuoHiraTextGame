using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.Draw
{
    public class YuoDrawLayerManager : MonoBehaviour
    {
        public List<DrawingBoard> DrawingBoards = new List<DrawingBoard>();
        public List<RawImage> LayerPreviews = new List<RawImage>();

        public int CurrentLayerIndex = 0;

        public DrawingBoard CurrentLayer => DrawingBoards[CurrentLayerIndex];

        public void AddLayerPreview(DrawingBoard board)
        {
            var go = new GameObject($"LayerPreview_{LayerPreviews.Count}", typeof(RawImage), typeof(RectTransform));
            var rect = go.GetComponent<RectTransform>();
            var rawImage = go.GetComponent<RawImage>();

            rect.SetParent(transform);
            rect.localScale = Vector3.one;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = Vector2.zero * 100;
            rect.anchoredPosition = Vector2.zero;

            rawImage.color = Color.white;
            rawImage.texture = board.Texture;

            int currentPreviewIndex = LayerPreviews.Count;

            go.AddComponent<Button>().onClick.AddListener(() =>
            {
                CurrentLayerIndex = currentPreviewIndex;
            });

            LayerPreviews.Add(rawImage);
        }
    }
}