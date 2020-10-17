using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class spawner : SerializedMonoBehaviour
{

    [InfoBox("In order to serialize dctionaries, all we need to do is to inherit our class from SerializedMonoBehaviour.")]
    public Dictionary<string, GameObject[]> StringGameObjectDictionary = new Dictionary<string, GameObject[]>()
    {

    };
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
