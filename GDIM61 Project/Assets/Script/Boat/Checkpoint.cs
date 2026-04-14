using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    bool exited;

    void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (exited)
            return;

        var boat = other.GetComponentInParent<BoatController>() ?? other.GetComponent<BoatController>();
        if (boat == null)
            return;

        exited = true;

        BoatFuel.Instance?.Refill();
        BoatIntegrity.Instance?.HealIntegrity();

        boat.ResetToSpawn();
    }

    void OnTriggerExit(Collider other)
    {
        var boat = other.GetComponentInParent<BoatController>() ?? other.GetComponent<BoatController>();
        if (boat != null)
            exited = false;
    }
}
