using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{

    public GameObject playerObject;


    private float _zPosition;
    private void Start()
    {
        _zPosition = transform.position.z;
    }

    // LateUpdate is called after all Update methods are finished
    void LateUpdate()
    {
        var newPosition = playerObject.transform.position;
        this.transform.position = new Vector3(newPosition.x,newPosition.y, _zPosition);
    }
}
