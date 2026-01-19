using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovingPlatform1 : PlatformBase
{
    [Header("Movement")]
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    private Rigidbody platformRb;
    private bool goingToB = true;

    // Player
    private Rigidbody playerRb;
    private Vector3 lastPlatformPosition;

    void Awake()
    {
        platformRb = GetComponent<Rigidbody>();

 
        platformRb.isKinematic = true;
        platformRb.useGravity = false;
        platformRb.interpolation = RigidbodyInterpolation.Interpolate;
        platformRb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        lastPlatformPosition = platformRb.position;
    }

    void FixedUpdate()
    {
        MovePlatform();
        MovePlayerWithOffset();
        lastPlatformPosition = platformRb.position;
    }


    void MovePlatform()
    {
        Transform target = goingToB ? pointB : pointA;

        Vector3 newPos = Vector3.MoveTowards(
            platformRb.position,
            target.position,
            speed * Time.fixedDeltaTime
        );

        platformRb.MovePosition(newPos);

        if (Vector3.Distance(platformRb.position, target.position) < 0.05f)
            goingToB = !goingToB;
    }


    void MovePlayerWithOffset()
    {
        if (playerRb == null) return;

        Vector3 platformDelta = platformRb.position - lastPlatformPosition;
        playerRb.MovePosition(playerRb.position + platformDelta);
    }

  
    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        playerRb = collision.gameObject.GetComponent<Rigidbody>();
    }

    void OnCollisionExit(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        playerRb = null;
    }
}

