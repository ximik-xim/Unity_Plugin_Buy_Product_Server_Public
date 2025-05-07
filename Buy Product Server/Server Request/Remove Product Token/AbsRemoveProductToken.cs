using System;
using UnityEngine;

public abstract class AbsRemoveProductToken : MonoBehaviour
{
    public abstract bool IsInit { get; }
    public abstract event Action OnInit;

    public abstract void RemoveProductToken(Action<int, KeyProductId, StatusCallBackServer, RemoveProductTokenData, string> callback, int id, KeyProductId keyProduct, string keyInstanceClass);

}
