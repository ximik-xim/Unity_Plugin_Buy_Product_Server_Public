using UnityEngine;
using UnityEngine.UI;

public class UIGetPriceSinglProduct : MonoBehaviour
{
    [SerializeField]
    private SinglProduct _singleProduct;
    
    [SerializeField]
    private AbsGetPriceProductId _getPriceProductId;
    
    [SerializeField] 
    private Text _text;
    
    private void Awake()
    {
        if (_singleProduct.IsInit == false)
        {
            _singleProduct.OnInit += OnInitSingleProduct;
        }

        if (_getPriceProductId.IsInit == false)
        {
            _getPriceProductId.OnInit += OnInitGetPriceProductId;
        }
        
        CheckInit();
    }

    private void OnInitSingleProduct()
    {
        _singleProduct.OnInit -= OnInitSingleProduct;
        
        CheckInit();
    }
    
    private void OnInitGetPriceProductId()
    {
        _getPriceProductId.OnInit -= OnInitGetPriceProductId;
        CheckInit();
    }

    
    private void CheckInit()
    {
        if (_singleProduct.IsInit == true && _getPriceProductId.IsInit == true)  
        {
            Init();
        }   
    }
    
    private void Init()
    {
       _text.text = _getPriceProductId.GetPriceProduct(_singleProduct.GetProductId()).ToString();
    }

    
}
