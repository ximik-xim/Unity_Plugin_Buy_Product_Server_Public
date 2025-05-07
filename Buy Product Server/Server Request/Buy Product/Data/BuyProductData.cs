using UnityEngine;

public class BuyProductData 
{
    public BuyProductData()
    {
        
    }
    
    public BuyProductData(bool productHaveBuy)
    {
        _productHaveBuy = productHaveBuy;
    }
    
    [SerializeField]
    private bool _productHaveBuy;

    public bool ProductHaveBuy => _productHaveBuy;
}
