using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ListProductTokenData 
{
    public ListProductTokenData()
    {
        
    }

    public ListProductTokenData(List<ProductTokenData> listToken)
    {
        _list = listToken;
    }
    
    [SerializeField] 
    private List<ProductTokenData> _list;

    public List<ProductTokenData> ListProductToken => _list;
}
