using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindZoneArea : MonoBehaviour
{

    [Header("Wind Settings")]
    [SerializeField] private Transform windDirectionSource;
    [SerializeField] private float windStrength = 5f;

    public Vector3 WindDirection
    {
        get
        {
            if (windDirectionSource != null)
                return windDirectionSource.forward.normalized;

            return transform.forward.normalized;
        }
    }

    public float WindStrength => windStrength;

    private void OnDrawGizmos()
    {
        Vector3 dir = Application.isPlaying ? WindDirection : transform.forward;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, dir * 3f);
    }
}
