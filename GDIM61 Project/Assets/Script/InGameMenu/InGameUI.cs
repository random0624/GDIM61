using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    public GameObject mapUI;
    public GameObject integrityUI;
    public GameObject fuelUI;

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
    }
    private void InGameUIDisplay()
    {
        mapUI.SetActive(true);
        integrityUI.SetActive(true);
        fuelUI.SetActive(true);
    }
}
