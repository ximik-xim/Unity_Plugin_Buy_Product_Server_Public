using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Нужен для сохранения Токенов купленных продуктов
/// Нужно для многоразовых покупок, что бы в случае чего можно было узнать сле.
/// 1 - была ли обработана покупка(если нет, то тут токена не окажеться)
/// 2 - если сбросяться все покупки игрока, то в теории по этому списку их можно будет востоновить(как минимум буду знать что за продукт и сколько раз был куплен)
/// </summary>
public class SaveDataTokenReusableProductSDStorageDataStringPrefs : AbsSaveDataTokenReusableProduct
{
   [SerializeField] 
    private SD_StorageDataStringPrefs _storageSaveData;
    [SerializeField] 
    private SD_GetClassKeyDataGetDKOString _keySaveData;

    private StorageKeyListTokensBuyProduct _productTokensData;

    public override event Action OnSaveData;

    public override StatusStorageAction LastStatusSaveData => _lastStatusSaveData;
    private StatusStorageAction _lastStatusSaveData = StatusStorageAction.None;
    
    public override event Action OnUpdateData;
    
    public override event Action OnInit;
    public override bool IsInit => _isInit;
    private bool _isInit = false;

    private void Awake()
    {
        if (_storageSaveData.IsInit == false) 
        {
            _storageSaveData.OnInit += OnInitStorage;
            return;
        }

        Init();
    }

    private void OnInitStorage()
    {
        _storageSaveData.OnInit -= OnInitStorage;
        
        Init();
    }

    private void Init()
    {
        _storageSaveData.OnUpdateData += OnUpdateDataStorage;
        _storageSaveData.OnUpdateValue += OnUpdateValueStorage;

        _storageSaveData.OnSaveDataComplited += OnSaveDataComplitedStorage;
        
        SetData();

        _isInit = true;
        OnInit?.Invoke();
    }


    private void OnUpdateValueStorage(SD_KeyStorageStringVariable key)
    {
        if (_keySaveData.GetKey().GetKey() == key.GetKey())
        {
            string data = JsonUtility.ToJson(_productTokensData);
            string storageData = _storageSaveData.GetData(_keySaveData.GetKey());
            
            if (data != storageData)
            {
                SetData();
            }
        }
    }

    private void OnUpdateDataStorage()
    {
        if (_storageSaveData.LastStatusUpdateData == StatusStorageAction.Ok)
        {
            string data = JsonUtility.ToJson(_productTokensData);
            string storageData = _storageSaveData.GetData(_keySaveData.GetKey());
            
            if (data != storageData)
            {
                SetData();
            }
        }
    }

  
    private void SetData()
    {
        string dataJS = _storageSaveData.GetData(_keySaveData.GetKey());
        _productTokensData = JsonUtility.FromJson<StorageKeyListTokensBuyProduct>(dataJS);
        
        if (_productTokensData == null)
        {
            Debug.Log("ВНИМАНИЕ в хранилище с сохран. данными не были найдены данные об списке необработанных токенов для многораз. покупок");
            _productTokensData = new StorageKeyListTokensBuyProduct();
        }
        
        OnUpdateData?.Invoke();
    }
    

    private void OnSaveDataComplitedStorage()
    {
        _lastStatusSaveData = _storageSaveData.LastStatusSaveData;
        OnSaveData?.Invoke();
    }
    
    
    public override bool IsToken(KeyProductId keyProduct, string token)
    {
        if (_productTokensData.IsThereListToken(keyProduct.GetKey()) == true)
        {
            var listToken = _productTokensData.GetListToken(keyProduct.GetKey());

            foreach (var VARIABLE in listToken.ListProductToken)
            {
                if (VARIABLE.ProductToken == token) 
                {
                    return true;
                }
            }
        }

        return false;
    }

    public override void AddDataToken(KeyProductId keyProduct, string token)
    {
        if (_productTokensData.IsThereListToken(keyProduct.GetKey()) == false)
        {
            _productTokensData.AddElement(keyProduct.GetKey(), new ListProductTokenData());
        }

        var listToken = _productTokensData.GetListToken(keyProduct.GetKey());

        listToken.ListProductToken.Add(new ProductTokenData(token));

        //Это на случай, если по какой либо причине ссылка на список будет отличаться
        //_productTokensData.SetListToken(keyProduct.GetKey(), listToken);
    }

    public override void RemoveDataToken(KeyProductId keyProduct, string token)
    {
        if (_productTokensData.IsThereListToken(keyProduct.GetKey()) == false)
        {
            _productTokensData.AddElement(keyProduct.GetKey(), new ListProductTokenData());
        }

        var listToken = _productTokensData.GetListToken(keyProduct.GetKey());
        for (int i = 0; i < listToken.ListProductToken.Count; i++)
        {
            if (listToken.ListProductToken[i].ProductToken == token)
            {
                listToken.ListProductToken.RemoveAt(i);
                return;
            }
        }
        
    }

    public override void SaveData(TaskInfo taskInfo, bool urgentSaving = false)
    {
       _storageSaveData.SaveData(taskInfo, urgentSaving);
    }
    
    private void OnDestroy()
    {
        _storageSaveData.OnUpdateData -= OnUpdateDataStorage;
        _storageSaveData.OnUpdateValue -= OnUpdateValueStorage;
        
        _storageSaveData.OnSaveDataComplited -= OnSaveDataComplitedStorage;
    }

}



