using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{

    #region UIInfoData Class
    class UIInfoData
    {
        public EnumUIType UIType { get; private set; }
        public Type ScriptType { get; private set; }
        public string Path { get; private set; }
        public object[] UIParams { get; private set; }
        public UIInfoData(EnumUIType _uiType, string _path,params object[] _uiparams)
        {
            this.UIType = _uiType;
            this.Path = _path;
            this.UIParams = _uiparams;
            this.ScriptType = UIPathDefines.GetUIScriptByType(this.UIType);
        }
    }
    #endregion
    /// <summary>
    /// 存放UI
    /// </summary>
    private Dictionary<EnumUIType, GameObject> dicOpenUIs = null;

    /// <summary>
    /// 用栈存放将要打开的UI
    /// </summary>
    private Stack<UIInfoData> stackOpenUIs = null;
    public override void Init()
    {
        dicOpenUIs = new Dictionary<EnumUIType, GameObject>();
        stackOpenUIs = new Stack<UIInfoData>();
    }
    #region Get UI & UIObject By EnumUIType
    public T GetUI<T>(EnumUIType _uiType) where T:BaseUI
    {
        GameObject _retObj = GetUIObject(_uiType);
        if (_retObj!=null)
        {
            return _retObj.GetComponent<T>();
        }
        return null;
    }
    public GameObject GetUIObject(EnumUIType _uiType)
    {
        GameObject _retObj = null;
        if (!dicOpenUIs.TryGetValue(_uiType,out _retObj))
        {
            throw new Exception("dicOpenUIs TryGetValue Failure! _uiType:" + _uiType.ToString());
        }
        return _retObj;
    }
    #endregion
    #region Preload UI Prefab By EnumUIType
    public void PreloadUI(EnumUIType[] _uiType)
    {
        for (int i = 0; i < _uiType.Length; i++)
        {
            PreloadUI(_uiType[i]);
        }
    }
    public void PreloadUI(EnumUIType _uiType)
    {
        string path = UIPathDefines.GetPrefabPathByType(_uiType);
        Resources.Load(path);
    }
    #endregion
    #region Open UI By EnumUIType

    /// <summary>
    /// 打开界面
    /// </summary>
    /// <param name="uiType"></param>
    public void OpenUI(EnumUIType[] uiTypes)
    {
        OpenUI(false, uiTypes, null);
    }
    public void OpenUI(EnumUIType uiType ,params object[] uiObject)
    {
        EnumUIType[] uiTypes = new EnumUIType[1];
        uiTypes[0] = uiType;
        OpenUI(false, uiTypes, uiObject);
    }
    public void OpenUICloseOthers(EnumUIType[] uiTypes)
    {
        // OpenUI(true )
        OpenUI(true, uiTypes, null);
    }
    public void OpenUICloseOthers(EnumUIType uitype,params object[] uiObjParams)
    {
        EnumUIType[] uiTypes = new EnumUIType[1];
        uiTypes[0] = uitype;
        OpenUI(true, uiTypes, uiObjParams);
    }
    public void OpenUI(bool _isCloseOthers,EnumUIType[] _uiTypes,params object [] _uiParams)
    {
        if (_isCloseOthers)
        {
            CloseUIAll();
        }
        for (int i = 0; i < _uiTypes.Length; i++)
        {
            EnumUIType _uiType = _uiTypes[i];
            if (!dicOpenUIs.ContainsKey(_uiType))
            {
                string _path = UIPathDefines.GetPrefabPathByType(_uiType);
                stackOpenUIs.Push(new UIInfoData(_uiType, _path, _uiParams));
            }
        }
        if (stackOpenUIs.Count >0)
        {
            CoroutineController.Instance.StartCoroutine(AsyncLoadData());
        }
    }
    private IEnumerator<int> AsyncLoadData()
    {

        UIInfoData _uiInfoData = null;
        UnityEngine.Object _prefabobj = null;
        GameObject _uiObject = null;
        if (stackOpenUIs!=null&&stackOpenUIs.Count>0)
        {
            do
            {
                _uiInfoData = stackOpenUIs.Pop();
                //Debug.Log(_uiInfoData.Path);
                _prefabobj = Resources.Load(_uiInfoData.Path);
                if (_prefabobj!=null)
                {
                    _uiObject = MonoBehaviour.Instantiate(_prefabobj,GameObject.FindObjectOfType<Canvas>().transform,false) as GameObject;
                   // Debug.Log(_uiObject.GetComponent<BaseUI>());
                    BaseUI _baseUI = _uiObject.GetComponent<BaseUI>();
                    if (_baseUI==null)
                    {
                        _baseUI = _uiObject.AddComponent(_uiInfoData.ScriptType) as BaseUI;

                    }
                    if (_baseUI!=null)
                    {
                        _baseUI.SetUIWhenOpening(_uiInfoData.UIParams);
                    }
                    dicOpenUIs.Add(_uiInfoData.UIType, _uiObject);
                }
            } while (stackOpenUIs.Count >0);
        }
        yield return 0;
    }
    #endregion
    #region Close UI By EnumUIType
    /// <summary>
    /// 关闭界面
    /// </summary>
    /// <param name="_uiType"></param>
    public void CloseUI(EnumUIType _uiType)
    {
        GameObject _uiObj = null;
        if (!dicOpenUIs.TryGetValue(_uiType,out _uiObj))
        {
            return;
        }
        CloseUI(_uiType, _uiObj);
    }

    public void CloseUI(EnumUIType[] _uiTypes)
    {
        for (int i = 0; i < _uiTypes.Length; i++)
        {
            CloseUI(_uiTypes[i]);
        }
    }


    /// <summary>
    /// 关闭所有UI界面
    /// </summary>
    public void CloseUIAll()
    {
        List<EnumUIType> _keyList = new List<EnumUIType>(dicOpenUIs.Keys);
        for (int i = 0; i < _keyList.Count; i++)
        {
            GameObject _uiObj = dicOpenUIs[_keyList[i]];
            CloseUI(_keyList[i], _uiObj);
        }
        dicOpenUIs.Clear();
    }
    private void CloseUI(EnumUIType _uiType,GameObject _uiObj)
    {
        if (_uiObj==null)
        {
            dicOpenUIs.Remove(_uiType);
        }
        else
        {
            BaseUI _baseUI = _uiObj.GetComponent<BaseUI>();
            if (_baseUI!=null)
            {
                _baseUI.StateChanged += CloseUIHandler;
                _baseUI.Release();
            }
            else
            {
                GameObject.Destroy(_uiObj);
                dicOpenUIs.Remove(_uiType);
            }
        }
    }
    private void CloseUIHandler(object _sender,EnumObjectState  _newState,EnumObjectState _oldState)
    {
        if (_newState ==EnumObjectState.Closing )
        {
            BaseUI _baseUI = _sender as BaseUI;
            dicOpenUIs.Remove(_baseUI.getUIType());
            _baseUI.StateChanged -= CloseUIHandler;
        }
    }
    #endregion

}
