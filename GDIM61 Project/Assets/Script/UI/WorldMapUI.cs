using System.Collections.Generic;
using UnityEngine;

public class WorldMapUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform boat;
    [SerializeField] private RectTransform mapRect;
    [SerializeField] private RectTransform playerMarker;

    [Header("Chest")]
    [SerializeField] private List<Transform> chests;
    [SerializeField] private RectTransform chestMarkerPrefab;
    private List<RectTransform> chestMarkers = new List<RectTransform>();




    [Header("World Bounds")]
    [SerializeField] private float worldMinX = -100f;
    [SerializeField] private float worldMaxX = 100f;
    [SerializeField] private float worldMinZ = -100f;
    [SerializeField] private float worldMaxZ = 100f;

    private void Start()
    {
        foreach (Transform chest in chests)
        {
            RectTransform marker = Instantiate(chestMarkerPrefab, mapRect);
            chestMarkers.Add(marker);
        }
    }

    void Update()
    {
        if (boat == null || mapRect == null || playerMarker == null) return;

        playerMarker.anchoredPosition = WorldToMap(boat.position);

        for (int i = 0; i < chests.Count; i++)
        {
            if (chests[i] == null || chestMarkers[i] == null) continue;

            chestMarkers[i].anchoredPosition = WorldToMap(chests[i].position);
        }
    }
    private Vector2 WorldToMap(Vector3 pos)
    {
        float normalizedX = Mathf.InverseLerp(worldMinX, worldMaxX, pos.x);
        float normalizedY = Mathf.InverseLerp(worldMinZ, worldMaxZ, pos.z);

        float mapX = Mathf.Lerp(-mapRect.rect.width * 0.5f, mapRect.rect.width * 0.5f, normalizedX);
        float mapY = Mathf.Lerp(-mapRect.rect.height * 0.5f, mapRect.rect.height * 0.5f, normalizedY);

        return new Vector2(mapX, mapY);
    }
}

