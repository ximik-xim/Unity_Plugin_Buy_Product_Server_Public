using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ButtonSelectCurrentProduct : MonoBehaviour
{
   [SerializeField] 
   private GetDKOPatch _patchStorageOneSelectProductId;
   private StorageOneSelectProductId _oneSelectProduct;

   [SerializeField] 
   private AbsGetKeyProductId _absGetProductKey;
   
   [SerializeField] 
   private GetDataSO_KeyStorageSelectProductId _keyStorageSelectProductId;

   [SerializeField] 
   private Button _button;

   /// <summary>
   /// Создавать ли автоматически Хранилеще с выбр. ключами(если его нет)
   /// </summary>
   [SerializeField] 
   private bool _autoCrateStorageSelect = false;
   
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

      _button.onClick.AddListener(ButtonClick);
   }

   private void ButtonClick()
   {
      if (_autoCrateStorageSelect == true)
      {
         if (_oneSelectProduct.IsThereData(_keyStorageSelectProductId.GetData()) == false)
         {
            _oneSelectProduct.AddSelectKeyProduct(_keyStorageSelectProductId.GetData(), new StorageOneSelectProductIdData(_absGetProductKey.GetProductId()));
            return;
         }
      }
      
      _oneSelectProduct.GetSelectKeyProduct(_keyStorageSelectProductId.GetData()).SetKey(_absGetProductKey.GetProductId());
   }

   private void OnDestroy()
   {
      _button.onClick.RemoveListener(ButtonClick);
   }
}
