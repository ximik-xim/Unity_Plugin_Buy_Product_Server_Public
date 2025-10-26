using System;
using UnityEngine;

/// <summary>
/// Нужна что бы можно было получить информацию об стоймости продукта с разных мест
/// </summary>
public abstract class AbsGetPriceProductId : MonoBehaviour
{
    public abstract event Action OnInit;
    public abstract bool IsInit { get; }

    public abstract float GetPriceProduct(KeyProductId key);

}
