using UnityEngine;
using System;

public abstract class AbsGetProductTokens : MonoBehaviour
{
    public abstract bool IsInit { get; }
    public abstract event Action OnInit;

    public abstract void GetProductTokens(Action<int, KeyProductId, StatusCallBackServer, ListProductTokenData, string> callback, int id, KeyProductId keyProduct, string keyInstanceClass);

}
