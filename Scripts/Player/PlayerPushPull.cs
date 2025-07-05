using UnityEngine;

public class PlayerPushPull : MonoBehaviour
{
    public float pushSpeed = 3f;
    private GameObject pushableObject = null;
    private bool isPushing = false;
    private Rigidbody rb;
    private FixedJoint joint;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (pushableObject != null && Input.GetKeyDown(KeyCode.E) && IsColliding(pushableObject))
        {
            StartPushing();
        }
        else if (isPushing && Input.GetKeyUp(KeyCode.E))
        {
            StopPushing();
        }

        if (isPushing && pushableObject != null)
        {
            PushObject();
        }
    }

    void StartPushing()
    {
        if (pushableObject != null && joint == null)
        {
            isPushing = true;
            joint = pushableObject.AddComponent<FixedJoint>();
            joint.connectedBody = rb;
            joint.breakForce = Mathf.Infinity;
            joint.breakTorque = Mathf.Infinity;

        }
    }

    void StopPushing()
    {
        if (joint != null)
        {
            Destroy(joint);
            joint = null;
            isPushing = false;
        }
    }

    void PushObject()
    {
        Rigidbody pushableRb = pushableObject.GetComponent<Rigidbody>();

        if (pushableRb != null)
        {
            Vector3 moveDirection = new Vector3(0, 0, Input.GetAxis("Horizontal"));
            pushableRb.linearVelocity = moveDirection * pushSpeed;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pushable"))
        {
            pushableObject = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pushable"))
        {
            pushableObject = null;
        }
    }

    bool IsColliding(GameObject obj)
    {
        Collider playerCollider = GetComponent<BoxCollider>();
        Collider objCollider = obj.GetComponent<Collider>();
        return playerCollider.bounds.Intersects(objCollider.bounds);
    }
}
