using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Painter : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Painter")]
    public int textureSize = 1024;
    public int brushSize = 8;
    public Color brushColor = Color.black;

    public RawImage _rawImage;
    public RawImage miniMapRawImage;
    private Texture2D _drawableTexture;
    private Color32[] _pixelBuffer;
    private bool _textureDirty;
    private bool _hasLastPoint;
    private Vector2Int _lastPixelPoint;

    public RectTransform _rectTransform;

    public void OnPointerDown(PointerEventData eventData) => Draw(eventData);
    public void OnDrag(PointerEventData eventData) => Draw(eventData);
    public void OnPointerUp(PointerEventData eventData) => _hasLastPoint = false;

    void Start()
    {
        _drawableTexture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        _drawableTexture.filterMode = FilterMode.Point;

        _pixelBuffer = new Color32[textureSize * textureSize];
        Color32 white = Color.white;
        for (int i = 0; i < _pixelBuffer.Length; i++)
        {
            _pixelBuffer[i] = white;
        }

        _drawableTexture.SetPixels32(_pixelBuffer);
        _drawableTexture.Apply();

        _rawImage.texture = _drawableTexture;
        if (miniMapRawImage != null)
        {
            miniMapRawImage.texture = _drawableTexture;
        }
    }

    void LateUpdate()
    {
        if (!_textureDirty)
        {
            return;
        }

        _drawableTexture.SetPixels32(_pixelBuffer);
        _drawableTexture.Apply(false);
        _textureDirty = false;
    }

    private void Draw(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            float normalizedX = (localPoint.x / _rectTransform.rect.width) + _rectTransform.pivot.x;
            float normalizedY = (localPoint.y / _rectTransform.rect.height) + _rectTransform.pivot.y;
            int px = Mathf.Clamp((int)(normalizedX * textureSize), 0, textureSize - 1);
            int py = Mathf.Clamp((int)(normalizedY * textureSize), 0, textureSize - 1);
            Vector2Int currentPoint = new Vector2Int(px, py);

            if (_hasLastPoint)
            {
                PaintLine(_lastPixelPoint, currentPoint);
            }
            else
            {
                PaintBrush(px, py);
            }

            _lastPixelPoint = currentPoint;
            _hasLastPoint = true;
        }
    }

    private void PaintLine(Vector2Int start, Vector2Int end)
    {
        float distance = Vector2Int.Distance(start, end);
        int steps = Mathf.Max(1, Mathf.CeilToInt(distance));

        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
            int x = Mathf.RoundToInt(Mathf.Lerp(start.x, end.x, t));
            int y = Mathf.RoundToInt(Mathf.Lerp(start.y, end.y, t));
            PaintBrush(x, y);
        }
    }

    private void PaintBrush(int x, int y)
    {
        int radius = Mathf.Max(1, brushSize);
        Color32 targetColor = brushColor;

        for (int i = -radius; i <= radius; i++)
        {
            for (int j = -radius; j <= radius; j++)
            {
                if ((i * i) + (j * j) > radius * radius)
                {
                    continue;
                }

                int targetX = x + i;
                int targetY = y + j;

                if (targetX >= 0 && targetX < textureSize && targetY >= 0 && targetY < textureSize)
                {
                    int index = targetY * textureSize + targetX;
                    _pixelBuffer[index] = targetColor;
                }
            }
        }

        _textureDirty = true;
    }
}
