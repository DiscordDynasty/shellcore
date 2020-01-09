﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCreatorCamera : MonoBehaviour
{
    public WorldCreatorCursor cursor;
    void FixedUpdate() {
        transform.position += Input.GetAxis("Horizontal") * Vector3.right;
        transform.position += Input.GetAxis("Vertical") * Vector3.up;

        if(Input.GetKey(KeyCode.LeftControl) && Input.GetAxis("Mouse ScrollWheel") < 0 && transform.position.z >= -150) 
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 5);
        }
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetAxis("Mouse ScrollWheel") > 0 && transform.position.z < -10) 
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 5);
        }

        if(Input.GetKeyUp(KeyCode.Space))
        {
            var vec = cursor.GetSectorCenter();
            transform.position = new Vector3(vec.x, vec.y, transform.position.z);
        }
    }
}