using UnityEngine;

public class FloatingItem : MonoBehaviour
{
    public float rotateSpeed = 50f;
    public float floatAmplitude = 0.2f;
    public float floatFrequency = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //rotate on y
        transform.Rotate(0,0, rotateSpeed * Time.deltaTime);

        //hover
        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
