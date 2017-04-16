using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class GameTest : NetworkBehaviour
{

    public MyNetManager mnm;
    public NetworkIdentity Player;
    //public void creat
    
    /// <summary>
    /// 创建匹配房间
    /// </summary>
    public void CreatRoom()
    {
        mnm.StartMatchMaker();
        //mnm.matchMaker.CreateMatch()
    }
}
