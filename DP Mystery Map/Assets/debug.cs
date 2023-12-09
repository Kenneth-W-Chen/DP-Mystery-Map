using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PlayerInfo;
using UnityEngine;

public class debug : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var temp = Resources.Load("Prefabs/Player", typeof(GameObject));
        if(temp is null)
            Debug.Log("Null");
        else
        {
            Debug.Log("Not null");
        }

        UnityEngine.Object.Instantiate(temp);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
