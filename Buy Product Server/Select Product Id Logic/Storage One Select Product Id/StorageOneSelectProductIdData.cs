using System;
using UnityEngine;

public class StorageOneSelectProductIdData
{
    public StorageOneSelectProductIdData(KeyProductId keyProductId)
    {
        _key = keyProductId;
    }
    
    public KeyProductId Key => _key;
    private KeyProductId _key;

    public event Action OnUpdateData;
    
    public void SetKey(KeyProductId key)
    {
        _key = key;
        OnUpdateData?.Invoke();
    }

    public KeyProductId GetKey()
    {
        return _key;
    }
}