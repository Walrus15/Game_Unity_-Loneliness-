using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public Transform respawnPoint;

    void Start()
    {
        if (respawnPoint == null)
        {
            respawnPoint = this.transform;
        }
    }

    public void RespawnPlayer(GameObject player)
    {
        player.SetActive(false);

        player.transform.position = respawnPoint.position;
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null)
        {
        }

        player.SetActive(true);

    }
}