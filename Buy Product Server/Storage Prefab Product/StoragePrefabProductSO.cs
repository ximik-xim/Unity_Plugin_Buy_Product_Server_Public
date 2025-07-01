using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(menuName = "Buy Product Server/Storage Prefab Product SO")]
public class StoragePrefabProductSO : ScriptableObject, IInitScripObj
{
    [SerializeField] 
    private List<AbsKeyData<GetDataSODataProductId, DKOKeyAndTargetAction>> _listData = new List<AbsKeyData<GetDataSODataProductId, DKOKeyAndTargetAction>>();
    private Dictionary<string, DKOKeyAndTargetAction> _dictionaryData = new Dictionary<string, DKOKeyAndTargetAction>();
    
    public event Action OnInit;
    public bool IsInit => _isInit;
    private bool _isInit = false;

    private void Awake()
    {
        if (_isInit == false)
        {
            foreach (var VARIABLE in _listData)
            {
                _dictionaryData.Add(VARIABLE.Key.GetData().GetKey(), VARIABLE.Data);
            }       

            _isInit = true;
            OnInit?.Invoke();
        }
    }
    
    public DKOKeyAndTargetAction GetPrefabProduct(KeyProductId key)
    {
        return _dictionaryData[key.GetKey()];
    }
    
    
    public void InitScripObj()
    {
#if UNITY_EDITOR
        
        EditorApplication.playModeStateChanged -= OnUpdateStatusPlayMode;
        EditorApplication.playModeStateChanged += OnUpdateStatusPlayMode;

        //На случай если event playModeStateChanged не отработает при входе в режим PlayModeStateChange.EnteredPlayMode (такое может быть, и как минимум по этому нужна пер. bool _init)
        if (EditorApplication.isPlaying == true)
        {
            if (_isInit == false)
            {
                Awake();
            }
        }
        else
        {
            //Нужен, что бы сбросить переменную при запуске проекта(т.к при выходе(закрытии) из проекта, переменная не факт что будет сброшена)
            _isInit = false;
        }
#endif
    }
        
#if UNITY_EDITOR
    private void OnUpdateStatusPlayMode(PlayModeStateChange obj)
    {
        //При выходе из Play Mode произвожу очистку данных(тем самым эмулирую что при след. запуске(вхождение в Play Mode) данные будут обнулены)
        if (obj == PlayModeStateChange.ExitingPlayMode)
        {
            if (_isInit == true)
            {
                _isInit = false;
            }
        }
        
        // При запуске игры эмулирую иниц. SO(По идеи не совсем верно, т.к Awake должен произойти немного раньше, но пофиг)(как показала практика метод может не сработать)
        if (obj == PlayModeStateChange.EnteredPlayMode)
        {
            if (_isInit == false)
            {
                Awake();
            }
            
        }
    }
#endif

    
    private void OnDestroy()
    {
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged -= OnUpdateStatusPlayMode;
#endif
    }
}
