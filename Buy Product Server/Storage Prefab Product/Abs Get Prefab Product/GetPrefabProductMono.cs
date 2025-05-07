using System;
using UnityEngine;

public class GetPrefabProductMono : AbsGetPrefabProduct
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
    private StoragePrefabProductMono _storagePrefabProduct;
    
    public override DKOKeyAndTargetAction GetPrefabProduct(KeyProductId key)
    {
        return _storagePrefabProduct.GetPrefabProduct(key);
    }
}
