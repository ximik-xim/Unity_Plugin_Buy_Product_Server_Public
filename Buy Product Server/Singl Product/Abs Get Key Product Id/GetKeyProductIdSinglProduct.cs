using System;
using UnityEngine;

public class GetKeyProductIdSinglProduct : AbsGetKeyProductId
{
    [SerializeField] 
    private SinglProduct _singlProduct;

    public override bool IsInit => _singlProduct.IsInit;
    public override event Action OnInit
    {
        add
        {
            _singlProduct.OnInit += value;
        }

        remove
        {
            _singlProduct.OnInit -= value;
        }
    }
    public override KeyProductId GetProductId()
    {
        return _singlProduct.GetProductId();
    }
}
