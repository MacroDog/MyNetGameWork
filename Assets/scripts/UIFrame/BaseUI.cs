using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUI : MonoBehaviour
{
    #region Cache gameObject& transfrom
    private Transform _cachedTransform;

    
    public Transform CachedTransform
    {
        get
        {
            if (!_cachedTransform)
            {
                _cachedTransform = this.transform;
            }
            return _cachedTransform;
        }
    }
    private GameObject _cachedGameObject;
    public GameObject CachedGameObject
    {
        get
        {
            if (!_cachedGameObject)
            {
                _cachedGameObject = this.gameObject;
            }
            return _cachedGameObject; 
        }
    }

    #endregion
    #region UIType&EnumObjectState
    /// <summary>
    /// The state
    /// </summary>
    protected EnumObjectState state = EnumObjectState.None;

    public event StateChanageEvent StateChanged;

    public EnumObjectState State
    {
        protected set
        {
            if (value !=state )
            {
                EnumObjectState oldState = state;
                state = value;
                if (StateChanged!=null)
                {
                    StateChanged(this, state, oldState);
                }
            }
        }
        get
        {
            return this.state;
        }
    }
    public abstract EnumUIType getUIType();
    #endregion
    void Start()
    {
        OnStart(); 
    }
    void Awake()
    {
        OnAwake();
    }
    void Update()
    {
        if (EnumObjectState.Ready==this.state)
        {
            OnUpdate(Time.deltaTime);
        }
    }

    public void Release()
    {
        this.State = EnumObjectState.Closing;
        GameObject.Destroy(CachedGameObject);
        OnRelease();   
    }
    protected virtual void OnStart()
    {

    }
    protected virtual void OnAwake()
    {
        this.State = EnumObjectState.Loading;
        //播放打开UI音乐
        this.OnPlayOpenUIAudio();
    }
    protected virtual void OnUpdate(float deltaTime)
    {

    }
    protected virtual void OnRelease()
    {
        this.OnPlayOpenUIAudio();
    }
    /// <summary>
    /// 播放打开界面音乐
    /// </summary>
    protected virtual void OnPlayOpenUIAudio()
    {

    }
    /// <summary>
    /// 播放关闭界面音乐
    /// </summary>
    protected virtual void OnPlayCloseUIAudio()
    {

    }
    protected virtual void SetUI(params object[] uiParams)
    {
        this.State = EnumObjectState.Loading;
    }
    public virtual void SetUIparam(params object[] uiParams)
    {

    }
    protected virtual void OnLoadData()
    {

    }
    public void SetUIWhenOpening(params object[] uiParams)
    {
        SetUI(uiParams);
        CoroutineController.Instance.StartCoroutine(AsyncOnLoadData());

    }
    private IEnumerator AsyncOnLoadData() 
    {
        yield return new WaitForSeconds(0);
        if (this.State==EnumObjectState.Loading)
        {
            this.OnLoadData();
            this.State = EnumObjectState.Ready;
        }
    }
}
