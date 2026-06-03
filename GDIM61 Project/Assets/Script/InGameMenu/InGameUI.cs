using UnityEngine;

public class InGameUI : MonoBehaviour
{
    public GameObject mapUI;
    public GameObject integrityUI;
    public GameObject fuelUI;
    [Tooltip("Hierarchy object for wind/compass HUD (e.g. CompassUI). Same pattern as Fuel UI: assign the GameObject that has CompassController, not a special script.")]
    public GameObject compassUI;

    private void OnEnable()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.OnSailStarted -= InGameUIDisplay;
            GameController.Instance.OnMainMenuStarted -= InGameUIHide;
            GameController.Instance.OnSailStarted += InGameUIDisplay;
            GameController.Instance.OnMainMenuStarted += InGameUIHide;
        }
    }

    private void Start()
    {
        InGameUIHide();
    }

    private void OnDisable()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.OnSailStarted -= InGameUIDisplay;
            GameController.Instance.OnMainMenuStarted -= InGameUIHide;
        }
    }

    private void InGameUIHide()
    {
        SetUIActive(mapUI, false);
        SetUIActive(integrityUI, false);
        SetUIActive(fuelUI, false);
        SetUIActive(compassUI, false);
    }

    private void InGameUIDisplay()
    {
        SetUIActive(mapUI, true);
        SetUIActive(integrityUI, true);
        SetUIActive(fuelUI, true);
        SetUIActive(compassUI, true);
    }

    private void SetUIActive(GameObject target, bool active)
    {
        if (target != null)
        {
            target.SetActive(active);
        }
    }
}
