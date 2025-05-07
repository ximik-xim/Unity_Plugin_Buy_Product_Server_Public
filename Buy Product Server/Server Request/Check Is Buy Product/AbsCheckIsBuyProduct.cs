using System;
using UnityEngine;

public abstract class AbsCheckIsBuyProduct : MonoBehaviour
{
    public abstract bool IsInit { get; }
    public abstract event Action OnInit;

    public abstract void CheckIsBuyLogic(Action<int, KeyProductId, StatusCallBackServer, CheckIsBuyProductData, string> callback, int id, KeyProductId keyProduct, string keyInstanceClass);
}
