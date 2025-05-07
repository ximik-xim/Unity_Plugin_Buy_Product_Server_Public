using System;
using UnityEngine;

/// <summary>
/// Пример запроса синхроно(без ожидания ответа от сервера)
/// </summary>
public class RemoveProductTokenExampleV1 : AbsRemoveProductToken
{
    public override bool IsInit => true;
    public override event Action OnInit;

    [SerializeField] 
    private StatusCallBackServer _statusServer;
    
    private void Awake()
    {
        OnInit?.Invoke();
    }

    public override void RemoveProductToken(Action<int, KeyProductId, StatusCallBackServer, RemoveProductTokenData, string> callback, int id, KeyProductId keyProduct, string keyInstanceClass)
    {
        callback.Invoke(id, keyProduct, _statusServer, new RemoveProductTokenData(), keyInstanceClass);
    }
}
