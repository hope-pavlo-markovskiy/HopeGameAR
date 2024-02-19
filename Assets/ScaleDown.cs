using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleDown : MonoBehaviour
{
    Boom _boom;
    Rigidbody rb;

    public float scaleSpeed = 1f;
    public float minScale = 0.1f;
    public float Speed = 1f;
    private bool _hasExploded;

    // Start is called before the first frame update
    void Start()
    {
        _boom = GetComponentInParent<Boom>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _hasExploded = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(_hasExploded)
        {
            transform.localScale -= new Vector3(scaleSpeed, scaleSpeed, scaleSpeed) * Time.deltaTime * Speed;

            transform.localScale = Vector3.Max(transform.localScale, new Vector3(minScale, minScale, minScale));

            if (transform.localScale.x <= minScale)
            {
                Destroy(gameObject);
            }

        }
    }
}
