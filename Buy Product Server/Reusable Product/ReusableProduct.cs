using System;
using UnityEngine;

public class ReusableProduct : MonoBehaviour
{
    public event Action OnErrorBuy;
   
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
    private GetDKOPatch _patchDKOGetTokensProductId;
    private GetProductTokensWrapper _getTokensProductIdLogic;
    private GetServerRequestData<ListProductTokenData> _getTokensProductIdLogicData;
    
    [SerializeField] 
    private GetDKOPatch _patchDKORemoveProductFromList;
    private RemoveProductTokensWrapper _removeProductFromList;
    private GetServerRequestData<RemoveProductTokenData> _removeProductFromListData;
    
    [SerializeField] 
    private GetDataSODataProductId _productId;
    
    [SerializeField] 
    private LogicListTaskDKO _logicBuyProductComlited;
    [SerializeField] 
    private DKOKeyAndTargetAction _dkoLogicComplited;

    [SerializeField] 
    private AbsSaveDataTokenReusableProduct _patchDataProduct;
    
    [SerializeField] 
    private GetDataSO_TSG_KeyTaskData _keyBlockBuy;
    [SerializeField] 
    private string _textTaskBlockBuy = "Идет обработка";

    [SerializeField] 
    private TypeSaveProduct _typeSaveLogicComplited;
    
    [SerializeField] 
    private TypeSaveProduct _typeSaveRemoveProductFromList;
    

    
    private bool _init = false;
       public bool IsInit => _init;
       public event Action OnInit;

       private ListProductTokenData _lastData;
       private ProductTokenData _currentToken;
       

       private bool _isStartLogic = false;
       
       /// <summary>
       /// Сработает когда запустить метод для начало покупки(начнеться проверка, есть ли необработанные покупки)
       /// </summary>
       public event Action OnStart;
       
       /// <summary>
       /// Сработает в случае error или в случае если обработка всех покупок завершина
       /// </summary>
       public event Action OnComlited;
       private void Awake()
       {       
          AddBlock();
          
          if (_patchDKOBuyProduct.Init == false)
          {
             //Debug.Log("YYYY P2");
             _patchDKOBuyProduct.OnInit += OnInitBuyProduct;
          } 
          
          if (_patchDKOGetTokensProductId.Init == false)
          {
             //Debug.Log("YYYY P3");
             _patchDKOGetTokensProductId.OnInit += OnInitGetTokensProductId;
          } 
          
          if (_patchDKORemoveProductFromList.Init == false)
          {
             //Debug.Log("YYYY P4");
             _patchDKORemoveProductFromList.OnInit += OnInitRemoveProductFromList;
          } 
                
          if (_logicBuyProductComlited.IsInit == false)
          {
             //Debug.Log("YYYY P5");
             _logicBuyProductComlited.OnInit += OnInitLogicBuy;
          } 
          
          if (_patchDataProduct.IsInit() == false)
          {
             //Debug.Log("YYYY P6");
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
          //Debug.Log("YYYY P7");
          _patchDKOBuyProduct.OnInit -= OnInitBuyProduct;
          OnInitGeneral();
       }
    
       private void OnInitGetTokensProductId()
       {          
          //Debug.Log("YYYY P8");
          _patchDKOGetTokensProductId.OnInit -= OnInitGetTokensProductId;
          OnInitGeneral();
       }
       
       private void OnInitRemoveProductFromList()
       {
          //Debug.Log("YYYY P9");
          _patchDKORemoveProductFromList.OnInit -= OnInitRemoveProductFromList;
          OnInitGeneral();
       }
       
       private void OnInitLogicBuy()
       {
          //Debug.Log("YYYY P10");
          _logicBuyProductComlited.OnInit -= OnInitLogicBuy;
          OnInitGeneral();
       }
       
       private void OnInitPatchDataProduct()
       {
          //Debug.Log("YYYY P11");
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
          if (_patchDKOBuyProduct.Init == true && _patchDKOGetTokensProductId.Init == true && _patchDKORemoveProductFromList.Init == true && _logicBuyProductComlited.IsInit == true && _patchDataProduct.IsInit() == true && _patchStorageKey.Init == true) 
          {         
             //Debug.Log("YYYY P20");
             var dkoProductBuy = (DKODataInfoT<BuyProductWrapper>)_patchDKOBuyProduct.GetDKO();
             _productBuy = dkoProductBuy.Data;
             
             var dkoGetTokensProductIdLogic = (DKODataInfoT<GetProductTokensWrapper>)_patchDKOGetTokensProductId.GetDKO();
             _getTokensProductIdLogic = dkoGetTokensProductIdLogic.Data;

             var dkoRemoveProductFromList = (DKODataInfoT<RemoveProductTokensWrapper>)_patchDKORemoveProductFromList.GetDKO();
             _removeProductFromList = dkoRemoveProductFromList.Data;
             
             var dkoStorageKeyTaskDataMono= (DKODataInfoT<TSG_StorageKeyTaskDataMono>) _patchStorageKey.GetDKO();
             dkoStorageKeyTaskDataMono.Data.AddTaskData(_keyStorageTaskBlock.GetData(), new TSG_StorageTaskDefaultData());
             _taskBlockStorage = dkoStorageKeyTaskDataMono.Data.GetTaskData(_keyStorageTaskBlock.GetData());
             
             if (_productBuy.IsInit == false)
             {    
                //Debug.Log("YYYY P21");
                _productBuy.OnInit += OnInitProductBuy;
             }
             else
             {
                //Debug.Log("YYYY P22");
                Init();
             }         
          }
       }
       
       private void OnInitProductBuy()
       {
          //Debug.Log("YYYY P23");
          _productBuy.OnInit -= OnInitProductBuy;
          Init();
       }
       
       private void Init()
       {
          //Debug.Log("YYYY P24");
          _patchDataProduct.OnUpdateData += OnUpdateDataStorage;
          GetTokensProductInit();
       }

       private void GetTokensProductInit()
       {
          if (_getTokensProductIdLogic.IsBlock == false)
          {
             if (_getTokensProductIdLogicData != null)
             {
                _getTokensProductIdLogicData.OnGetDataCompleted -= OnCallbackGetTokensProductInitV2;
             }
             
             _getTokensProductIdLogicData = _getTokensProductIdLogic.GetTokensProductIdLogic(_productId.GetData());
             if (_getTokensProductIdLogicData.IsGetDataCompleted == false)
             {
                _getTokensProductIdLogicData.OnGetDataCompleted += OnCallbackGetTokensProductInitV2;
             }
             else
             {
                CallbackGetTokensProductInitV2();
             }
                
             return;
          }
          
          //Debug.Log("YYYY P27");
          _getTokensProductIdLogic.OnUpdateStatusBlock -= GetTokensProductOnComplitedInit;
          _getTokensProductIdLogic.OnUpdateStatusBlock += GetTokensProductOnComplitedInit;
          
       }

       private void GetTokensProductOnComplitedInit()
       {
          if (_getTokensProductIdLogic.IsBlock == false)
          {
             
             if (_getTokensProductIdLogicData != null)
             {
                _getTokensProductIdLogicData.OnGetDataCompleted -= OnCallbackGetTokensProductInitV2;
             }
             
             _getTokensProductIdLogic.OnUpdateStatusBlock -= GetTokensProductOnComplitedInit;
             
             _getTokensProductIdLogicData = _getTokensProductIdLogic.GetTokensProductIdLogic(_productId.GetData());
             if (_getTokensProductIdLogicData.IsGetDataCompleted == false)
             {
                _getTokensProductIdLogicData.OnGetDataCompleted += OnCallbackGetTokensProductInitV2;
             }
             else
             {
                CallbackGetTokensProductInitV2();
             }
          }
       }

       private void OnCallbackGetTokensProductInitV2()
       {
          _getTokensProductIdLogicData.OnGetDataCompleted -= OnCallbackGetTokensProductInitV2;
          CallbackGetTokensProductInitV2();
       }
       
       private void CallbackGetTokensProductInitV2()
       {
       
             //Debug.Log("YYYY P31");
             if (_getTokensProductIdLogicData.StatusServer == StatusCallBackServer.Ok)
             {
                //Debug.Log("YYYY P33");
                if (_getTokensProductIdLogicData.GetData.ListProductToken.Count == 0)
                { 
                   //Debug.Log("YYYY P34");
                   InitInvoke();
                }
                else
                {
                   //Debug.Log("YYYY P35");
                   _lastData = _getTokensProductIdLogicData.GetData;
                   CheckTokenProductInStorageInit();
                }
             }
             else
             {
                //Debug.Log("YYYY P36");
                GetTokensProductInit();
             }
       }

       private void CheckTokenProductInStorageInit()
       {
          //Debug.Log("YYYY P37");
          if (_lastData.ListProductToken.Count > 0)
          {
             //Debug.Log("YYYY P38");
             _currentToken = _lastData.ListProductToken[0];
             if (_patchDataProduct.IsToken(_productId.GetData(), _currentToken.ProductToken) == true) 
             {
                //Debug.Log("YYYY P39");
                //Запускаю логику удаления
                RemoveTokenProductFromListInit();
             }
             else
             {
                //Debug.Log("YYYY P40");
                //знач запускаю логику и жду пока отработает
                _logicBuyProductComlited.OnCompleted += OnComplitedLogicInit;
                _logicBuyProductComlited.StartAction(_dkoLogicComplited);
             }
          }
          else
          {
             //Debug.Log("YYYY P41");
             InitInvoke();
          }
       }

       private void OnComplitedLogicInit()
       {
          //Debug.Log("YYYY P42");
          if (_logicBuyProductComlited.IsCompleted == true)
          {
             //Debug.Log("YYYY P43");
             _logicBuyProductComlited.OnCompleted -= OnComplitedLogicInit;
             
             _patchDataProduct.AddDataToken(_productId.GetData(), _currentToken.ProductToken);

             //Debug.Log("YYYY P44");
             switch (_typeSaveLogicComplited)
             {
                case TypeSaveProduct.NoSave:
                {
                   //Debug.Log("YYYY P45");
                   RemoveTokenProductFromListInit();
                }
                   break;
                
                case TypeSaveProduct.DefaultSave:
                {
                   //Debug.Log("YYYY P46");
                   _patchDataProduct.OnSaveData += OnSaveDataSetTokenInit;
                   _patchDataProduct.SaveData(new TaskInfo("dd"));
                }
                   break;
                
                case TypeSaveProduct.UrgentSave:
                {
                   //Debug.Log("YYYY P47");
                   _patchDataProduct.OnSaveData += OnSaveDataSetTokenInit;
                   _patchDataProduct.SaveData(new TaskInfo("dd"),true);
                }
                   break;
                
             }
             
        
          }
       }

       private void OnSaveDataSetTokenInit() 
       {
          if (_patchDataProduct.LastStatusSaveData == StatusStorageAction.Ok) 
          {
             _patchDataProduct.OnSaveData -= OnSaveDataSetTokenInit;

             RemoveTokenProductFromListInit();
          }
       }


       private void RemoveTokenProductFromListInit()
       {
          if (_removeProductFromList.IsBlock == false)
          {
             if (_removeProductFromListData != null)
             {
                _removeProductFromListData.OnGetDataCompleted -= OnCallbackRemoveTokenProductFromListInitV2;
             }

             _removeProductFromListData = _removeProductFromList.RemoveTokenProduct(_productId.GetData());
             if (_removeProductFromListData.IsGetDataCompleted == false)
             {
                _removeProductFromListData.OnGetDataCompleted += OnCallbackRemoveTokenProductFromListInitV2;
             }
             else
             {
                CallbackRemoveTokenProductFromListInitV2();
             }
             
             return;
          }
          
          _removeProductFromList.OnUpdateStatusBlock -= OnCallbackRemoveTokenProductFromListInitV2CheckComplited;
          _removeProductFromList.OnUpdateStatusBlock += OnCallbackRemoveTokenProductFromListInitV2CheckComplited;
       }
       
       private void OnCallbackRemoveTokenProductFromListInitV2CheckComplited()
       {
          if (_removeProductFromList.IsBlock == false)
          {
             if (_removeProductFromListData != null)
             {
                _removeProductFromListData.OnGetDataCompleted -= OnCallbackRemoveTokenProductFromListInitV2;
             }
             
             _removeProductFromList.OnUpdateStatusBlock -= OnCallbackRemoveTokenProductFromListInitV2CheckComplited;
             
             _removeProductFromListData = _removeProductFromList.RemoveTokenProduct(_productId.GetData());
             if (_removeProductFromListData.IsGetDataCompleted == false)
             {
                _removeProductFromListData.OnGetDataCompleted += OnCallbackRemoveTokenProductFromListInitV2;
             }
             else
             {
                CallbackRemoveTokenProductFromListInitV2();
             }
             
             return;
          }
       }
       
       private void OnCallbackRemoveTokenProductFromListInitV2()
       {
          _removeProductFromListData.OnGetDataCompleted -= OnCallbackRemoveTokenProductFromListInitV2;
          CallbackRemoveTokenProductFromListInitV2();
       }
       
       private void CallbackRemoveTokenProductFromListInitV2()
       {
             if (_removeProductFromListData.StatusServer == StatusCallBackServer.Ok) 
             {
                //Даже если не найдет токен, логика та же, сначало удаляю токен из хран, затем из списка и вызываю сохранение 
                _patchDataProduct.RemoveDataToken(_productId.GetData(), _currentToken.ProductToken);
                _lastData.ListProductToken.Remove(_currentToken);
                
                switch (_typeSaveRemoveProductFromList)
                {
                   case TypeSaveProduct.NoSave:
                   {
                      CheckTokenProductInStorageInit();
                   }
                      break;
                
                   case TypeSaveProduct.DefaultSave:
                   {
                      _patchDataProduct.OnSaveData += OnSaveDataRemoveTokenInit;
                      _patchDataProduct.SaveData(new TaskInfo("dd"));
                   }
                      break;
                
                   case TypeSaveProduct.UrgentSave:
                   {
                      _patchDataProduct.OnSaveData += OnSaveDataRemoveTokenInit;
                      _patchDataProduct.SaveData(new TaskInfo("dd"),true);
                   }
                      break;
                
                }
             }
             else
             {
                RemoveTokenProductFromListInit();
             }
       }

       private void OnSaveDataRemoveTokenInit()
       {
          if (_patchDataProduct.LastStatusSaveData == StatusStorageAction.Ok)
          {
             _patchDataProduct.OnSaveData -= OnSaveDataRemoveTokenInit;
             CheckTokenProductInStorageInit();
          }
          
       }
   
       /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

       

/// <summary>
/// В случае обновление данных в хранилеще(к примеру из за переключения хранилеща)
/// </summary>
       private void OnUpdateDataStorage()
       {
          if (IsInit == false)
          {
             _getTokensProductIdLogicData.OnGetDataCompleted -= OnCallbackGetTokensProductInitV2;
             _removeProductFromListData.OnGetDataCompleted -= OnCallbackRemoveTokenProductFromListInitV2;
          
             _getTokensProductIdLogic.OnUpdateStatusBlock -= GetTokensProductOnComplitedInit;
             _logicBuyProductComlited.OnCompleted -= OnComplitedLogicInit;
          
             _patchDataProduct.OnSaveData -= OnSaveDataSetTokenInit;
             _patchDataProduct.OnSaveData -= OnSaveDataRemoveTokenInit;

             GetTokensProductInit();
          }
          else
          {
             if (_isStartLogic==true)
             {
                _getTokensProductIdLogicData.OnGetDataCompleted -= OnCallbackGetTokensProductCheckV2;
                _productBuyData.OnGetDataCompleted -= OnCallbackBuyProductV2;
                _getTokensProductIdLogicData.OnGetDataCompleted -= OnCallbackGetTokensProductV2;
                _removeProductFromListData.OnGetDataCompleted -= OnCallbackRemoveTokenProductFromListV2;
             
                _getTokensProductIdLogic.OnUpdateStatusBlock -= GetTokensProductOnComplitedCheck;
             
                _getTokensProductIdLogic.OnUpdateStatusBlock -= GetTokensProductOnComplited;
                _logicBuyProductComlited.OnCompleted -= OnComplitedLogic;
             
                _patchDataProduct.OnSaveData -= OnSaveDataSetToken;
                _patchDataProduct.OnSaveData -= OnSaveDataRemoveToken;
             
                // AddBlock();
                // GetTokensProduct();
                Error();
             }
          }
       }
       
       
       /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
       
       
       
       
       
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
    
    
       public void BuyProduct()
       {
          if (IsStart() == true)
          {
             AddBlock();
             _isStartLogic = true;
             OnStart?.Invoke();
             //Теперь перед вызовом покупки делаю проверку, на то что есть ли не обработанные покупки или нет 
            GetTokensProductCheck();

          }
      
       }
       
        private void GetTokensProductCheck()
       {
          if (_getTokensProductIdLogic.IsBlock == false)
          {
             if (_getTokensProductIdLogicData != null)
             {
                _getTokensProductIdLogicData.OnGetDataCompleted -= OnCallbackGetTokensProductCheckV2;
             }
             
             _getTokensProductIdLogicData = _getTokensProductIdLogic.GetTokensProductIdLogic(_productId.GetData());
             if (_getTokensProductIdLogicData.IsGetDataCompleted == false)
             {
                _getTokensProductIdLogicData.OnGetDataCompleted += OnCallbackGetTokensProductCheckV2;
             }
             else
             {
                CallbackGetTokensProductCheckV2();
             }
             
             return;
          }
          
          _getTokensProductIdLogic.OnUpdateStatusBlock -= GetTokensProductOnComplitedCheck;
          _getTokensProductIdLogic.OnUpdateStatusBlock += GetTokensProductOnComplitedCheck;
          
       }

       private void GetTokensProductOnComplitedCheck()
       {
          if (_getTokensProductIdLogic.IsBlock == false)
          {
             if (_getTokensProductIdLogicData != null)
             {
                _getTokensProductIdLogicData.OnGetDataCompleted -= OnCallbackGetTokensProductCheckV2;
             }
             
             _getTokensProductIdLogic.OnUpdateStatusBlock -= GetTokensProductOnComplitedCheck;
             
             _getTokensProductIdLogicData = _getTokensProductIdLogic.GetTokensProductIdLogic(_productId.GetData());
             if (_getTokensProductIdLogicData.IsGetDataCompleted == false)
             {
                _getTokensProductIdLogicData.OnGetDataCompleted += OnCallbackGetTokensProductCheckV2;
             }
             else
             {
                CallbackGetTokensProductCheckV2();
             }
          }
       }

       private void OnCallbackGetTokensProductCheckV2()
       {
          _getTokensProductIdLogicData.OnGetDataCompleted -= OnCallbackGetTokensProductCheckV2;
          CallbackGetTokensProductCheckV2();
       }


       private void CallbackGetTokensProductCheckV2()
       {          
             if (_getTokensProductIdLogicData.StatusServer == StatusCallBackServer.Ok)
             {
                if (_getTokensProductIdLogicData.GetData.ListProductToken.Count == 0)
                {
                   //начинаю покупку
                   BuyProductLogic();
                }
                else
                {
                   _lastData = _getTokensProductIdLogicData.GetData;
                   //начинаю проверку и обработку
                   CheckTokenProductInStorage();
                }
             }
             else
             {
                //Выведу ошибку при покупке
                Error();
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

          Error();
       }
       


       private void OnCallbackBuyProductV2()
       {
          _productBuyData.OnGetDataCompleted -= OnCallbackBuyProductV2;
           
          CallbackBuyProductV2();
       }



       private void CallbackBuyProductV2()
       {
      
             if (_productBuyData.StatusServer == StatusCallBackServer.Ok)
             {
                if (_productBuyData.GetData.ProductHaveBuy == true)
                {
                   GetTokensProduct();
                }
                else
                {
                   //Выведу ошибку при покупке
                   Error();
                }
             }
             else
             {
                //Выведу ошибку при покупке
                Error();
             }
       }
       
       
       private void GetTokensProduct()
       {
          if (_getTokensProductIdLogic.IsBlock == false)
          {
             if (_getTokensProductIdLogicData != null)
             {
                _getTokensProductIdLogicData.OnGetDataCompleted -= OnCallbackGetTokensProductV2;
             }
             
             _getTokensProductIdLogicData = _getTokensProductIdLogic.GetTokensProductIdLogic(_productId.GetData());
             if (_getTokensProductIdLogicData.IsGetDataCompleted == false)
             {
                _getTokensProductIdLogicData.OnGetDataCompleted += OnCallbackGetTokensProductV2;
             }
             else
             {
                CallbackGetTokensProductV2();
             }
             
             return;
          }
          
          _getTokensProductIdLogic.OnUpdateStatusBlock -= GetTokensProductOnComplited;
          _getTokensProductIdLogic.OnUpdateStatusBlock += GetTokensProductOnComplited;
       }

       private void GetTokensProductOnComplited()
       {
          if (_getTokensProductIdLogic.IsBlock == false)
          {
             if (_getTokensProductIdLogicData != null)
             {
                _getTokensProductIdLogicData.OnGetDataCompleted -= OnCallbackGetTokensProductV2;
             }
             
             _getTokensProductIdLogic.OnUpdateStatusBlock -= GetTokensProductOnComplited;
             
             _getTokensProductIdLogicData = _getTokensProductIdLogic.GetTokensProductIdLogic(_productId.GetData());
             if (_getTokensProductIdLogicData.IsGetDataCompleted == false)
             {
                _getTokensProductIdLogicData.OnGetDataCompleted += OnCallbackGetTokensProductV2;
             }
             else
             {
                CallbackGetTokensProductV2();
             }
          }
       }

       private void OnCallbackGetTokensProductV2()
       {
          _getTokensProductIdLogicData.OnGetDataCompleted -= OnCallbackGetTokensProductV2;
          CallbackGetTokensProductV2();
       }
       
       private void CallbackGetTokensProductV2()
       {
        
             if (_getTokensProductIdLogicData.StatusServer == StatusCallBackServer.Ok)
             {
                if (_getTokensProductIdLogicData.GetData.ListProductToken.Count == 0)
                {
                   //Выведу ошибку при покупке
                   Error();
                }
                else
                {
                   _lastData = _getTokensProductIdLogicData.GetData;
                   CheckTokenProductInStorage();
                }
             }
             else
             {
                //Выведу ошибку при покупке
                Error();
             }
          
          
       }
       
       private void CheckTokenProductInStorage()
       {
          if (_lastData.ListProductToken.Count > 0)
          {
             _currentToken = _lastData.ListProductToken[0];
             if (_patchDataProduct.IsToken(_productId.GetData(), _currentToken.ProductToken) == true) 
             {           
                //Запускаю логику удаления
                RemoveTokenProductFromList();
             }
             else
             {
                //знач запускаю логику и жду пока отработает
                _logicBuyProductComlited.OnCompleted += OnComplitedLogic;
                _logicBuyProductComlited.StartAction(_dkoLogicComplited);
             }
          }
          else
          {
             //получаеться, тут все конец и можно заканчивать

             OnComlited?.Invoke();
             _isStartLogic = false;
             RemoveBlock();
          }
       }
       
       
       
       
       
       
       
       private void OnComplitedLogic()
       {
          if (_logicBuyProductComlited.IsCompleted == true)
          {
             _logicBuyProductComlited.OnCompleted -= OnComplitedLogic;
             
             _patchDataProduct.AddDataToken(_productId.GetData(), _currentToken.ProductToken);
             
             switch (_typeSaveLogicComplited)
             {
                case TypeSaveProduct.NoSave:
                {
                   RemoveTokenProductFromList();
                }
                   break;
                
                case TypeSaveProduct.DefaultSave:
                {
                   _patchDataProduct.OnSaveData += OnSaveDataSetToken;
                   _patchDataProduct.SaveData(new TaskInfo("dd"));
                }
                   break;
                
                case TypeSaveProduct.UrgentSave:
                {
                   _patchDataProduct.OnSaveData += OnSaveDataSetToken;
                   _patchDataProduct.SaveData(new TaskInfo("dd"),true);
                }
                   break;
                
             }
             

          }
       }
       
       private void OnSaveDataSetToken()
       {
          if (_patchDataProduct.LastStatusSaveData == StatusStorageAction.Ok) 
          {
             _patchDataProduct.OnSaveData -= OnSaveDataSetToken;

             RemoveTokenProductFromList();

          }
       }
       
       private void RemoveTokenProductFromList()
       {
          if (_removeProductFromList.IsProductStartWating(_productId.GetData()) == false)
          {
             if (_removeProductFromListData != null)
             {
                _removeProductFromListData.OnGetDataCompleted -= OnCallbackRemoveTokenProductFromListV2;
             }
             
             _removeProductFromListData = _removeProductFromList.RemoveTokenProduct(_productId.GetData());
             if (_removeProductFromListData.IsGetDataCompleted == false)
             {
                _removeProductFromListData.OnGetDataCompleted += OnCallbackRemoveTokenProductFromListV2;
             }
             else
             {
                CallbackRemoveTokenProductFromListV2();
             }
             
             return;
          }
          
          Error();
       }

       private void OnCallbackRemoveTokenProductFromListV2()
       {
          _removeProductFromListData.OnGetDataCompleted -= OnCallbackRemoveTokenProductFromListV2;
          CallbackRemoveTokenProductFromListV2();
       }

       private void CallbackRemoveTokenProductFromListV2()
       {
        
             if (_removeProductFromListData.StatusServer == StatusCallBackServer.Ok)
             {
                //Даже если не найдет токен, логика та же, сначало удаляю токен из хран, затем из списка и вызываю сохранение 
                _patchDataProduct.RemoveDataToken(_productId.GetData(), _currentToken.ProductToken);
                _lastData.ListProductToken.Remove(_currentToken);

                
                switch (_typeSaveRemoveProductFromList)
                {
                   case TypeSaveProduct.NoSave:
                   {
                      CheckTokenProductInStorage();
                   }
                      break;
                
                   case TypeSaveProduct.DefaultSave:
                   {
                      _patchDataProduct.OnSaveData += OnSaveDataRemoveToken;
                      _patchDataProduct.SaveData(new TaskInfo("dd"));
                   }
                      break;
                
                   case TypeSaveProduct.UrgentSave:
                   {
                      _patchDataProduct.OnSaveData += OnSaveDataRemoveToken;
                      _patchDataProduct.SaveData(new TaskInfo("dd"),true);
                   }
                      break;
                
                }
                

             }
             else
             {
                Error();
             }
          
       }


       private void OnSaveDataRemoveToken()
       {
          if (_patchDataProduct.LastStatusSaveData == StatusStorageAction.Ok)
          {
             _patchDataProduct.OnSaveData -= OnSaveDataRemoveToken;
             CheckTokenProductInStorage();
          }
          
       }
       
       private void OnDestroy()
       {
          _patchDataProduct.OnUpdateData -= OnUpdateDataStorage;
       }

       private void Error()
       {
          _isStartLogic = false;
          RemoveBlock();
          
          OnErrorBuy?.Invoke();
          
          OnComlited?.Invoke();
       }

       private void AddBlock()
       {
          if (_taskBlockStorage.IsKeyTask(_keyBlockBuy.GetData()) == false)
          {
             _taskBlockStorage.AddTask(_keyBlockBuy.GetData(), _textTaskBlockBuy);
          }
       }

       private void RemoveBlock()
       {
          if (_taskBlockStorage.IsKeyTask(_keyBlockBuy.GetData()) == true)
          {
             _taskBlockStorage.RemoveTask(_keyBlockBuy.GetData());
          }
       }

       private void InitInvoke()
       {
          RemoveBlock();
          
          _init = true;
          OnInit?.Invoke();
       }
       
       public TSG_StorageTaskDefaultData GetStorageTask()
       {
          return _taskBlockStorage;
       }
}


