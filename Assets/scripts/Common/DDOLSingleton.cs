using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDOLSingleton<T> : MonoBehaviour where　T:DDOLSingleton<T>
{
    protected static T _instance = null;
    public static T Instance
    {
        get
        {
            if (_instance==null)
            {
                GameObject ga = GameObject.Find("DDOLGameObject");
                if (ga==null)
                {
                    ga = new GameObject("DDOLGameObject");
                    if (ga==null)
                    {
                        ga = new GameObject("DDOLGameObject");
                        DontDestroyOnLoad(ga);
                    }
                    _instance = ga.AddComponent<T>();
                }
            }
            return _instance;
        }
    }


    /// <summary>
    /// 退出程序时清空内存
    /// </summary>
    private void OnApplicationQuit()
    {
        _instance = null;
    }


}
