
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class RemoveProductTokensWrapper : MonoBehaviour
{
    private bool _init = false;
    public bool IsInit => _init;
    public event Action OnInit;
    
    /// <summary>
    /// Нужен, если будет несколько таких классов для вызова покупок
    /// (у каждого экземпляра должен быть УНКИКАЛЬНЫЙ ключ)
    /// </summary>
    [SerializeField] 
    private string _keyInstanceClass = "";
    
    [SerializeField]
    private GetDKOPatch _patchStorageKey;
    [SerializeField]
    private GetDataSO_TSG_KeyStorageTask _keyStorageTaskBlock;
    private TSG_StorageTaskDefaultData _taskBlockStorage;

    /// <summary>
    /// Заблокированы ли покупки
    /// </summary>
    public bool IsBlock => _taskBlockStorage.IsThereTasks();
    public event Action OnUpdateStatusBlock
    {
        add
        {
            _taskBlockStorage.OnUpdateStatus += value;
        }

        remove
        {
            _taskBlockStorage.OnUpdateStatus -= value;
        }
        
    }

    private Dictionary<string, Dictionary<int, ServerRequestDataWrapperRemoveProductTokenData>> _data = new Dictionary<string, Dictionary<int, ServerRequestDataWrapperRemoveProductTokenData>>();

    [SerializeField] 
    private AbsRemoveProductToken _removeTokenLogic;
    
    private void Awake()
    {
        if (_removeTokenLogic.IsInit == false)
        {
            _removeTokenLogic.OnInit += OnInitLogicBuy;
        } 
        
        if (_patchStorageKey.Init == false)
        {
            _patchStorageKey.OnInit += OnInitPatchStorageKey;
        } 
        
        CheckInit();
    }

    private void OnInitLogicBuy()
    {
        _removeTokenLogic.OnInit -= OnInitLogicBuy;
        CheckInit();
    }
    
    private void OnInitPatchStorageKey()
    {
        _patchStorageKey.OnInit -= OnInitPatchStorageKey;
        CheckInit();
    }

    private void CheckInit()
    {
        if (_init == false)
        {
            if (_removeTokenLogic.IsInit == true && _patchStorageKey.Init == true) 
            {
                Init();
            }    
        }
    }
    
    private void Init()
    {
        var storageKeyTaskDataMono= (DKODataInfoT<TSG_StorageKeyTaskDataMono>) _patchStorageKey.GetDKO();
        storageKeyTaskDataMono.Data.AddTaskData(_keyStorageTaskBlock.GetData(), new TSG_StorageTaskDefaultData());
        _taskBlockStorage = storageKeyTaskDataMono.Data.GetTaskData(_keyStorageTaskBlock.GetData());

        _init = true;
        OnInit?.Invoke();
    }
    

    
    public GetServerRequestData<RemoveProductTokenData> RemoveTokenProduct(KeyProductId keyProduct)
    {
        if (_data.ContainsKey(keyProduct.GetKey()) == false)
        {
            _data.Add(keyProduct.GetKey(),new Dictionary<int, ServerRequestDataWrapperRemoveProductTokenData>());
        }

        int id = 0;
        while (_data[keyProduct.GetKey()].ContainsKey(id) == true)
        {
            id = Random.Range(0, 2147483600);
        }

        var data = new ServerRequestDataWrapperRemoveProductTokenData(id);
        _data[keyProduct.GetKey()].Add(id, data);

        _removeTokenLogic.RemoveProductToken(Callback, id, keyProduct, _keyInstanceClass);

        return data.DataGet;
    }


    /// <summary>
    ///     1 - id который отпровлял
    ///     2 - ключ продукта
    ///     3 - статус сервера
    ///    4 - сами данные которые отправлю назад
    /// </summary>
    private void Callback(int id, KeyProductId keyProduct, StatusCallBackServer statusServer, RemoveProductTokenData data, string keyInstanceClass)
    {
        if (keyInstanceClass == _keyInstanceClass)
        {
            var dataReturn = _data[keyProduct.GetKey()][id].Data;

            dataReturn.IsGetDataCompleted = true;
            dataReturn.StatusServer = statusServer;
            dataReturn.GetData = data;

            _data[keyProduct.GetKey()].Remove(id);

            dataReturn.Invoke();
        }
    }

    /// <summary>
    /// Есть ли запросы на обработку этого ключа
    /// </summary>
    public bool IsProductStartWating(KeyProductId keyProduct)
    {
        if (_data.ContainsKey(keyProduct.GetKey()) == true)
        {
            if (_data[keyProduct.GetKey()].Count > 0)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Есть ли запросы на обработку этого ключа с указанным Id
    /// </summary>
    public bool IsProductStartWating(KeyProductId keyProduct, int id)
    {
        if (_data.ContainsKey(keyProduct.GetKey()) == true)
        {
            if (_data[keyProduct.GetKey()].ContainsKey(id) == true) 
            {
                return true;
            }
        }

        return false;
    }
}
