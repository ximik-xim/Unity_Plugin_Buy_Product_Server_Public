using System;
using UnityEngine;

/// <summary>
/// Пример запроса синхроно(без ожидания ответа от сервера)
/// </summary>
public class CheckIsBuyProductExampleV1 : AbsCheckIsBuyProduct
{
    public override bool IsInit => true;
    public override event Action OnInit;

    [SerializeField] 
    private bool _isBuyProduct;

    [SerializeField] 
    private StatusCallBackServer _statusServer;
    
    private void Awake()
    {
        OnInit?.Invoke();
    }

    public override void CheckIsBuyLogic(Action<int, KeyProductId, StatusCallBackServer, CheckIsBuyProductData, string> callback, int id, KeyProductId keyProduct, string keyInstanceClass)
    {
        callback.Invoke(id, keyProduct, _statusServer, new CheckIsBuyProductData(_isBuyProduct), keyInstanceClass);
    }
}
