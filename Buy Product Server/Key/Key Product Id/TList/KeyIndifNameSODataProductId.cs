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
    
#if UNITY_EDITOR
    public override string GetJsonSaveData()
    {
        return JsonUtility.ToJson(_dataKey);
    }

    public override void SetJsonData(string json)
    {
        _dataKey = JsonUtility.FromJson<KeyProductId>(json);
    }
#endif
}
