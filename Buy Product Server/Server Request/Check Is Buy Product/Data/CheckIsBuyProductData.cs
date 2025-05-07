using UnityEngine;

public class  CheckIsBuyProductData
{
    public CheckIsBuyProductData()
    {

    }

    public CheckIsBuyProductData(bool isBuyProduct)
    {
        _isBuyProduct = isBuyProduct;
    }
    
    [SerializeField]
    private bool _isBuyProduct;

    public bool IsBuyProduct => _isBuyProduct;
}
