using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainCameraScript : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotationSpeed = 100f;

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return; // Exit Update method if the pointer is over a UI element
        }

        Vector3 translation = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            translation += new Vector3(0, 0, 1);
        if (Input.GetKey(KeyCode.S))
            translation += new Vector3(0, 0, -1);
        if (Input.GetKey(KeyCode.A))
            translation += new Vector3(-1, 0, 0);
        if (Input.GetKey(KeyCode.D))
            translation += new Vector3(1, 0, 0);
        if (Input.GetKey(KeyCode.Q))
            translation += new Vector3(0, -1, 0);
        if (Input.GetKey(KeyCode.E))
            translation += new Vector3(0, 1, 0);

        translation = translation.normalized * moveSpeed * Time.deltaTime;

        Camera.main.transform.Translate(translation);

        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            Camera.main.transform.Rotate(Vector3.up, mouseX, Space.World);
            Camera.main.transform.Rotate(Vector3.right, -mouseY, Space.Self);
        }
    }
}
