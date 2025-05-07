using System;
using UnityEngine;

public class StorageMoney : MonoBehaviour
{
    private bool _init = false;
    public bool IsInit => _init;
    public event Action OnInit;

    public event Action OnUpdateCount;
    [SerializeField]
    private float _countMoney;
    public float CountMoney => _countMoney;

    [SerializeField]
    private StorageMoneyTypeSave _typeSaveData;
    
    [SerializeField] 
    private SD_StorageDataFloatPrefs _storagSaveData;

    [SerializeField] 
    private SD_GetClassKeyDataGetDKOFloat _keySaveData;
    
    private void Awake()
    {
        if (_storagSaveData.IsInit == false)
        {
            _storagSaveData.OnInit += OnInitStorageSave;
            return;
        }

        Init();
    }

    private void OnInitStorageSave()
    {
        _storagSaveData.OnInit -= OnInitStorageSave;
        Init();
    }

    private void Init()
    {
        _storagSaveData.OnUpdateData += OnUpdateDataStorage;
        OnUpdateDataStorage();
        
        _init = true;
        OnInit?.Invoke();
    }

    private void OnUpdateDataStorage()
    {
        if (_storagSaveData.IsThereData(_keySaveData.GetKey()) == true)
        {
            _countMoney = _storagSaveData.GetData(_keySaveData.GetKey());
        }
        else
        {
            _countMoney = 0;
        }
    }

    public void AddCount(float value)
    {
        _countMoney += value;
        SaveData();
        
        OnUpdateCount?.Invoke();
    }

    public void RemoveCount(float value)
    {
        _countMoney -= value;
        
        SaveData();
        
        OnUpdateCount?.Invoke();
    }

    public float GetCountMoney()
    {
        return CountMoney;
    }

    private void SaveData()
    {
        switch (_typeSaveData)
        {
            case StorageMoneyTypeSave.SetValue:
            {
                _storagSaveData.SetData(_keySaveData.GetKey(), _countMoney);
            } break;
               
            case StorageMoneyTypeSave.SetValueAndSave:
            {
                _storagSaveData.SetData(_keySaveData.GetKey(), _countMoney);
                _storagSaveData.SaveData(new TaskInfo("Save Money"));
            } break;
        }
    }

    private void OnDestroy()
    {
        _storagSaveData.OnUpdateData -= OnUpdateDataStorage;
    }
}

public enum StorageMoneyTypeSave
{
    SetValue,
    SetValueAndSave
}
