using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Список токенов по ключу
/// (нужен что бы получить по ключу продукта список токенов)
/// (и да, что бы преобразовать в JSON и востоновить данные все из того же JSON)
/// </summary>
[System.Serializable]
public class StorageKeyListTokensBuyProduct
{
    [SerializeField]
    private List<AbsKeyData<string, ListProductTokenData>> _productTokensData = new List<AbsKeyData<string, ListProductTokenData>>();
    
    public void AddElement(string productId, ListProductTokenData data)
    {
        _productTokensData.Add(new AbsKeyData<string, ListProductTokenData>(productId, data));
    }

    public void RemoveListTokens(string productId)
    {
        for (int i = 0; i < _productTokensData.Count; i++)
        {
            if (_productTokensData[i].Key == productId) 
            {
                _productTokensData.RemoveAt(i);
                return;
            }
        }
    }

    public bool IsThereListToken(string productId)
    {
        for (int i = 0; i < _productTokensData.Count; i++)
        {
            if (_productTokensData[i].Key == productId) 
            {
                return true;
            }
        }

        return false;
    }
    
    public ListProductTokenData GetListToken(string productId)
    {
        for (int i = 0; i < _productTokensData.Count; i++)
        {
            if (_productTokensData[i].Key == productId) 
            {
                return _productTokensData[i].Data;
            }
        }

        return null;
    }

    public void SetListToken(string productId, ListProductTokenData data)
    {
        for (int i = 0; i < _productTokensData.Count; i++)
        {
            if (_productTokensData[i].Key==productId)
            {
                _productTokensData[i].Data = data;
                return;
            }
        }

        AddElement(productId, data);
    }
}