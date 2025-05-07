using System.Collections.Generic;
using UnityEngine;
using System;

public class StorageOneSelectProductId : MonoBehaviour
{
    [SerializeField] 
    private List<AbsKeyData<GetDataSO_KeyStorageSelectProductId, GetDataSODataProductId>> _listData = new List<AbsKeyData<GetDataSO_KeyStorageSelectProductId, GetDataSODataProductId>>();
    private Dictionary<string, StorageOneSelectProductIdData> _dictionaryData = new Dictionary<string, StorageOneSelectProductIdData>();
    
    public bool IsInit => _isInit;
    private bool _isInit = false;
    public event Action OnInit;

    /// <summary>
    /// Вызываеться когда обновляються данные об ключе
    /// (не очень эфективно, лучше переписать на конкретные классы, где уже буду уст. ключи. Но пока и так сойдет)
    /// </summary>
    public event Action OnUpdateData;
    
    private void Awake()
    {
        foreach (var VARIABLE in _listData)
        {
            _dictionaryData.Add(VARIABLE.Key.GetData().GetKey(), new StorageOneSelectProductIdData(VARIABLE.Data.GetData()));
        }       

        _isInit = true;
        OnInit?.Invoke();
    }

    public void AddSelectKeyProduct(KeyStorageSelectProductId key, StorageOneSelectProductIdData data)
    {
        _dictionaryData.Add(key.GetKey(), data);
        OnUpdateData?.Invoke();
    }
    
    public void RemoveSelectKeyProduct(KeyStorageSelectProductId key)
    {
        _dictionaryData.Remove(key.GetKey());
        OnUpdateData?.Invoke();
    }

    public bool IsThereData(KeyStorageSelectProductId key)
    {
       return  _dictionaryData.ContainsKey(key.GetKey());
    }
    
    public StorageOneSelectProductIdData GetSelectKeyProduct(KeyStorageSelectProductId key)
    {
        return _dictionaryData[key.GetKey()];
    }
}


