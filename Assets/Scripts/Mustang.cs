using UnityEngine;

public class Mustang : MonoBehaviour
{
    public float speed = 0.1f;

    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha2))
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Translate(-Vector3.forward * speed);
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Translate(Vector3.forward * speed);
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.Translate(Vector3.up * (speed * Time.deltaTime));
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.Translate(Vector3.down * (speed * Time.deltaTime));
            }
        }
    }

    void ApplyDistortion(Vector3 v)
    {
        transform.localScale = v;
    }
}