using UnityEngine;

public class JetFan : MonoBehaviour
{
    public float speed = 0.1f;

    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha8))
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Translate(-Vector3.up * speed);
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Translate(-Vector3.down * speed);
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.Translate(Vector3.left * (speed * Time.deltaTime));
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.Translate(Vector3.right * (speed * Time.deltaTime));
            }
        }
    }

    void ApplyDistortion(Vector3 v)
    {
        transform.localScale = v;
    }
}