using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglProductErrorLogicOpenPanel : MonoBehaviour
{
    [SerializeField] 
    private SinglProduct _product;
    
    [SerializeField]
    private GetUIGamePanelInfoStorageInStorage _errorPanel;
    
    
    private void Awake()
    {
        if (_errorPanel.IsInit == false) 
        {
            _errorPanel.OnInit += OnInit;
            return;
        }

        Init();

    }

    private void OnInit()
    {
        _errorPanel.OnInit -= OnInit;
        Init();
    }

    private void Init()
    {
        _product.OnErrorBuy += OnErrorBuy;
    }

    private void OnErrorBuy()
    {
        _errorPanel.GetPanel().Open();
    }

    private void OnDestroy()
    {
        _product.OnErrorBuy -= OnErrorBuy;
    }
}
