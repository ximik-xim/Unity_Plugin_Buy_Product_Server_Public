using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using AOT;
using UnityEngine;

/// <summary>
/// Пример запроса к серверу(в webGL в яндекс играх)
/// Т.К ПРИМЕР, ТО ПРИ СБОРКЕ НЕ БУДЕТ УЧАВСТВОВАТЬ, Т.К НЕТУ ФАИЛА С JS КОДОМ ДЛЯ РАБОТЫ И ПРИ СБОРКЕ БУДЕТ ВОЗНИКАТЬ ОШИБКА
/// </summary>
#if UNITY_EDITOR
public class GetProductTokensExampleV2 : AbsGetProductTokens
{
    [SerializeField] 
    private string _keyInstanceClass = "";
    private static Dictionary<string, GetProductTokensExampleV2> _instancesClasses = new Dictionary<string, GetProductTokensExampleV2>();
    private Dictionary<int, GetProductTokensExampleV2Data> _dataCallbackUploadingData = new Dictionary<int, GetProductTokensExampleV2Data>();
    
    private int _identifierRequest = 0;

    [DllImport("__Internal")]
    private static extern void YandexBuyProduct(Action<string, int, string, string, string> callBack, string keyInstanceClass, int indific, string productID);

    
    private bool _init = false;
    public override bool IsInit => _init;
    public override event Action OnInit;

    private void Awake()
    {
        _instancesClasses.Add(_keyInstanceClass, this);

        Init();
    }
    
    private void Init()
    {
        _init = true;
        OnInit?.Invoke();
    }

    public override void GetProductTokens(Action<int, KeyProductId, StatusCallBackServer, ListProductTokenData, string> callback, int id, KeyProductId keyProduct, string keyInstanceClass)
    {
        _identifierRequest++;

        var data = new GetProductTokensExampleV2Data(callback, keyInstanceClass, id);
        _dataCallbackUploadingData.Add(_identifierRequest, data);
        YandexBuyProduct(CallbackDataBuyProduct, _keyInstanceClass, _identifierRequest, keyProduct.GetKey());
    }

    [MonoPInvokeCallback(typeof(Action<string, int, string, string, string>))]
    private static void CallbackDataBuyProduct(string keyInstanceClass, int identifierRequest, string productId, string jsonData, string statusData)
    {
        var exampleClass = _instancesClasses[keyInstanceClass];
        var callbackData = exampleClass._dataCallbackUploadingData[identifierRequest];
        exampleClass._dataCallbackUploadingData.Remove(identifierRequest);

        if (jsonData == "null")
        {
            jsonData = "";
        }
        
        ListProductTokenData dataServer = JsonUtility.FromJson<ListProductTokenData>(jsonData); 
        StatusCallBackServer statusCallBack = JsonUtility.FromJson<StatusCallBackServer>(statusData);

        callbackData.CallBack.Invoke(callbackData.Id, new KeyProductId(productId), statusCallBack, dataServer, callbackData.KeyInstanceClass);
    }
    
    private void OnDestroy()
    {
        _instancesClasses.Remove(_keyInstanceClass);
    }
}

public class GetProductTokensExampleV2Data
{
    public GetProductTokensExampleV2Data(Action<int, KeyProductId, StatusCallBackServer, ListProductTokenData, string> callBack, string keyInstanceClass, int id)
    {
        _callBack = callBack;
        _id = id;
        _keyInstanceClass = keyInstanceClass;
    }

    private Action<int, KeyProductId, StatusCallBackServer, ListProductTokenData, string> _callBack;
    private string _keyInstanceClass;
    private int _id;

    public Action<int, KeyProductId, StatusCallBackServer, ListProductTokenData, string> CallBack => _callBack;
    public string KeyInstanceClass => _keyInstanceClass;
    public int Id => _id;
}
#endif

