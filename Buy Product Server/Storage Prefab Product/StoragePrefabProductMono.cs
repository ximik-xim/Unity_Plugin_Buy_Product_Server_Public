using System;
using System.Collections.Generic;
using UnityEngine;

public class StoragePrefabProductMono : MonoBehaviour
{
    [SerializeField] 
    private List<AbsKeyData<GetDataSODataProductId, DKOKeyAndTargetAction>> _listData = new List<AbsKeyData<GetDataSODataProductId, DKOKeyAndTargetAction>>();
    private Dictionary<string, DKOKeyAndTargetAction> _dictionaryData = new Dictionary<string, DKOKeyAndTargetAction>();
    
    public bool IsInit => _isInit;
    private bool _isInit = false;
    public event Action OnInit;
    
    private void Awake()
    {
        foreach (var VARIABLE in _listData)
        {
            _dictionaryData.Add(VARIABLE.Key.GetData().GetKey(), VARIABLE.Data);
        }       

        _isInit = true;
        OnInit?.Invoke();
    }

    public DKOKeyAndTargetAction GetPrefabProduct(KeyProductId key)
    {
        return _dictionaryData[key.GetKey()];
    }
}
