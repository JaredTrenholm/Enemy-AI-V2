using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float baseSpeed;
    public float speedSound;

    private float speed;
    void Update()
    {
        CheckSound();
        CheckInput();
    }
    private void CheckSound()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            speed = baseSpeed * 2;
        else if (Input.GetKey(KeyCode.LeftControl))
            speed = baseSpeed / 2;
        else
            speed = baseSpeed;
        if (Input.anyKey)
        {
            if (speed == baseSpeed)
                speedSound = 1;
            if (speed == baseSpeed * 2)
                speedSound = 2;
            if (speed == baseSpeed / 2)
                speedSound = 0.25f;
        }
        else
        {
            speedSound = 0;
        }
    }
    private void CheckInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            this.transform.Translate(new Vector3(0, 0, speed) * Time.deltaTime, Space.Self);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            this.transform.Translate(new Vector3(0, 0, -speed) * Time.deltaTime, Space.Self);
        }
        if (Input.GetKey(KeyCode.A))
        {
            this.transform.Translate(new Vector3(-speed, 0, 0) * Time.deltaTime, Space.Self);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            this.transform.Translate(new Vector3(speed, 0, 0) * Time.deltaTime, Space.Self);
        }
    }
}
