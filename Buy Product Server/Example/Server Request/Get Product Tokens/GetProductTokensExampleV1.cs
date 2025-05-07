using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Пример запроса синхроно(без ожидания ответа от сервера)
/// </summary>
public class GetProductTokensExampleV1 : AbsGetProductTokens
{
    public override bool IsInit => true;
    public override event Action OnInit;

    [SerializeField] 
    private List<ProductTokenData> _productToken;

    [SerializeField] 
    private StatusCallBackServer _statusServer;
    
    private void Awake()
    {
        OnInit?.Invoke();
    }

    public override void GetProductTokens(Action<int, KeyProductId, StatusCallBackServer, ListProductTokenData, string> callback, int id, KeyProductId keyProduct, string keyInstanceClass)
    {
        callback.Invoke(id, keyProduct, _statusServer, new ListProductTokenData(_productToken), keyInstanceClass);
    }
}
