using UnityEngine;

public class PushableObject : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameObject.AddComponent<FixedJoint>();
    }
}
