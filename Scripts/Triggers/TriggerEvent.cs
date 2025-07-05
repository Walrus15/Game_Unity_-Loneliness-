using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TriggerEvent : MonoBehaviour
{
    public Light flickerLight;
    public GameObject breakableFloor;
    public float delayBeforeFall = 1f;

    private bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;
            StartCoroutine(EventSequence(other));
        }
    }

    IEnumerator EventSequence(Collider player)
    {
        var renderers = player.GetComponentsInChildren<Renderer>();

        flickerLight.enabled = false;
        yield return new WaitForSeconds(0.3f);
        foreach (var rend in renderers)
        {
            rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
        flickerLight.enabled = true;
        yield return new WaitForSeconds(0.5f);
        flickerLight.enabled = false;
        yield return new WaitForSeconds(0.7f);
        foreach (var rend in renderers)
        {
            rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }
        flickerLight.enabled = true;

        yield return new WaitForSeconds(delayBeforeFall);

        if (breakableFloor != null)
        {
            Rigidbody[] pieces = breakableFloor.GetComponentsInChildren<Rigidbody>();
            foreach (var piece in pieces)
            {
                piece.isKinematic = false;
                piece.useGravity = true;
            }
        }

    }
}
