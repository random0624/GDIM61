using UnityEngine;

public class InGameUI : MonoBehaviour
{
    public GameObject mapUI;
    public GameObject integrityUI;
    public GameObject fuelUI;
    [Tooltip("Hierarchy object for wind/compass HUD (e.g. CompassUI). Same pattern as Fuel UI: assign the GameObject that has CompassController, not a special script.")]
    public GameObject compassUI;

    private void Start()
    {
        InGameUIHide();
        if (GameController.Instance != null)
        {
            GameController.Instance.OnSailStarted += InGameUIDisplay;
            GameController.Instance.OnMainMenuStarted += InGameUIHide;
        }
        InGameUIHide();
    }

    private void InGameUIHide()
    {
        mapUI.SetActive(false);
        integrityUI.SetActive(false);
        fuelUI.SetActive(false);
        if (compassUI != null)
            compassUI.SetActive(false);
    }

    private void InGameUIDisplay()
    {
        mapUI.SetActive(true);
        integrityUI.SetActive(true);
        fuelUI.SetActive(true);
        if (compassUI != null)
            compassUI.SetActive(true);
    }
}
