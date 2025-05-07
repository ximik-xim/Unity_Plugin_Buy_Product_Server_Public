using System;
using UnityEngine;

public class GetPrefabProductSO : AbsGetPrefabProduct
{
    public override bool IsInit => _storagePrefabProduct; 
    public override event Action OnInit
    {
        add
        {
            _storagePrefabProduct.OnInit += value;
        }

        remove
        {
            _storagePrefabProduct.OnInit -= value;
        }
    }

    [SerializeField]
    private StoragePrefabProductSO _storagePrefabProduct;
    
    public override DKOKeyAndTargetAction GetPrefabProduct(KeyProductId key)
    {
        return _storagePrefabProduct.GetPrefabProduct(key);
    }
}
