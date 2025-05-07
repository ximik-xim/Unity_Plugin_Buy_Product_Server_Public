using UnityEngine; 
using TListPlugin; 
[System.Serializable]
public class IdentifierAndData_KeyStorageSelectProductId : AbsIdentifierAndData<IndifNameSO_KeyStorageSelectProductId, string, KeyStorageSelectProductId>
{

 [SerializeField] 
 private KeyStorageSelectProductId _dataKey;


 public override KeyStorageSelectProductId GetKey()
 {
  return _dataKey;
 }
}
