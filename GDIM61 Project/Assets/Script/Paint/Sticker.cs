using UnityEngine;
using UnityEngine.UI;

public class Sticker : MonoBehaviour
{
    public static Sticker Instance;

    private void Awake()
    {
        Instance = this;
    }
    [Header("Canvas Area")]
    [SerializeField] private RectTransform paintRect;
    [SerializeField] private RectTransform worldMapRect;

    [Header("Sticker")]
    [SerializeField] private Image stickerPrefab;
    [SerializeField] private Vector2 mapStickerSize = new Vector2(40, 40);
    [SerializeField] private RectTransform winArea;
     public bool isWin = false;

    public void PlaceSticker(Sprite sprite, Vector2 screenPosition, Camera eventCamera)
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint(paintRect, screenPosition, eventCamera))
            return;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            paintRect,
            screenPosition,
            eventCamera,
            out localPoint
        );

        Image paintSticker = Instantiate(stickerPrefab, paintRect);
        paintSticker.sprite = sprite;
        paintSticker.rectTransform.anchoredPosition = localPoint;

        Vector2 mapPos = PaintToMapPosition(localPoint);

        Image mapSticker = Instantiate(stickerPrefab, worldMapRect);
        mapSticker.sprite = sprite;
        mapSticker.rectTransform.anchoredPosition = mapPos;
        mapSticker.rectTransform.sizeDelta = mapStickerSize;

        bool insideWinArea = RectTransformUtility.RectangleContainsScreenPoint(
            winArea,
            screenPosition,
            eventCamera
        );

        Debug.Log("inside win area: " + insideWinArea);

        if (insideWinArea)
        {
            isWin = true;
            Debug.Log("Win triggered!");
        }
    }

    private Vector2 PaintToMapPosition(Vector2 paintLocalPos)
    {
        float normalizedX = paintLocalPos.x / paintRect.rect.width + paintRect.pivot.x;
        float normalizedY = paintLocalPos.y / paintRect.rect.height + paintRect.pivot.y;

        float mapX = Mathf.Lerp(
            -worldMapRect.rect.width * worldMapRect.pivot.x,
            worldMapRect.rect.width * (1 - worldMapRect.pivot.x),
            normalizedX
        );

        float mapY = Mathf.Lerp(
            -worldMapRect.rect.height * worldMapRect.pivot.y,
            worldMapRect.rect.height * (1 - worldMapRect.pivot.y),
            normalizedY
        );

        return new Vector2(mapX, mapY);
    }
}
