using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Пример запроса синхроно(без ожидания ответа от сервера)
/// </summary>
public class GetProductTokensExampleV1 : AbsGetProductTokens
{
    public override bool IsInit => _isInit;
    private bool _isInit = false;
    public override event Action OnInit;

    [SerializeField] 
    private ExampleStorageTokens _tokenData;

    [SerializeField] 
    private StatusCallBackServer _statusServer;

    // [SerializeField] 
    // private bool _autoGenerateToken = false;
    
    private void Awake()
    {
        if (_tokenData.IsInit == false)
        {
            _tokenData.OnInit += OnInitTokenData;
            return;
        }

        InitTokenData();
    }

    private void OnInitTokenData()
    {
        _tokenData.OnInit += OnInitTokenData;
        InitTokenData();
    }
    
    private void InitTokenData()
    {
        _isInit = true;
        OnInit?.Invoke();
    }


    public override void GetProductTokens(Action<int, KeyProductId, StatusCallBackServer, ListProductTokenData, string> callback, int id, KeyProductId keyProduct, string keyInstanceClass)
    {
        ListProductTokenData tokenData = _tokenData.GetListToken(keyProduct);
        
        // if (_autoGenerateToken == true)
        // {
        //     tokenData.ListProductToken.Add(new ProductTokenData(Random.Range(1000, 100000).ToString()));
        // }
        
        callback.Invoke(id, keyProduct, _statusServer, tokenData, keyInstanceClass);
    }
}
