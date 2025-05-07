using System;
using UnityEngine;

public abstract class AbsBuyProduct : MonoBehaviour
{
    public abstract bool IsInit { get; }
    public abstract event Action OnInit;

    public abstract void BuyLogic(Action<int, KeyProductId, StatusCallBackServer, BuyProductData, string> callback, int id, KeyProductId keyProduct, string keyInstanceClass);

}
