using UnityEngine;

public class WorldMapUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform boat;
    [SerializeField] private RectTransform mapRect;
    [SerializeField] private RectTransform playerMarker;

    [Header("World Bounds")]
    [SerializeField] private float worldMinX = -100f;
    [SerializeField] private float worldMaxX = 100f;
    [SerializeField] private float worldMinZ = -100f;
    [SerializeField] private float worldMaxZ = 100f;

    void Update()
    {
        if (boat == null || mapRect == null || playerMarker == null) return;

        Vector3 pos = boat.position;

        float normalizedX = Mathf.InverseLerp(worldMinX, worldMaxX, pos.x);
        float normalizedY = Mathf.InverseLerp(worldMinZ, worldMaxZ, pos.z);

        float mapX = Mathf.Lerp(-mapRect.rect.width * 0.5f, mapRect.rect.width * 0.5f, normalizedX);
        float mapY = Mathf.Lerp(-mapRect.rect.height * 0.5f, mapRect.rect.height * 0.5f, normalizedY);

        playerMarker.anchoredPosition = new Vector2(mapX, mapY);
    }
}
