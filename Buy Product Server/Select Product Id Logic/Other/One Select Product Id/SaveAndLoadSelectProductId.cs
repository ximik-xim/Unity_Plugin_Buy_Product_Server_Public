using System;
using UnityEngine;
using UnityEngine.Serialization;

public class SaveAndLoadSelectProductId : MonoBehaviour
{
   [SerializeField] 
   private GetDKOPatch _patchStorageOneSelectProductId;
   private StorageOneSelectProductId _oneSelectProduct;
   
   [SerializeField] 
   private GetDataSO_KeyStorageSelectProductId _keyStorageSelectProductId;

   [SerializeField] 
   private SD_StorageDataStringPrefs _storageSaveData;
   [SerializeField]
   private GetDataSO_SD_KeyStorageStringVariable _keySaveData;

   private void Awake()
   {
      if (_patchStorageOneSelectProductId.Init == false)
      {
         _patchStorageOneSelectProductId.OnInit += OnInitStoragePanel;
      }

      if (_storageSaveData.IsInit == false)
      {
         _storageSaveData.OnInit += OnInitStorageSaveData;
      }

      CheckInit();
   }

   private void OnInitStorageSaveData()
   {
      _storageSaveData.OnInit -= OnInitStorageSaveData;
      CheckInit();
   }


   private void OnInitStoragePanel()
   {
      _patchStorageOneSelectProductId.OnInit -= OnInitStoragePanel;
      CheckInit();
   }

   private void CheckInit()
   {
      if (_patchStorageOneSelectProductId.Init == true && _storageSaveData.IsInit == true)
      {
         var DKOData = (DKODataInfoT<StorageOneSelectProductId>)_patchStorageOneSelectProductId.GetDKO();
         _oneSelectProduct = DKOData.Data;

         if (_oneSelectProduct.IsThereData(_keyStorageSelectProductId.GetData()) == false)
         {
            _oneSelectProduct.AddSelectKeyProduct(_keyStorageSelectProductId.GetData(), new StorageOneSelectProductIdData(null));
         }
         
         GetSaveData();
        
         
         var data = _oneSelectProduct.GetSelectKeyProduct(_keyStorageSelectProductId.GetData());
         data.OnUpdateData += OnSaveData;
      }
   }


   private void OnSaveData()
   {
      var key = _oneSelectProduct.GetSelectKeyProduct(_keyStorageSelectProductId.GetData());

      _storageSaveData.SetData(_keySaveData.GetData(), key.GetKey().GetKey());
      _storageSaveData.SaveData(new TaskInfo("text"));
   }

   private void GetSaveData()
   {
      var key = _storageSaveData.GetData(_keySaveData.GetData());
      
      KeyProductId keyProductId = null;
     
      if (key != "" && key != " " && key != String.Empty)
      {
         keyProductId = new KeyProductId(key);
      }
      
      if (_oneSelectProduct.IsThereData(_keyStorageSelectProductId.GetData()) == true)
      {
         var data = _oneSelectProduct.GetSelectKeyProduct(_keyStorageSelectProductId.GetData());
         data.SetKey(keyProductId);
      }
      else
      {
         _oneSelectProduct.AddSelectKeyProduct(_keyStorageSelectProductId.GetData(), new StorageOneSelectProductIdData(keyProductId));
      }
     
   }
   
   private void OnDestroy()
   {
      var data = _oneSelectProduct.GetSelectKeyProduct(_keyStorageSelectProductId.GetData());
      data.OnUpdateData -= OnSaveData;
   }
}
