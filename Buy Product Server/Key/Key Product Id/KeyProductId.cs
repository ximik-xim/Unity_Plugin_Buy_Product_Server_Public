using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KeyProductId 
{
    public KeyProductId()
    {
        
    }

    public KeyProductId(string key)
    {
        _key = key;
    }
    
    [SerializeField]
    private string _key;

    public string GetKey()
    {
        return _key;
    }
}
