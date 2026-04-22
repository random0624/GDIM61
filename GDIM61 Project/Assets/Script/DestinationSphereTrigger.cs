using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DestinationSphereTrigger : MonoBehaviour
{
    [SerializeField] private GameObject promptTextObject;
    [SerializeField] private bool hideOnStart = true;

    void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void Awake()
    {
        GetComponent<Collider>().isTrigger = true;

        if (hideOnStart && promptTextObject != null)
            promptTextObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<BoatController>() == null && other.GetComponent<BoatController>() == null)
            return;

        if (promptTextObject != null)
            promptTextObject.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<BoatController>() == null && other.GetComponent<BoatController>() == null)
            return;

        if (promptTextObject != null)
            promptTextObject.SetActive(false);
    }
}
