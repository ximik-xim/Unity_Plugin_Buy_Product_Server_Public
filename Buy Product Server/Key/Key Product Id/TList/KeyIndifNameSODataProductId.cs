using TListPlugin; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KeyIndifNameSODataProductId : AbsIdentifierAndData<ProductIdIndifNameSO, string, KeyProductId>
{

    [SerializeField]
    private KeyProductId _dataKey;


    public override KeyProductId GetKey()
    {
        return _dataKey;
    }
}
