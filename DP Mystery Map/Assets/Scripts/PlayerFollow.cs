using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{

    public GameObject playerObject;

    private static PlayerFollow _reference;

    private float _zPosition;
    private void Start()
    {
        if(_reference is not null)
        {
            Destroy(this.gameObject);
            return;
        }

        _reference = this;
        DontDestroyOnLoad(this.gameObject);
        _zPosition = transform.position.z;
    }

    // LateUpdate is called after all Update methods are finished
    void LateUpdate()
    {
        var newPosition = playerObject.transform.position;
        this.transform.position = new Vector3(newPosition.x,newPosition.y, _zPosition);
    }

    private void OnDestroy()
    {
        if (_reference != this)
            _reference = null;
    }
}
