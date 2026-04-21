using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;

public class Painter : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [Header("Painter")]
    public int textureSize = 1024;     
    public int brushSize = 8;        
    public Color brushColor = Color.black; 

    public RawImage _rawImage;
    public RawImage miniMapRawImage;
    private Texture2D _drawableTexture;
    public RectTransform _rectTransform;
    public void OnPointerDown(PointerEventData eventData) => Draw(eventData);
    public void OnDrag(PointerEventData eventData) => Draw(eventData);
    // Start is called before the first frame update
    void Start()
    {
        _drawableTexture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        Color[] resetColorArray = _drawableTexture.GetPixels();
        for (int i = 0; i < resetColorArray.Length; i++) resetColorArray[i] = Color.white;
        _drawableTexture.SetPixels(resetColorArray);
        _drawableTexture.Apply();

        _rawImage.texture = _drawableTexture;
        if (miniMapRawImage != null)
        {
            miniMapRawImage.texture = _drawableTexture;
        }
    }

    private void Draw(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            float normalizedX = (localPoint.x / _rectTransform.rect.width) + _rectTransform.pivot.x;
            float normalizedY = (localPoint.y / _rectTransform.rect.height) + _rectTransform.pivot.y;
            int px = (int)(normalizedX * textureSize);
            int py = (int)(normalizedY * textureSize);

            PaintPixel(px, py);
        }
    }

    private void PaintPixel(int x, int y)
    {

        for (int i = -brushSize; i < brushSize; i++)
        {
            for (int j = -brushSize; j < brushSize; j++)
            {
                int targetX = x + i;
                int targetY = y + j;

                if (targetX >= 0 && targetX < textureSize && targetY >= 0 && targetY < textureSize)
                {
                    _drawableTexture.SetPixel(targetX, targetY, brushColor);
                }
            }
        }
        _drawableTexture.Apply();
    }

}
