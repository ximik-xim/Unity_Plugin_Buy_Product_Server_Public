using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

/// <summary>
/// Нуен что бы эмулировать работу сервера с токенами
/// (дело в том, для многоразовых покупок, при покупки создаеться токен, благодаря которому и отслеживаеться была ли покупка)
/// (У однораз. покупок такого нету. Там просто через запр. на сервер можно получить был ли куплен товар) 
/// </summary>
public class ExampleStorageTokens : MonoBehaviour
{
    public bool IsInit => true;
    public event Action OnInit;
    
    /// <summary>
    /// Данные об токене продукта
    /// </summary>
    [SerializeField]
    private StorageKeyListTokensBuyProduct _listTokensBuyProduct = new StorageKeyListTokensBuyProduct();

    private void Awake()
    {
        OnInit?.Invoke();
    }

    public void AddToken(KeyProductId keyProduct)
    {
        if (_listTokensBuyProduct.IsThereListToken(keyProduct.GetKey()) == false)
        {
            _listTokensBuyProduct.AddElement(keyProduct.GetKey(), new ListProductTokenData());
        }

        var listToken = _listTokensBuyProduct.GetListToken(keyProduct.GetKey());

        listToken.ListProductToken.Add(new ProductTokenData(GenerateHash(keyProduct.GetKey())));
    }

    public bool IsThereToken(KeyProductId keyProduct)
    {
        if (_listTokensBuyProduct.IsThereListToken(keyProduct.GetKey()) == true)
        {
            var listToken = _listTokensBuyProduct.GetListToken(keyProduct.GetKey());
            if (listToken.ListProductToken.Count > 0) 
            {
                return true;
            }
            
        }

        return false;
    }
    
    public void DeletedToken(KeyProductId keyProduct)
    {
        if (_listTokensBuyProduct.IsThereListToken(keyProduct.GetKey()) == true)
        {
            var listToken = _listTokensBuyProduct.GetListToken(keyProduct.GetKey());
            if (listToken.ListProductToken.Count > 0)
            {
                listToken.ListProductToken.RemoveAt(0);
            }
        }
    }
    
    public ListProductTokenData GetListToken(KeyProductId keyProduct)
    {
        if (_listTokensBuyProduct.IsThereListToken(keyProduct.GetKey()) == false)
        {
            _listTokensBuyProduct.AddElement(keyProduct.GetKey(), new ListProductTokenData());
        }

        var listToken = _listTokensBuyProduct.GetListToken(keyProduct.GetKey());

        return listToken;
    }
    
    private string GenerateHash(string keyProduct)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(keyProduct);
            byte[] hashBytes = sha256.ComputeHash(bytes);

            // Преобразуем хэш в строку
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }
            
            return sb.ToString();
        }
    }
}
