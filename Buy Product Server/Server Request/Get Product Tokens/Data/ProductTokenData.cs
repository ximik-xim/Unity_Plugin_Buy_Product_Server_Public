using UnityEngine;
[System.Serializable]
public class ProductTokenData 
{
    public ProductTokenData()
    {
        
    }

    public ProductTokenData(string productToken)
    {
        _productToken = productToken;
    }
    
    [SerializeField] 
    private string _productToken;
    
    public string ProductToken => _productToken;
}
