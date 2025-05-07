using System;
using UnityEngine;

public abstract class AbsGetKeyProductId : MonoBehaviour
{
   public abstract bool IsInit { get; }
   public abstract event Action OnInit;

   public abstract KeyProductId GetProductId();
}
