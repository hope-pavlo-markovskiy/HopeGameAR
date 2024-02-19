using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ProjectorController : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;

    [SerializeField] private DecalProjector projector;

    private void Awake()
    {
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.LookAt(targetObject.transform.position);
    }

    public void SetTarget(GameObject target)
    {
        targetObject = target;
    }
}
