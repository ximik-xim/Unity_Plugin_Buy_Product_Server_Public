
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIStorageMoney : MonoBehaviour
{
    [SerializeField] 
    private Text _text;
    
    [SerializeField] 
    private GetDKOPatch _patchStorageMoney;
    private StorageMoney _storageMoney;
    
    private void Awake()
    {
        if (_patchStorageMoney.Init == false)
        {
            _patchStorageMoney.OnInit += OnInitStoragePanel;
        }

        CheckInit();
    }

    private void OnInitStoragePanel()
    {
        _patchStorageMoney.OnInit -= OnInitStoragePanel;
        CheckInit();
    }

    private void CheckInit()
    {
        if (_patchStorageMoney.Init == true)
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
        _storageMoney.OnUpdateCount += OnUpdateValue;
        OnUpdateValue();
    }

    private void OnUpdateValue()
    {
        float hpValue = _storageMoney.GetCountMoney();
        
        _text.text = hpValue.ToString();
    }

    private void OnDestroy()
    {
        _storageMoney.OnUpdateCount -= OnUpdateValue;
    }
}
