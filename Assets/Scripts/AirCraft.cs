using UnityEngine;

public class AirCraft : MonoBehaviour
{
    public float speed = 1.0f;
    public float shiftMultiplier = 4.0f; // Multiplier for speed when Shift is held down

    void Update()
    {
        float currentSpeed = speed;

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentSpeed *= shiftMultiplier;
        }


            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Translate(Vector3.right * currentSpeed);

            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Translate(Vector3.left * currentSpeed);
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.Translate(Vector3.up * currentSpeed );
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.Translate(Vector3.down * currentSpeed );
            }
        
    }

    void ApplyDistortion(Vector3 v)
    {
        transform.localScale = v;
    }
}