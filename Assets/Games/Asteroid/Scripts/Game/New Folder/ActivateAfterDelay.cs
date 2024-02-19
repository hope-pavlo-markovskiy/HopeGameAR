using UnityEngine;
using System.Collections;

public class ActivateAfterDelay : MonoBehaviour
{
    public GameObject objectToActivate;
    //public ShipController shipController;
    public float delay = 10f;

    void OnEnable()
    {
        StartCoroutine(ActivateObjectAfterDelay());
    }

    IEnumerator ActivateObjectAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        objectToActivate.SetActive(true);
        //shipController.enabled = true;
    }
}
