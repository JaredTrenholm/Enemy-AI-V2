using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public GameObject objectToFollow;
    private void Start()
    {
        this.transform.position = new Vector3(objectToFollow.transform.position.x, this.transform.position.y, objectToFollow.transform.position.z);
        this.transform.LookAt(objectToFollow.transform);
    }
    private void Update()
    {
        this.transform.position = new Vector3(objectToFollow.transform.position.x, this.transform.position.y, objectToFollow.transform.position.z);
    }
}
