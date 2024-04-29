using System;
using System.Collections;
using System.Collections.Generic;
using champs3.Hotfix;
using UnityEngine;

public class ResourceManagerInit : MonoBehaviour
{
    private void Awake()
    {
        #if UNITY_EDITOR
                ResourceManager.Instance.Initialize();
        #endif
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
