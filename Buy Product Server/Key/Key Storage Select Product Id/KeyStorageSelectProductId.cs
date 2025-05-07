using UnityEngine;

[System.Serializable]
public class KeyStorageSelectProductId
{
    [SerializeField]
    private string _key;

    public string GetKey()
    {
        return _key;
    }
}
