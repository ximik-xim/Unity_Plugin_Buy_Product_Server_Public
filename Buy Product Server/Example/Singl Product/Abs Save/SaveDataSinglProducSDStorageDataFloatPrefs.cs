using System;
using UnityEngine;

public class SaveDataSinglProducSDStorageDataFloatPrefs : AbsSaveDataSinglProduct
{
    [SerializeField] 
    private SD_StorageDataFloatPrefs _storageSaveData;
    [SerializeField] 
    private SD_GetClassKeyDataGetDKOFloat _keySaveData;


    public override bool IsInit()
    {
       return _storageSaveData.IsInit;
    }

    public override event Action OnInit
    {
        add
        {
            _storageSaveData.OnInit += value;
        }
        remove
        {
            _storageSaveData.OnInit -= value;
        }
    }

    public override event Action OnUpdateCurrentValue;

    public override event Action OnUpdateStorageDataData
    {
        add
        {
            _storageSaveData.OnUpdateData += value;
        }
        remove
        {
            _storageSaveData.OnUpdateData -= value;
        }
    }
    
    
    private void Awake()
    {
        _storageSaveData.OnUpdateValue += OnUpdateValue;
    }

    private void OnUpdateValue(SD_KeyStorageFloatVariable key)
    {
        if (key.GetKey() == _keySaveData.GetKey().GetKey())
        {
            OnUpdateCurrentValue?.Invoke();
        }
    }
    
    public override bool IsBuyProduct()
    {
        if (_storageSaveData.IsThereData(_keySaveData.GetKey()) == true)
        {
            if (_storageSaveData.GetData(_keySaveData.GetKey()) == 1f)
            {
                return true;
            }
        }

        return false;
    }

    public override void SetStatusBuyProduct()
    {
        _storageSaveData.SetData(_keySaveData.GetKey(), 1f);
    }

    public override void RemoveStatusBuyProduct()
    {
        _storageSaveData.SetData(_keySaveData.GetKey(), 0f);
    }

   
    public override event Action OnSaveData
    {
        add
        {
            _storageSaveData.OnSaveDataComplited += value;
        }
        remove
        {
            _storageSaveData.OnSaveDataComplited -= value;
        }
    }
    
    public override void SaveData(TaskInfo taskInfo, bool urgentSaving = false)
    {
        _storageSaveData.SaveData(taskInfo, urgentSaving);
    }

    public override StatusStorageAction LastStatusSaveData => _storageSaveData.LastStatusSaveData;

    private void OnDestroy()
    {
        _storageSaveData.OnUpdateValue -= OnUpdateValue;
    }
}
