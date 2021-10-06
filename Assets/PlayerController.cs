using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    void Update()
    {
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
