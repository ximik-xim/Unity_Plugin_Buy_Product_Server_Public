using System;
using UnityEngine;

/// <summary>
/// Пример запроса синхроно(без ожидания ответа от сервера)
/// </summary>
public class RemoveProductTokenExampleV1 : AbsRemoveProductToken
{
    public override bool IsInit => _isInit;
    private bool _isInit = false;
    public override event Action OnInit;

    [SerializeField] 
    private ExampleStorageTokens _tokenData;

    [SerializeField] 
    private StatusCallBackServer _statusServer;
    
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

    public override void RemoveProductToken(Action<int, KeyProductId, StatusCallBackServer, RemoveProductTokenData, string> callback, int id, KeyProductId keyProduct, string keyInstanceClass)
    {
        _tokenData.DeletedToken(keyProduct);
        
        callback.Invoke(id, keyProduct, _statusServer, new RemoveProductTokenData(), keyInstanceClass);
    }
}
