using UnityEngine;

/// <summary>
/// HUD arrows on a north-up style widget: boat heading (bow) and world wind direction on XZ.
/// Visibility: <see cref="InGameUI"/>.
/// </summary>
public class CompassController : MonoBehaviour
{
    [SerializeField] private WindZoneArea globalWind;
    [SerializeField] private Transform boat;

    [Header("Arrows")]
    [Tooltip("Points where the boat faces in the world (XZ), same frame as wind. Sprite should point +Y = north (+Z world) when Z rotation is 0.")]
    [SerializeField] private RectTransform boatHeadingArrow;
    [Tooltip("Points world wind blow direction on XZ.")]
    [SerializeField] private RectTransform windArrow;

    [SerializeField] private bool autoFindBoat = true;
    [SerializeField] private bool autoFindWindInScene = true;

    private void LateUpdate()
    {
        if (boatHeadingArrow == null && windArrow == null)
            return;

        if (boat == null && autoFindBoat)
        {
            BoatController found = FindObjectOfType<BoatController>();
            if (found != null)
                boat = found.transform;
        }

        if (globalWind == null && autoFindWindInScene)
            globalWind = FindObjectOfType<WindZoneArea>();

        if (boatHeadingArrow != null && boat != null)
        {
            Vector3 bow = boat.forward;
            bow.y = 0f;
            float boatBearingZ = 0f;
            if (bow.sqrMagnitude > 1e-6f)
            {
                bow.Normalize();
                boatBearingZ = Mathf.Atan2(bow.x, bow.z) * Mathf.Rad2Deg;
            }

            boatHeadingArrow.localEulerAngles = new Vector3(0f, 0f, -boatBearingZ);
        }

        if (windArrow != null)
        {
            Vector3 wind = globalWind != null ? globalWind.WindDirection : Vector3.zero;
            wind.y = 0f;

            float windBearingZ = 0f;
            if (wind.sqrMagnitude > 1e-6f)
            {
                wind.Normalize();
                windBearingZ = Mathf.Atan2(wind.x, wind.z) * Mathf.Rad2Deg;
            }

            windArrow.localEulerAngles = new Vector3(0f, 0f, -windBearingZ);
        }
    }
}
