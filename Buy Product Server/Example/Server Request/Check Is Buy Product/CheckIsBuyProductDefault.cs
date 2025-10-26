using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Проверяет куплен ли товар через хранилеще SD
///
/// Если в хранилеще есть сохраненный список с ключами товаров.
/// И в этом списке есть указ ключ товара, то товар был куплен
/// </summary>
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
    private SD_AbsStringStorage _storageSaveData;

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
