using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
public class spawner : SerializedMonoBehaviour
{

    [SerializeField]
    public Dictionary<string, GameObject[]> StringGameObjectDictionary = new Dictionary<string, GameObject[]>();
  
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
