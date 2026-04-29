using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class IslandPaintPromptTrigger : MonoBehaviour
{
    [Header("Prompt UI")]
    [SerializeField] private GameObject promptObject;

    [Header("Paint UI")]
    [SerializeField] private RawImage drawingCanvas;
    [SerializeField] private Button closeCanvasButton;

    private bool playerInside;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;

        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!playerInside)
        {
            return;
        }

        if (!Input.GetKeyDown(KeyCode.Space))
        {
            return;
        }

        playerInside = false;

        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }

        if (GameController.Instance != null)
        {
            GameController.Instance.StartPaint();
        }

        if (drawingCanvas != null)
        {
            drawingCanvas.gameObject.SetActive(true);
        }

        if (closeCanvasButton != null)
        {
            closeCanvasButton.gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsBoat(other))
        {
            return;
        }

        playerInside = true;

        if (promptObject != null)
        {
            promptObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsBoat(other))
        {
            return;
        }

        playerInside = false;

        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private static bool IsBoat(Collider other)
    {
        return other.GetComponentInParent<BoatController>() != null ||
               other.GetComponent<BoatController>() != null;
    }
}
