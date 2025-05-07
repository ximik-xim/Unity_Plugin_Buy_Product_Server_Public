using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbsSaveDataSinglProduct : MonoBehaviour
{
    public abstract bool IsInit();
    public abstract event Action OnInit;

    /// <summary>
    /// Вызываеться когда была обновлена именно эта переменная
    /// (не зависит, было ли услановлено новое знач. отсюда или нет)
    /// </summary>
    public abstract event Action OnUpdateCurrentValue;
    
    /// <summary>
    /// Вызываеться когда хранилеще выгрузило новые данные с сервера
    /// </summary>
    public abstract event Action OnUpdateStorageDataData;
    public abstract bool IsBuyProduct();
    
    public abstract void SetStatusBuyProduct();
    public abstract void RemoveStatusBuyProduct();
    
    public abstract event Action OnSaveData;
    public abstract void SaveData(TaskInfo taskInfo, bool urgentSaving = false);
    public abstract StatusStorageAction LastStatusSaveData { get; }

}
