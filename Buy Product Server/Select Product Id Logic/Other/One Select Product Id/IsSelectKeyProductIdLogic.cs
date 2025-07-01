using System;
using UnityEngine;
using UnityEngine.Serialization;

public class IsSelectKeyProductIdLogic : MonoBehaviour
{
   [SerializeField]
   private GetDKOPatch _patchStorageOneSelectProductId;
   private StorageOneSelectProductId _oneSelectProduct;
   
   [SerializeField] 
   private GetDataSO_KeyStorageSelectProductId _keyStorageSelectProductId;
   
       [SerializeField] 
       private DKOKeyAndTargetAction _dkoData;
       
        [SerializeField] 
       private LogicListTaskDKO _taskProductSelect;
       
       [SerializeField] 
       private LogicListTaskDKO _taskProductDontSelect;

       [SerializeField] 
       private AbsGetKeyProductId _absGetProductKey;
       
       private void Awake()
       {
          if (_patchStorageOneSelectProductId.Init == false)
          {
             _patchStorageOneSelectProductId.OnInit += OnInitStoragePanel;
             return;
          }

          GetDataDKO();
       }

       private void OnInitStoragePanel()
       {
          _patchStorageOneSelectProductId.OnInit -= OnInitStoragePanel;
          GetDataDKO();
       }

       private void GetDataDKO()
       {
          var DKOData = (DKODataInfoT<StorageOneSelectProductId>)_patchStorageOneSelectProductId.GetDKO();
          _oneSelectProduct = DKOData.Data;

          if (_oneSelectProduct.IsInit == false)
          {
             _oneSelectProduct.OnInit += OnInitSelectProduct;
             return;
          }

          InitSelectProduct();
       }

       private void OnInitSelectProduct()
       {
          _oneSelectProduct.OnInit -= OnInitSelectProduct;
          InitSelectProduct();
       }
       
       private void InitSelectProduct()
       {
          if (_oneSelectProduct.IsThereData(_keyStorageSelectProductId.GetData()) == false)
          {
             _oneSelectProduct.OnUpdateData += OnAddCheck;
          }
          else
          {
             AddCheck();
          }
       }

       private void OnAddCheck()
       {
          _oneSelectProduct.OnUpdateData -= OnAddCheck;
          AddCheck();
       }

       private void AddCheck()
       {
          _oneSelectProduct.GetSelectKeyProduct(_keyStorageSelectProductId.GetData()).OnUpdateData += Check;
          Check();
       }

       private void OnEnable()
       {
          if (_oneSelectProduct != null)
          {
             Check();   
          }
       }
    
       private void Check()
       {
          if (_oneSelectProduct.GetSelectKeyProduct(_keyStorageSelectProductId.GetData()).GetKey() != null) 
          {
             if (_oneSelectProduct.GetSelectKeyProduct(_keyStorageSelectProductId.GetData()).GetKey().GetKey() == _absGetProductKey.GetProductId().GetKey()) 
             {
                _taskProductSelect.StartAction(_dkoData);
                return;
             }
          }
          
          _taskProductDontSelect.StartAction(_dkoData);
       }


       private void OnDestroy()
       {
          _oneSelectProduct.OnUpdateData -= OnAddCheck;

          if (_oneSelectProduct.IsThereData(_keyStorageSelectProductId.GetData()) == true)
          {
             _oneSelectProduct.GetSelectKeyProduct(_keyStorageSelectProductId.GetData()).OnUpdateData -= Check;
          }

       }
}
