using UnityEngine;
using static GameEnums;

public class FloatingItem : MonoBehaviour
{
    public float rotateSpeed = 50f;
    public float floatAmplitude = 0.2f;
    public float floatFrequency = 2f;
    public bool isFloatOnX = false;
    public bool isFloatOnY = true;
    public bool isFloatOnZ = false;

    private Vector3 startPos;
    private float X = 0;
    private float Y = 0;
    private float Z = 0;

    void Start()
    {
        startPos = transform.position;

        if (isFloatOnX)
        {
            X = rotateSpeed * Time.deltaTime;
        }
        if (isFloatOnY)
        {
            Y = rotateSpeed * Time.deltaTime;
        }
        if (isFloatOnZ)
        {
            Z = rotateSpeed * Time.deltaTime;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //rotate on y
        transform.Rotate(X, Y, Z);

        //hover
        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
