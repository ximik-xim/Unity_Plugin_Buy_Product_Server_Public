using System;
using System.Collections.Generic;
using UnityEngine;

public class CheckIsBuyProductDefault : AbsCheckIsBuyProduct
{
    public override bool IsInit => _storageSaveData.IsInit;
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

    [SerializeField] 
    private SD_StorageDataStringPrefs _storageSaveData;

    [SerializeField] 
    private SD_GetClassKeyDataGetDKOString _keySaveData;
    
    
    public override void CheckIsBuyLogic(Action<int, KeyProductId, StatusCallBackServer, CheckIsBuyProductData, string> callback, int id, KeyProductId keyProduct, string keyInstanceClass)
    {
        string jsData = _storageSaveData.GetData(_keySaveData.GetKey());

        if (jsData != "" && jsData != " " && jsData != string.Empty) 
        {
            ListProductBuy keyProductId = JsonUtility.FromJson<ListProductBuy>(jsData);
            if (keyProductId.KeyProductId.Contains(keyProduct.GetKey()) == true)
            {
                callback.Invoke(id, keyProduct, StatusCallBackServer.Ok, new CheckIsBuyProductData(true), keyInstanceClass);
                return;
            }
        }
        
        callback.Invoke(id, keyProduct, StatusCallBackServer.Ok, new CheckIsBuyProductData(false), keyInstanceClass);
    }
}
