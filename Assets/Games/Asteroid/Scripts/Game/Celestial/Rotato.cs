using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotato : MonoBehaviour
{
    public float speed = 5f;    

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 2 * Time.deltaTime, 0, Space.Self);
        //this.transform.Rotate(0, this.transform.rotation.y + (speed * Time.deltaTime), 0);
    }
}
