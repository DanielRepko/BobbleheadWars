﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject followTarget;
    public float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //makes the gameobject follow the followTarget as long as the property is not null
        if(followTarget != null)
        {
            transform.position = Vector3.Lerp(transform.position,
                                              followTarget.transform.position,
                                              moveSpeed * Time.deltaTime);
        }
    }
}
