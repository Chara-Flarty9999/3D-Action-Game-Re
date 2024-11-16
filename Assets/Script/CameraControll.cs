using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControll : MonoBehaviour
{
    float side;
    float ver;
    float speed = 2f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(Input.GetAxis(ÅgMouse XÅh));
        float h = Input.GetAxis("Mouse X");
        float v = Input.GetAxis("Mouse Y");
        side += h * speed;
        ver += v * speed;

        transform.rotation = Quaternion.Euler(ver, side, 0f);

    }
}

