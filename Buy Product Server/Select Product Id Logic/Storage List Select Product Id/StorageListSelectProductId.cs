using System.Collections.Generic;
using UnityEngine;
using System;

public class StorageListSelectProductId : MonoBehaviour
{
    [SerializeField] 
    private List<AbsKeyData<GetDataSO_KeyStorageSelectProductId, List<GetDataSODataProductId> >> _listData = new List<AbsKeyData<GetDataSO_KeyStorageSelectProductId, List<GetDataSODataProductId>>>();
    private Dictionary<string, List<KeyProductId>> _dictionaryData = new Dictionary<string, List<KeyProductId>>();
    
    public bool IsInit => _isInit;
    private bool _isInit = false;
    public event Action OnInit;
    
    private void Awake()
    {
        foreach (var VARIABLE in _listData)
        {
            _dictionaryData.Add(VARIABLE.Key.GetData().GetKey(), new List<KeyProductId>());

            foreach (var VARIABLE2 in VARIABLE.Data)
            {
                _dictionaryData[VARIABLE.Key.GetData().GetKey()].Add(VARIABLE2.GetData());
            }
        }       

        _isInit = true;
        OnInit?.Invoke();
    }

    public void AddSelectKeyProduct(KeyStorageSelectProductId key, KeyProductId keySelectProduct)
    {
        if (_dictionaryData.ContainsKey(key.GetKey()) == false) 
        {
            _dictionaryData.Add(key.GetKey(), new List<KeyProductId>());
        }

        if (_dictionaryData[key.GetKey()].Contains(keySelectProduct) == false)
        {
            _dictionaryData[key.GetKey()].Add(keySelectProduct);
        }
        
    }
    
    public void RemoveSelectKeyProduct(KeyStorageSelectProductId key, KeyProductId keySelectProduct)
    {
        if (_dictionaryData.ContainsKey(key.GetKey()) == true)
        {
            _dictionaryData[key.GetKey()].Remove(keySelectProduct);
        }
    }
    
    public List<KeyProductId> GetSelectKeyProduct(KeyStorageSelectProductId key)
    {
        return _dictionaryData[key.GetKey()];
    }
}
