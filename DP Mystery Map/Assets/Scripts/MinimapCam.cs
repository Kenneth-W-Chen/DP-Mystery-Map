using System;
using System.Collections;
using System.Collections.Generic;
using PlayerInfo;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinimapCam : GameplayScript
{

    public GameObject playerObject;

    private static MinimapCam _reference;

    private float _zPosition;

    protected override void Start()
    {
        if (_reference is not null)
        {
            Destroy(this.gameObject);
            return;
        }
        base.Start();

        _reference = this;
        _zPosition = transform.position.z;
    }

    // LateUpdate is called after all Update methods are finished
    void LateUpdate()
    {
        var newPosition = playerObject.transform.position;
        this.transform.position = new Vector3(newPosition.x, newPosition.y, _zPosition);
    }

    private void OnDestroy()
    {
        if (_reference != this)
            _reference = null;
    }
}
