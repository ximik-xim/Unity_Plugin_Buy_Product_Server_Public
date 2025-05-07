using System;
using UnityEngine;

public abstract class AbsGetPriceProductId : MonoBehaviour
{
    public abstract event Action OnInit;
    public abstract bool IsInit { get; }

    public abstract float GetPriceProduct(KeyProductId key);

}
