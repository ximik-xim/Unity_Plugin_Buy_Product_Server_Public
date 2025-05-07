using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class CheckIsBuySinglProduct : MonoBehaviour
{
   [SerializeField] 
   private SinglProduct _singlProduct;

   [SerializeField] 
   private DKOKeyAndTargetAction _dkoData;
   
   [SerializeField] 
   private LogicListTaskDKO _taskProductBuy;
   
   [SerializeField] 
   private LogicListTaskDKO _taskProductDontBuy;
   
   private void Awake()
   {
      if (_singlProduct.IsInit == false) 
      {
         _singlProduct.OnInit += OnInitProduct;
         return;
      }

      Init();
   }

   private void OnInitProduct()
   {
      _singlProduct.OnInit -= OnInitProduct;
      Init();
   }

   private void Init()
   {
      _singlProduct.OnUpdateCurrentProductData += OnUpdateData;
      OnUpdateData();
   }

   private void OnEnable()
   {
      if (_singlProduct != null && _singlProduct.IsInit == true) 
      {
         OnUpdateData();
      }
   }

   private void OnUpdateData()
   {
      if (_singlProduct.IsBuyProduct() == true) 
      {
         _taskProductBuy.StartAction(_dkoData);
      }
      else
      {
         _taskProductDontBuy.StartAction(_dkoData);
      }
   }


   private void OnDestroy()
   {
      _singlProduct.OnUpdateCurrentProductData -= OnUpdateData;
   }
}
