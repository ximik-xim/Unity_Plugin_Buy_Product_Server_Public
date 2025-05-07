using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GetPriceStoragePriceProductIdSO : AbsGetPriceProductId
{
    [SerializeField] 
    private StoragePriceProductIdSO _storagePriceProduct;

    public override event Action OnInit
    {
        add
        {
            _storagePriceProduct.OnInit += value;
        }
        
        remove
        {
            _storagePriceProduct.OnInit -= value;    
        }
    }
    public override bool IsInit => _storagePriceProduct.IsInit;
    public override float GetPriceProduct(KeyProductId key)
    {
        return _storagePriceProduct.GetPriceProduct(key);
    }
}
