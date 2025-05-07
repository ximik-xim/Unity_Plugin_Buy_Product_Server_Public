using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbsSaveDataTokenReusableProduct : MonoBehaviour
{
    public abstract bool IsInit();
    public abstract event Action OnInit;

    public abstract event Action OnUpdateData;
    
    public abstract bool IsToken(KeyProductId keyProduct, string token);
    public abstract void AddDataToken(KeyProductId keyProduct, string token);
    public abstract void RemoveDataToken(KeyProductId keyProduct, string token);

    public abstract event Action OnSaveData;
    public abstract void SaveData(TaskInfo taskInfo, bool urgentSaving = false);
    public abstract StatusStorageAction LastStatusSaveData { get; }

}
