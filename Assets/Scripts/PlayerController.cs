using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float speedSound;
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            speed = 20;
        else if (Input.GetKey(KeyCode.LeftControl))
            speed = 5;
        else
            speed = 10;

        if (speed == 10)
            speedSound = 1;
        if (speed == 20)
            speedSound = 2;
        if (speed == 5)
            speedSound = 0.5f;


        if (Input.GetKey(KeyCode.W))
        {
            this.transform.Translate(new Vector3(0, 0, speed) * Time.deltaTime, Space.Self);
        } else if (Input.GetKey(KeyCode.S))
        {
            this.transform.Translate(new Vector3(0, 0, -speed) * Time.deltaTime, Space.Self);
        }
        if (Input.GetKey(KeyCode.A))
        {
            this.transform.Translate(new Vector3(-speed, 0, 0) * Time.deltaTime, Space.Self);
        } else if (Input.GetKey(KeyCode.D))
        {
            this.transform.Translate(new Vector3(speed, 0, 0) * Time.deltaTime, Space.Self);
        }
    }
}
