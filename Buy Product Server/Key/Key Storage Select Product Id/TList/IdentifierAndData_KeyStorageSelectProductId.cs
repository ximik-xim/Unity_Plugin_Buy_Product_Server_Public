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
 
#if UNITY_EDITOR
 public override string GetJsonSaveData()
 {
  return JsonUtility.ToJson(_dataKey);
 }

 public override void SetJsonData(string json)
 {
  _dataKey = JsonUtility.FromJson<KeyStorageSelectProductId>(json);
 }
#endif
}
