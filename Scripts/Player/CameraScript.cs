using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform playerTransform;
    public Vector3 offset;
    public float camPositionSpeed = 5f;
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        Vector3 newCamPosition = new Vector3(offset.x, playerTransform.position.y + offset.y, playerTransform.position.z + offset.z);
        transform.position = Vector3.Lerp(transform.position, newCamPosition, camPositionSpeed * Time.deltaTime);   
    }
}
