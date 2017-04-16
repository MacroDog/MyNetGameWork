using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
public class LobbyMianPanel : BaseUI
{

    public MyNetManager LobbyNetManager;
    public InputField CreatRoomName;//创建的房间的名字
    public InputField playername;//玩家姓名
    public RectTransform serverRoomListPanel;//服务器房间


    public override EnumUIType getUIType()
    {
        return EnumUIType.LobbyMianPanel;
    }


    /// <summary>
    /// 创建房间
    /// </summary>
    public void OnClickCreatMacthRoom()
    {
        LobbyNetManager.StartMatchMaker();
        LobbyNetManager.matchMaker.CreateMatch(CreatRoomName.text, (uint)LobbyNetManager.maxPlayers, true, "", "", "", 0, 0,
               LobbyNetManager.OnMatchCreate);
        Debug.Log("begin");
       // UIManager.Instance.OpenUICloseOthers(EnumUIType.RoomPanel);
    }
    /// <summary>
    /// 得到匹配的房间列表
    /// </summary>
    public void OnClickListRoom()
    {
        LobbyNetManager.StartMatchMaker();
        LobbyNetManager.matchMaker.ListMatches(0, 6, "", true, 0, 0, ShowRoom);
        Debug.Log("List");
    }
    protected override void OnStart()
    {
        LobbyNetManager = GameObject.FindObjectOfType<MyNetManager>();
    }

    public void ShowRoom(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        Debug.Log(matches.Count);

        for (int i = 0; i < matches.Count; i++)
        {
            Debug.Log(matches[i].name + " " + matches[i].networkId + " " + matches[i].currentSize.ToString());
        }
        UIManager.Instance.OpenUICloseOthers(EnumUIType.ServerRoomListPanel);

    }
   

}

