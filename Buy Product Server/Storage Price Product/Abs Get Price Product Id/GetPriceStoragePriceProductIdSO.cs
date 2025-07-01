using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GetPriceStoragePriceProductIdSO : AbsGetPriceProductId
{
    [SerializeField] 
    private StoragePriceProductIdSO _storagePriceProduct;

    /// <summary>
    /// Если нет ценника в хранилеще с цениками, значит товар бесплатный(вернет стоймость = 0)
    /// </summary>
    [SerializeField]
    private bool _useFreeItemNoKey;
    
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
        if (_useFreeItemNoKey == true) 
        {
            if (_storagePriceProduct.IsThereKey(key) == false)
            {
                Debug.Log($"ВНИМАНИЕ! Ключ товара {key.GetKey()} не был найден в хранилище с ценниками. А значит товар бесплатны(его цена = 0)");

                return 0;
            }
        }
        
        return _storagePriceProduct.GetPriceProduct(key);
    }
}
