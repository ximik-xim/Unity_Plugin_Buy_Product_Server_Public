using System;
using UnityEngine;

public abstract class AbsGetPrefabProduct : MonoBehaviour
{
   public abstract bool IsInit { get; }
   
   public abstract event Action OnInit;
   
   public abstract DKOKeyAndTargetAction GetPrefabProduct(KeyProductId key);
}
