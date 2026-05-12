using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StickerControl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Sticker stickerManager;
    [SerializeField] private Sprite stickerSprite;

    private Image previewImage;
    private Canvas rootCanvas;

    private void Start()
    {
        rootCanvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GameObject previewObj = new GameObject("Sticker Preview");
        previewObj.transform.SetParent(rootCanvas.transform, false);

        previewImage = previewObj.AddComponent<Image>();
        previewImage.sprite = stickerSprite;
        previewImage.raycastTarget = false;

        RectTransform rect = previewImage.rectTransform;
        rect.sizeDelta = new Vector2(50, 50);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (previewImage != null)
        {
            previewImage.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (previewImage != null)
        {
            Destroy(previewImage.gameObject);
        }

        stickerManager.PlaceSticker(
            stickerSprite,
            eventData.position,
            eventData.pressEventCamera
        );
    }
}