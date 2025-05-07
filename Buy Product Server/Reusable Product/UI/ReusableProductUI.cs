using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReusableProductUI : MonoBehaviour
{
    [SerializeField] 
    private ReusableProduct _infoProductFloatUI;

    [SerializeField]
    private GameObject _panel;

    [SerializeField]
    private Text _text;
    [SerializeField] 
    private Button _buttonBuy;
    
    private void Awake()
    {
        if (_infoProductFloatUI.IsInit == false)
        {
            _infoProductFloatUI.OnInit += OnInit;
            return;
        }

        Init();
    }

    private void OnInit()
    {
        _infoProductFloatUI.OnInit -= OnInit;
        Init();
    }

    private void Init()
    {
        _infoProductFloatUI.GetStorageTask().OnUpdateStatus += OnUpdateStatusBlock;
        
        OnUpdateStatusBlock();
        OnUpdateCountTaskBlock();
        
        _buttonBuy.onClick.AddListener(ClickBuy);
    }
    
    private void CheckBlock()
    {
        if (_infoProductFloatUI.GetStorageTask().IsThereTasks() == true)
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

        CheckBlock();
        OnUpdateCountTaskBlock();
    }
    
    private void OnDestroy()
    {
        _buttonBuy.onClick.RemoveListener(ClickBuy);
        _infoProductFloatUI.GetStorageTask().OnUpdateStatus -= OnUpdateStatusBlock;
    }
}
