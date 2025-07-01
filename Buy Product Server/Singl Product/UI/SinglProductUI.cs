using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SinglProductUI : MonoBehaviour
{
    [SerializeField] 
    private SinglProduct _infoProductFloatUI;

    [SerializeField]
    private GameObject _panel;

    [SerializeField]
    private Text _text;
    [SerializeField] 
    private Button _buttonBuy;
    
    [SerializeField]
    private SinglProduct _singleProduct;
    
    [SerializeField] 
    private GetDKOPatch _patchStorageMoney;
    private StorageMoney _storageMoney;
    
    [SerializeField]
    private AbsGetPriceProductId _storagePriceProductIdSO;
    
    private void Awake()
    {
        if (_singleProduct.IsInit == false)
        {
            _singleProduct.OnInit += OnInitSingleProduct;
        }
        
        if (_patchStorageMoney.Init == false)
        {
            _patchStorageMoney.OnInit += OnInitPatchStorageMoney;
        }

        if (_storagePriceProductIdSO.IsInit == false)
        {
            _storagePriceProductIdSO.OnInit += OnInitGetPriceProductId;
        }
        
        if (_infoProductFloatUI.IsInit == false)
        {
            _infoProductFloatUI.OnInit += OnInitInfoProductFloatUI;
            return;
        }
        
        CheckInit();
    }

    
    private void OnInitSingleProduct()
    {
        _singleProduct.OnInit -= OnInitSingleProduct;
        
        CheckInit();
    }
    
    private void OnInitPatchStorageMoney()
    {
        _patchStorageMoney.OnInit -= OnInitPatchStorageMoney;
        
        CheckInit();
    }
    
    private void OnInitGetPriceProductId()
    {
        _storagePriceProductIdSO.OnInit -= OnInitGetPriceProductId;
        CheckInit();
    }

    private void OnInitInfoProductFloatUI()
    {
        _infoProductFloatUI.OnInit -= OnInitInfoProductFloatUI;
        CheckInit();
    }
    
    private void CheckInit()
    {
        if (_patchStorageMoney.Init == true && _storagePriceProductIdSO.IsInit == true && _singleProduct.IsInit == true && _infoProductFloatUI.IsInit == true)  
        {
            var DKOData = (DKODataInfoT<StorageMoney>)_patchStorageMoney.GetDKO();
            _storageMoney = DKOData.Data;

            if (_storageMoney.IsInit == false)
            {
                _storageMoney.OnInit += OnInitStorageMoney;
                return;
            }

            InitStorageMoney();
        }
    }

    private void OnInitStorageMoney()
    {
        _storageMoney.OnInit -= OnInitStorageMoney;
        InitStorageMoney();
    }
    
    private void InitStorageMoney()
    {
        Init();
    }


    private void Init()
    {
        _infoProductFloatUI.GetStorageTask().OnUpdateStatus += OnUpdateStatusBlock;
        
        OnUpdateStatusBlock();
        OnUpdateCountTaskBlock();
        
        _buttonBuy.onClick.AddListener(ClickBuy);
        
        _storageMoney.OnUpdateCount += OnUpdateCountMoney;
        OnUpdateCountMoney();
    }

    private void OnUpdateCountMoney()
    {
        if (_infoProductFloatUI.GetStorageTask().IsThereTasks() == true)
        {
            _buttonBuy.interactable = false;
        }
        else if (_storageMoney.GetCountMoney() < _storagePriceProductIdSO.GetPriceProduct(_singleProduct.GetProductId()))
        {
            _buttonBuy.interactable = false;
        }
        else
        {
            _buttonBuy.interactable = true;
        }
    }

    private void ClickBuy()
    {
        _infoProductFloatUI.BuyProduct();
    }

    private void OnUpdateCountTaskBlock()
    {
        _text.text = "Товар заблокирован" + '\n';
        foreach (var VARIABLE in _infoProductFloatUI.GetStorageTask().GetTextAllTaskInfo())
        {
            _text.text += VARIABLE + '\n';
        }
    }

    private void OnUpdateStatusBlock()
    {
        if (_infoProductFloatUI.GetStorageTask().IsThereTasks() == true)
        {
            _panel.SetActive(true);
        }
        else
        {
            _panel.SetActive(false);
        }

        OnUpdateCountMoney();
        OnUpdateCountTaskBlock();
    }
    
    private void OnDestroy()
    {
        _storageMoney.OnUpdateCount -= OnUpdateCountMoney;
        
        _buttonBuy.onClick.RemoveListener(ClickBuy);
        _infoProductFloatUI.GetStorageTask().OnUpdateStatus -= OnUpdateStatusBlock;
    }
}
