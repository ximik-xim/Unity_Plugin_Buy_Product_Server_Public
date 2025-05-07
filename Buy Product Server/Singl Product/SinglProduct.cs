using System;
using UnityEngine;
using UnityEngine.Serialization;

public class SinglProduct : MonoBehaviour
{
    //Нужно в ошибке пометить, что если товар был куплен но не отобразился в игре из за плохого соединения, нажмите еще раз на кнопку покупки
   public event Action OnErrorBuy;

   /// <summary>
   /// Сработает когда будут изменны данные об прадукте в хранилеще
   /// </summary>
   public event Action OnUpdateCurrentProductData;

   [SerializeField]
   private GetDKOPatch _patchStorageKey;
   [SerializeField]
   private GetDataSO_TSG_KeyStorageTask _keyStorageTaskBlock;
    private TSG_StorageTaskDefaultData _taskBlockStorage;

   [SerializeField] 
   private GetDKOPatch _patchDKOBuyProduct;
   private BuyProductWrapper _productBuy;
   private GetServerRequestData<BuyProductData> _productBuyData;
   
   [SerializeField] 
   private GetDKOPatch _patchDKOCheckProductHaveBuy;
   private CheckIsBuyProductWrapper _checkProductHaveBuy;
   private GetServerRequestData<CheckIsBuyProductData> _checkProductHaveBuyData;
   
   [SerializeField] 
   private GetDataSODataProductId _productId;

   [SerializeField] 
   private LogicListTaskDKO _logicBuyProductComlited;
   [SerializeField] 
   private DKOKeyAndTargetAction _dkoLogicComplited;

   [SerializeField] 
   private AbsSaveDataSinglProduct _patchDataProduct;
   
   [SerializeField] 
   private GetDataSO_TSG_KeyTaskData _keyBlockBuyProcessing;
   [SerializeField] 
   private string _textTaskBlockBuyProcessing = "Идет обработка";
   
   [SerializeField] 
   private GetDataSO_TSG_KeyTaskData _keyBlockProductHaveBuy;
   [SerializeField] 
   private string _textTaskBlockBuy = "Товар уже кулпен";
   
   [SerializeField] 
   private TypeSaveProduct _typeSaveLogicComplited;
   
   private bool _init = false;
   public bool IsInit => _init;
   public event Action OnInit;
   
   private bool _isStartLogic = false;
       
   /// <summary>
   /// Сработает когда запустить метод для начало покупки(начнеться проверка, есть ли необработанные покупки)
   /// </summary>
   public event Action OnStart;
       
   /// <summary>
   /// Сработает в случае error или в случае если обработка всех покупок завершина
   /// </summary>
   public event Action OnComlited;
   
   ////////////////////////////////////////////// Инициализация ///////////////////////////////////////////////////////////////////
#if UNITY_EDITOR

   [SerializeField]
   private bool _isDebug;
#endif
   
   private void Awake()
   {
#if UNITY_EDITOR
      if (_isDebug == true)
      {
         Debug.Log(gameObject.name + " SP 0");
      }
#endif
      
      if (_patchDKOBuyProduct.Init == false)
      {
         _patchDKOBuyProduct.OnInit += OnInitBuyProduct;
      } 
      
      if (_patchDKOCheckProductHaveBuy.Init == false)
      {
         _patchDKOCheckProductHaveBuy.OnInit += OnInitProductHaveBuy;
      } 
      
      if (_logicBuyProductComlited.IsInit == false)
      {
         _logicBuyProductComlited.OnInit += OnInitLogicBuy;
      } 
            
      if (_patchDataProduct.IsInit() == false)
      {
         _patchDataProduct.OnInit += OnInitPatchDataProduct;
      }

      if (_patchStorageKey.Init == false)
      {
         _patchStorageKey.OnInit += OnInitPatchStorageKey;
      }

      OnInitGeneral();
   }

   private void OnInitBuyProduct()
   {
      _patchDKOBuyProduct.OnInit -= OnInitBuyProduct;
      OnInitGeneral();
   }

   private void OnInitProductHaveBuy()
   {
      _patchDKOCheckProductHaveBuy.OnInit -= OnInitProductHaveBuy;
      OnInitGeneral();
   }
   
   private void OnInitLogicBuy()
   {
      _logicBuyProductComlited.OnInit -= OnInitLogicBuy;
      OnInitGeneral();
   }
   
   private void OnInitPatchDataProduct()
   {
      _patchDataProduct.OnInit -= OnInitPatchDataProduct;
      OnInitGeneral();
   }
   
   private void OnInitPatchStorageKey()
   {
      _patchStorageKey.OnInit -= OnInitPatchStorageKey;
      OnInitGeneral();
   }
   
   private void OnInitGeneral()
   {
      if (_patchDKOBuyProduct.Init == true && _patchDKOCheckProductHaveBuy.Init == true && _logicBuyProductComlited.IsInit == true && _patchDataProduct.IsInit() == true && _patchStorageKey.Init == true) 
      {  
#if UNITY_EDITOR
         if (_isDebug == true)
         {
            Debug.Log(gameObject.name + " SP 10");
         }
#endif
         
         var dkoProductBuy = (DKODataInfoT<BuyProductWrapper>)_patchDKOBuyProduct.GetDKO();
         _productBuy = dkoProductBuy.Data;

         var dkoCheckProductHaveBuy = (DKODataInfoT<CheckIsBuyProductWrapper>)_patchDKOCheckProductHaveBuy.GetDKO();
         _checkProductHaveBuy = dkoCheckProductHaveBuy.Data;
         
         var dkoStorageKeyTaskDataMono= (DKODataInfoT<TSG_StorageKeyTaskDataMono>) _patchStorageKey.GetDKO();
         dkoStorageKeyTaskDataMono.Data.AddTaskData(_keyStorageTaskBlock.GetData(), new TSG_StorageTaskDefaultData());
         _taskBlockStorage = dkoStorageKeyTaskDataMono.Data.GetTaskData(_keyStorageTaskBlock.GetData());
         
         SetKeyProductBuyProcessing();
         
         if (_productBuy.IsInit == false)
         {
#if UNITY_EDITOR
            if (_isDebug == true)
            {
               Debug.Log(gameObject.name + " SP 11");
            }
#endif
            _productBuy.OnInit += OnInitProductBuy;
         }
         else
         {
#if UNITY_EDITOR
            if (_isDebug == true)
            {
               Debug.Log(gameObject.name + " SP 12");
            }
#endif
            Init();
         }
      }
   }

   private void OnUpdateCurrentValue()
   {
      OnUpdateCurrentProductData?.Invoke();
   }

   private void OnUpdateDataPatchProduct()
   {
      if (_init == false)
      {
         //тут отписки 
            
            _checkProductHaveBuyData.OnGetDataCompleted -= OnOnInitCheckProductHaveBuyV2;
            
         _checkProductHaveBuy.OnUpdateStatusBlock -= CheckProductHaveBuyOnComplitedInit;
         _logicBuyProductComlited.OnCompleted -= OnInitComplitedLogicBuyProduct;
         _patchDataProduct.OnSaveData -= OnSaveDataStatusProductInit;
         

         InitLogicProduct();
      }
      else
      {
         if (_isStartLogic == true) 
         {
            //тут отписки 
            
            _productBuyData.OnGetDataCompleted -= OnCallbackBuyProductV2;
             
            _checkProductHaveBuyData.OnGetDataCompleted -= OnOnCheckProductHaveBuyLastBuyV2;
            _checkProductHaveBuyData.OnGetDataCompleted -= OnOnCheckProductHaveBuyStartBuyV2;
            
            _checkProductHaveBuy.OnUpdateStatusBlock -= CheckProductHaveBuyOnComplited;
            _logicBuyProductComlited.OnCompleted -= OnComplitedLogicBuyProduct;
  
            _checkProductHaveBuy.OnUpdateStatusBlock -= CheckProductHaveBuyOnComplited2;
            
            _patchDataProduct.OnSaveData -= OnSaveDataStatusProduct;
            
            //тут отписки и добавление индификаторов
            ErrorLogic();
         }
         else
         {
            //Debug.Log("SSSS P204");
            if (_patchDataProduct.IsBuyProduct() == true)
            {
               SetKeyProductBuy();
            }
            else
            {
               RemoveKeyProductBuy();
            }
         }
      }
   }

   private void OnInitProductBuy()
   {
      //Debug.Log("SSSS P12");
      _productBuy.OnInit -= OnInitProductBuy;
      Init();
   }


   private void Init()
   {
#if UNITY_EDITOR
      if (_isDebug == true)
      {
         Debug.Log(gameObject.name + " SP 20");
      }
#endif
      
      //Debug.Log("SSSS P14");
      _patchDataProduct.OnUpdateCurrentValue += OnUpdateCurrentValue;
      _patchDataProduct.OnUpdateStorageDataData += OnUpdateDataPatchProduct;
      InitLogicProduct();
   }
   
   private void InitLogicProduct()
   {
#if UNITY_EDITOR
      if (_isDebug == true)
      {
         Debug.Log(gameObject.name + " SP 25");
      }
#endif
      
      if (_patchDataProduct.IsBuyProduct() == false) 
      {
         
#if UNITY_EDITOR
         if (_isDebug == true)
         {
            Debug.Log(gameObject.name + " SP 26");
         }
#endif
         RemoveKeyProductBuy();
         
         CheckProductHaveBuyInit();

      }
      else
      {
#if UNITY_EDITOR
         if (_isDebug == true)
         {
            Debug.Log(gameObject.name + " SP 27");
         }
#endif
         SetKeyProductBuy();

         InitLogic();
      }
   }
   
   private void InitLogic()
   {
#if UNITY_EDITOR
      if (_isDebug == true)
      {
         Debug.Log(gameObject.name + " SP 50 INIT");
      }
#endif
      RemoveKeyProductBuyProcessing();
      _init = true;
      OnInit?.Invoke();
   }
   
   private void CheckProductHaveBuyInit()
   {
      if (_checkProductHaveBuy.IsBlock == false)
      {
         if (_checkProductHaveBuyData != null)
         {
            _checkProductHaveBuyData.OnGetDataCompleted -= OnOnInitCheckProductHaveBuyV2;
         }
         
         _checkProductHaveBuyData = _checkProductHaveBuy.CheckProductHaveBuy(_productId.GetData());
         if (_checkProductHaveBuyData.IsGetDataCompleted == false)
         {
            _checkProductHaveBuyData.OnGetDataCompleted += OnOnInitCheckProductHaveBuyV2;
         }
         else
         {
            OnInitCheckProductHaveBuyV2();
         }
         
         return;
      }
      
      //Debug.Log("SSSS P20");
      _checkProductHaveBuy.OnUpdateStatusBlock -= CheckProductHaveBuyOnComplitedInit;
      _checkProductHaveBuy.OnUpdateStatusBlock += CheckProductHaveBuyOnComplitedInit;
          
   }

   private void CheckProductHaveBuyOnComplitedInit()
   {
      if (_checkProductHaveBuy.IsBlock == false)
      {
         if (_checkProductHaveBuyData != null)
         {
            _checkProductHaveBuyData.OnGetDataCompleted -= OnOnInitCheckProductHaveBuyV2;
         }
         
         _checkProductHaveBuy.OnUpdateStatusBlock -= CheckProductHaveBuyOnComplitedInit;

         _checkProductHaveBuyData = _checkProductHaveBuy.CheckProductHaveBuy(_productId.GetData());
         if (_checkProductHaveBuyData.IsGetDataCompleted == false)
         {
            _checkProductHaveBuyData.OnGetDataCompleted += OnOnInitCheckProductHaveBuyV2;
         }
         else
         {
            OnInitCheckProductHaveBuyV2();
         }
      }
      
   }

   private void OnOnInitCheckProductHaveBuyV2()
   {
      _checkProductHaveBuyData.OnGetDataCompleted -= OnOnInitCheckProductHaveBuyV2;
      OnInitCheckProductHaveBuyV2();
   }

   private void OnInitCheckProductHaveBuyV2()
   {
         if (_checkProductHaveBuyData.StatusServer == StatusCallBackServer.Ok)
         {
            if (_checkProductHaveBuyData.GetData.IsBuyProduct == true)
            {
               _logicBuyProductComlited.OnCompleted += OnInitComplitedLogicBuyProduct;
            
               _logicBuyProductComlited.StartAction(_dkoLogicComplited);
            }
            else
            {
               InitLogic();
            }
         
         }
         else
         {
            CheckProductHaveBuyInit();
         }
         
   }
   
   private void OnInitComplitedLogicBuyProduct()
   {
      //Debug.Log("SSSS P27");
      if (_logicBuyProductComlited.IsCompleted == true)
      {
         //Debug.Log("SSSS P28");
         _logicBuyProductComlited.OnCompleted -= OnInitComplitedLogicBuyProduct;
         
         _patchDataProduct.SetStatusBuyProduct();

         SetKeyProductBuy();
         
         switch (_typeSaveLogicComplited)
         {
            case TypeSaveProduct.NoSave:
            {   
               //Debug.Log("SSSS P29");
                    InitLogic();
            }
               break;
                
            case TypeSaveProduct.DefaultSave:
            {
               //Debug.Log("SSSS P30");
               _patchDataProduct.OnSaveData += OnSaveDataStatusProductInit;
               _patchDataProduct.SaveData(new TaskInfo("dd"));
            }
               break;
                
            case TypeSaveProduct.UrgentSave:
            {
               //Debug.Log("SSSS P31");
               _patchDataProduct.OnSaveData += OnSaveDataStatusProductInit;
               _patchDataProduct.SaveData(new TaskInfo("dd"),true);
            }
               break;
                
         }
      }
   }
   
   private void OnSaveDataStatusProductInit()
   {
      //Debug.Log("SSSS P32");
      if (_patchDataProduct.LastStatusSaveData == StatusStorageAction.Ok) 
      {
         //Debug.Log("SSSS P33");
         _patchDataProduct.OnSaveData -= OnSaveDataStatusProductInit;

         InitLogic();
      }
   }
   
   
   ////////////////////////////////////////////// Инициализация ///////////////////////////////////////////////////////////////////
   
   public void BuyProduct()
   {
#if UNITY_EDITOR
      if (_isDebug == true)
      {
         Debug.Log(gameObject.name + " SP 1000");
      }
#endif
      if (IsStart() == true)
      {
         //Если нету записи об покупки в хранилеще(или товар тупо не был куплен)
         if (_patchDataProduct.IsBuyProduct() == false)
         {
#if UNITY_EDITOR
            if (_isDebug == true)
            {
               Debug.Log(gameObject.name + " SP 1100");
            }
#endif
            _isStartLogic = true;
            OnStart?.Invoke();
            SetKeyProductBuyProcessing();
            RemoveKeyProductBuy();

            CheckProductHaveBuy();
         }
         else
         {
#if UNITY_EDITOR
            if (_isDebug == true)
            {
               Debug.Log(gameObject.name + " SP 1200");
            }
#endif
            SetKeyProductBuy();
         }
         
      }
      
   }

   private bool IsStart()
   {
      if (_taskBlockStorage.IsThereTasks() == false)
      {
         if (_productBuy.IsBlock == false)
         {
               return true;
         }
      }

      return false;
   }

   private void CheckProductHaveBuy()
   {
      if (_checkProductHaveBuy.IsBlock == false)
      {
         if (_checkProductHaveBuyData != null)
         {
            _checkProductHaveBuyData.OnGetDataCompleted -= OnOnCheckProductHaveBuyLastBuyV2;
         }
         
         _checkProductHaveBuyData = _checkProductHaveBuy.CheckProductHaveBuy(_productId.GetData());
         if (_checkProductHaveBuyData.IsGetDataCompleted == false)
         {
            _checkProductHaveBuyData.OnGetDataCompleted += OnOnCheckProductHaveBuyLastBuyV2;
         }
         else
         {
            OnCheckProductHaveBuyLastBuyV2();
         }
         
         return;
      }
      
      _checkProductHaveBuy.OnUpdateStatusBlock -= CheckProductHaveBuyOnComplited;
      _checkProductHaveBuy.OnUpdateStatusBlock += CheckProductHaveBuyOnComplited;
   }

   private void CheckProductHaveBuyOnComplited()
   {
      if (_checkProductHaveBuy.IsBlock == false)
      {
         if (_checkProductHaveBuyData != null)
         {
            _checkProductHaveBuyData.OnGetDataCompleted -= OnOnCheckProductHaveBuyLastBuyV2;
         }
         
         _checkProductHaveBuy.OnUpdateStatusBlock -= CheckProductHaveBuyOnComplited;
         
         _checkProductHaveBuyData = _checkProductHaveBuy.CheckProductHaveBuy(_productId.GetData());
         if (_checkProductHaveBuyData.IsGetDataCompleted == false)
         {
            _checkProductHaveBuyData.OnGetDataCompleted += OnOnCheckProductHaveBuyLastBuyV2;
         }
         else
         {
            OnCheckProductHaveBuyLastBuyV2();
         }
         
         return;
      }
   }
   

   private void OnOnCheckProductHaveBuyLastBuyV2()
   {
      _checkProductHaveBuyData.OnGetDataCompleted -= OnOnCheckProductHaveBuyLastBuyV2;
      OnCheckProductHaveBuyLastBuyV2();
   }

   private void OnCheckProductHaveBuyLastBuyV2()
   {
#if UNITY_EDITOR
      if (_isDebug == true)
      {
         Debug.Log(gameObject.name + " SP 1400");
      }
#endif
         if (_checkProductHaveBuyData.StatusServer == StatusCallBackServer.Ok)
         {
            //Но есть запись об покупке в списке yandex(значит логка не отработала и данные не были внесены)
            if (_checkProductHaveBuyData.GetData.IsBuyProduct == true)
            {
               _logicBuyProductComlited.OnCompleted -= OnComplitedLogicBuyProduct;
               _logicBuyProductComlited.OnCompleted += OnComplitedLogicBuyProduct;
               //Запускаю  логику и жду Callback
               _logicBuyProductComlited.StartAction(_dkoLogicComplited);
            
#if UNITY_EDITOR
               if (_isDebug == true)
               {
                  Debug.Log(gameObject.name + " SP 1500");
               }
#endif
            }
            else
            {
#if UNITY_EDITOR
               if (_isDebug == true)
               {
                  Debug.Log(gameObject.name + " SP 1600");
               }
#endif
               //Иначе, тупо начинаю покупку
               BuyProductLogic();
            }
         
         }
         else
         {
            ErrorLogic();
         }
      }      
   
   
   private void BuyProductLogic()
   {
      if (_productBuy.IsProductStartWating(_productId.GetData()) == false)
      {
         _productBuyData = _productBuy.BuyProduct(_productId.GetData());
         if (_productBuyData.IsGetDataCompleted == false)
         {
            _productBuyData.OnGetDataCompleted += OnCallbackBuyProductV2;
         }
         else
         {
            CallbackBuyProductV2();
         }
         return;
      }
      
      //тут вызываю error, т.к товар уже где ожидает ответа на покупку
      //Выведу ошибку при покупке
      ErrorLogic();
   }
   
   private void OnCallbackBuyProductV2()
   {
      _productBuyData.OnGetDataCompleted -= OnCallbackBuyProductV2;
      CallbackBuyProductV2();
   }
   
   private void CallbackBuyProductV2()
   {
#if UNITY_EDITOR
      if (_isDebug == true)
      {
         Debug.Log(gameObject.name + " SP 1700");
      }
#endif
      
         if (_productBuyData.StatusServer == StatusCallBackServer.Ok)
         {
            if (_productBuyData.GetData.ProductHaveBuy == true)
            {
               CheckProductHaveBuy2();
            }
            else
            {
               //Выведу ошибку при покупке
               ErrorLogic();
            }
         }
         else
         {
            //Выведу ошибку при покупке
            ErrorLogic();
         }
      

   }
   
   
   
   private void OnComplitedLogicBuyProduct()
   {
#if UNITY_EDITOR
      if (_isDebug == true)
      {
         Debug.Log(gameObject.name + " SP 2100");
      }
#endif
      
      if (_logicBuyProductComlited.IsCompleted == true)
      {
         _logicBuyProductComlited.OnCompleted -= OnComplitedLogicBuyProduct;
         
         _patchDataProduct.SetStatusBuyProduct();
         
         SetKeyProductBuy();
    
#if UNITY_EDITOR
         if (_isDebug == true)
         {
            Debug.Log(gameObject.name + " SP 2200");
         }
#endif

         switch (_typeSaveLogicComplited)
         {
            case TypeSaveProduct.NoSave:
            {
#if UNITY_EDITOR
               if (_isDebug == true)
               {
                  Debug.Log(gameObject.name + " SP 2300 Complited");
               }
#endif
               
               RemoveKeyProductBuyProcessing();
               _isStartLogic = false;
               OnComlited?.Invoke();
            }
               break;
                
            case TypeSaveProduct.DefaultSave:
            {
#if UNITY_EDITOR
               if (_isDebug == true)
               {
                  Debug.Log(gameObject.name + " SP 2400");
               }
#endif
               _patchDataProduct.OnSaveData += OnSaveDataStatusProduct;
               _patchDataProduct.SaveData(new TaskInfo("dd"));
            }
               break;
                
            case TypeSaveProduct.UrgentSave:
            {
#if UNITY_EDITOR
               if (_isDebug == true)
               {
                  Debug.Log(gameObject.name + " SP 2500");
               }
#endif
               _patchDataProduct.OnSaveData += OnSaveDataStatusProduct;
               _patchDataProduct.SaveData(new TaskInfo("dd"),true);
            }
               break;
                
         }
         
      }
   }
   
   private void OnSaveDataStatusProduct()
   {
#if UNITY_EDITOR
      if (_isDebug == true)
      {
         Debug.Log(gameObject.name + " SP 2600");
      }
#endif
      if (_patchDataProduct.LastStatusSaveData == StatusStorageAction.Ok) 
      {
#if UNITY_EDITOR
         if (_isDebug == true)
         {
            Debug.Log(gameObject.name + " SP 2700 Complited");
         }
#endif
         _patchDataProduct.OnSaveData -= OnSaveDataStatusProduct;
         
         RemoveKeyProductBuyProcessing();
         
         _isStartLogic = false;
         OnComlited?.Invoke();
      }
   }

   
   private void CheckProductHaveBuy2()
   {
      
#if UNITY_EDITOR
      if (_isDebug == true)
      {
         Debug.Log(gameObject.name + " SP 1800");
      }
#endif
      
      if (_checkProductHaveBuy.IsBlock == false)
      {
         if (_checkProductHaveBuyData != null)
         {
            _checkProductHaveBuyData.OnGetDataCompleted -= OnOnCheckProductHaveBuyStartBuyV2;
         }
         
         _checkProductHaveBuyData = _checkProductHaveBuy.CheckProductHaveBuy(_productId.GetData());
         if (_checkProductHaveBuyData.IsGetDataCompleted == false)
         {
            _checkProductHaveBuyData.OnGetDataCompleted += OnOnCheckProductHaveBuyStartBuyV2;
         }
         else
         {
            OnCheckProductHaveBuyStartBuyV2();
         }
         
         return;
      }
      
      _checkProductHaveBuy.OnUpdateStatusBlock -= CheckProductHaveBuyOnComplited2;
      _checkProductHaveBuy.OnUpdateStatusBlock += CheckProductHaveBuyOnComplited2;
   }

   private void CheckProductHaveBuyOnComplited2()
   {
      if (_checkProductHaveBuy.IsBlock == false)
      {
         if (_checkProductHaveBuyData != null)
         {
            _checkProductHaveBuyData.OnGetDataCompleted -= OnOnCheckProductHaveBuyStartBuyV2;
         }
         
         _checkProductHaveBuy.OnUpdateStatusBlock -= CheckProductHaveBuyOnComplited2;
         
         _checkProductHaveBuyData = _checkProductHaveBuy.CheckProductHaveBuy(_productId.GetData());
         if (_checkProductHaveBuyData.IsGetDataCompleted == false)
         {
            _checkProductHaveBuyData.OnGetDataCompleted += OnOnCheckProductHaveBuyStartBuyV2;
         }
         else
         {
            OnCheckProductHaveBuyStartBuyV2();
         }
      }
   }

   private void OnOnCheckProductHaveBuyStartBuyV2()
   {
      _checkProductHaveBuyData.OnGetDataCompleted -= OnOnCheckProductHaveBuyStartBuyV2;
      OnCheckProductHaveBuyStartBuyV2();
   }

   private void OnCheckProductHaveBuyStartBuyV2()
   {
#if UNITY_EDITOR
      if (_isDebug == true)
      {
         Debug.Log(gameObject.name + " SP 1900");
      }
#endif
      
         if (_checkProductHaveBuyData.StatusServer == StatusCallBackServer.Ok)
         {
            if (_checkProductHaveBuyData.GetData.IsBuyProduct == true)
            {
#if UNITY_EDITOR
               if (_isDebug == true)
               {
                  Debug.Log(gameObject.name + " SP 2000");
               }
#endif
               
               _logicBuyProductComlited.OnCompleted -= OnComplitedLogicBuyProduct;
               _logicBuyProductComlited.OnCompleted += OnComplitedLogicBuyProduct;

               _logicBuyProductComlited.StartAction(_dkoLogicComplited);

            }
            else
            {
#if UNITY_EDITOR
               if (_isDebug == true)
               {
                  Debug.Log(gameObject.name + " SP 1920");
               }
#endif
               //Выведу ошибку при покупке
               ErrorLogic();
            }
         }
         else
         {
#if UNITY_EDITOR
            if (_isDebug == true)
            {
               Debug.Log(gameObject.name + " SP 1940");
            }
#endif
            //Выведу ошибку при покупке
            ErrorLogic();
         }
      
   }
 

   private void SetKeyProductBuy()
   {
      if (_taskBlockStorage.IsKeyTask(_keyBlockProductHaveBuy.GetData()) == false)
      {
         _taskBlockStorage.AddTask(_keyBlockProductHaveBuy.GetData(), _textTaskBlockBuy);
      }
   }
   
   private void RemoveKeyProductBuy()
   {
      if (_taskBlockStorage.IsKeyTask(_keyBlockProductHaveBuy.GetData()) == true)
      {
         _taskBlockStorage.RemoveTask(_keyBlockProductHaveBuy.GetData());
      }
   }
   
   
   private void SetKeyProductBuyProcessing()
   {
      if (_taskBlockStorage.IsKeyTask(_keyBlockBuyProcessing.GetData()) == false)
      {
         _taskBlockStorage.AddTask(_keyBlockBuyProcessing.GetData(), _textTaskBlockBuyProcessing);
      }
   }
   
   private void RemoveKeyProductBuyProcessing()
   {
      if (_taskBlockStorage.IsKeyTask(_keyBlockBuyProcessing.GetData()) == true)
      {
         _taskBlockStorage.RemoveTask(_keyBlockBuyProcessing.GetData());
      }
   }

   public KeyProductId GetProductId()
   {
      return _productId.GetData();
   }

   public TSG_StorageTaskDefaultData GetStorageTask()
   {
      return _taskBlockStorage;
   }

   public bool IsBuyProduct()
   {
      return _patchDataProduct.IsBuyProduct();
   }


   private void ErrorLogic()
   {
      _isStartLogic = false;
      RemoveKeyProductBuyProcessing();
      OnErrorBuy?.Invoke();
      
      OnComlited?.Invoke();
   }
   
   private void OnDestroy()
   {
      _patchDataProduct.OnUpdateStorageDataData -= OnUpdateDataPatchProduct;
   }
}

public enum TypeSaveProduct
{
   NoSave,
   DefaultSave,
   UrgentSave
}
