using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Нужен если есть покупки за монеты внутри самой игры
/// (а не для случаем покупки через сервер магазина)
/// (сделал универсальным и для многраз. и для однораз. покупок.
/// (По этому этому и сохраняю все ключи покупок(для многораз. покупок эти ключи не нужны)
/// (И на каждую покупку создаю токен (одноразовым покупкам не нужен этот токен))
/// </summary>
public class BuyProductDefault : AbsBuyProduct
{
    private bool _init = false;
    public override bool IsInit => _init;
    public override event Action OnInit;

    [SerializeField] 
    private SD_StorageDataStringPrefs _storageSaveData;

    [SerializeField] 
    private SD_GetClassKeyDataGetDKOString _keySaveData;

    [SerializeField]
    private AbsGetPriceProductId _getPriceProductId;

    [SerializeField] 
    private StorageMoney _storageMoney;
    
    [SerializeField] 
    private ExampleStorageTokens _tokenData;
    
#if UNITY_EDITOR

    [SerializeField]
    private bool _isDebug;
#endif
    
    private void Awake()
    {
        if (_getPriceProductId.IsInit == false)
        {
#if UNITY_EDITOR
            if (_isDebug == true)
            {
                Debug.Log(gameObject.name + " BPD 10");
            }
#endif
            _getPriceProductId.OnInit += OnInitGetPriceProductId;
        }
        
        if (_storageSaveData.IsInit == false)
        {
#if UNITY_EDITOR
            if (_isDebug == true)
            {
                Debug.Log(gameObject.name + " BPD 20");
            }
#endif
            _storageSaveData.OnInit += OnInitStorageSaveData;
        }
        
        if (_storageMoney.IsInit == false)
        {
            
#if UNITY_EDITOR
            if (_isDebug == true)
            {
                Debug.Log(gameObject.name + " BPD 30");
            }
#endif
            _storageMoney.OnInit += OnInitStorageMoney;
        }
        
        
        if (_tokenData.IsInit == false)
        {
            _tokenData.OnInit += OnInitTokenData;
            return;
        }
        
        
        CheckInit();
    }

    private void OnInitGetPriceProductId()
    {
#if UNITY_EDITOR
        if (_isDebug == true)
        {
            Debug.Log(gameObject.name + " BPD 40");
        }
#endif
        _getPriceProductId.OnInit -= OnInitGetPriceProductId;
        CheckInit();
    }
    
    private void OnInitStorageSaveData()
    {
#if UNITY_EDITOR
        if (_isDebug == true)
        {
            Debug.Log(gameObject.name + " BPD 50");
        }
#endif
        _storageSaveData.OnInit -= OnInitStorageSaveData;
        CheckInit();
    }
    
    private void OnInitStorageMoney()
    {
#if UNITY_EDITOR
        if (_isDebug == true)
        {
            Debug.Log(gameObject.name + " BPD 60");
        }
#endif
        _storageMoney.OnInit -= OnInitStorageMoney;
        CheckInit();
    }
    
    private void OnInitTokenData()
    {
#if UNITY_EDITOR
        if (_isDebug == true)
        {
            Debug.Log(gameObject.name + " BPD 70");
        }
#endif
        
        _tokenData.OnInit += OnInitTokenData;
        CheckInit();
    }
    
    private void CheckInit()
    {
#if UNITY_EDITOR
        if (_isDebug == true)
        {
            Debug.Log(gameObject.name + " BPD 90");
        }
#endif
        if (_getPriceProductId.IsInit == true && _storageSaveData.IsInit == true && _storageMoney.IsInit == true && _tokenData.IsInit == true)  
        {
#if UNITY_EDITOR
            if (_isDebug == true)
            {
                Debug.Log(gameObject.name + " BPD 100 INIT");
            }
#endif
            _init = true;
            OnInit?.Invoke();
        }
    }

    public override void BuyLogic(Action<int, KeyProductId, StatusCallBackServer, BuyProductData, string> callback, int id, KeyProductId keyProduct, string keyInstanceClass)
    {
        float price = _getPriceProductId.GetPriceProduct(keyProduct);
        if (_storageMoney.CountMoney >= price) 
        {

            string jsData = _storageSaveData.GetData(_keySaveData.GetKey());
            ListProductBuy keyProductId = JsonUtility.FromJson<ListProductBuy>(jsData);

            if (keyProductId == null)
            {
                Debug.Log("ВНИМАНИЕ ПОТЕНЦИАЛЬНАЯ ОШИБКА, СПИСОК ПОКУПОК НЕ НАЙДЕН, СОЗДАН ПУСТОЙ СПИСОК ПОКУПОК");
                keyProductId = new ListProductBuy();
            }
            
            if (keyProductId.KeyProductId.Contains(keyProduct.GetKey()) == false)
            {
                keyProductId.KeyProductId.Add(keyProduct.GetKey());
            }
            else
            {
                Debug.Log("ВНИМАНИЕ ПОТЕНЦИАЛЬНАЯ ОШИБКА, ТОВАР С ТАКИМ КЛЮЧЕМ УЖЕ КУПЛЕМ !!!! KEY = " + keyProduct.GetKey());
            }
            
            _storageMoney.RemoveCount(price);


            //Создаем токен на кажд покупку(да да нужен только многораз. покупкам)
            _tokenData.AddToken(keyProduct);
            
            string jsDataSet = JsonUtility.ToJson(keyProductId);
            
            //Сохраняем данные об ключе купленного продукта(да да нужен только однораз. покупкам)
            _storageSaveData.SetData(_keySaveData.GetKey(),jsDataSet);
            _storageSaveData.SaveData(new TaskInfo("Save Buy"));
            
            
            callback.Invoke(id, keyProduct, StatusCallBackServer.Ok, new BuyProductData(true), keyInstanceClass);
            return;
        }
        
        callback.Invoke(id, keyProduct, StatusCallBackServer.Ok, new BuyProductData(false), keyInstanceClass);
    }
}

