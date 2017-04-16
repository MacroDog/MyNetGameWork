using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class MyNetLobbyPlayer : NetworkLobbyPlayer
{



    public Action<string, int,bool> Refresh;//UI刷新数据
 
    
 
    /// <summary>
    /// 名字
    /// </summary>
    private string id="player1";
    
    public string ID
    {
        get
        {
            return id;
        }
    }

    public int CharacterID
    {
        get
        {
            return characterID;
        }
    }
    private int characterID=0;

    /// <summary>
    /// 选择的角色
    /// </summary>
    private GameObject characterPlayer;
    public GameObject CharacterPlayer
    {
        get
        {
            return characterPlayer;
        }
    }


    [Command]
    /// <summary>
    /// 改变选择的角色
    /// </summary>
    /// <param name="ga"></param>
    public void CmdChangeCharacter(int i)
    {
        characterID = i;
        Refresh(name, characterID,readyToBegin);
    }

    [Command]
    public void CmdChangeName(string name)
    {
        id = name;
        Refresh(name, characterID, readyToBegin);
    }
   
    public override void OnClientReady(bool readyState)
    {
        base.OnClientReady(readyState);
        Refresh(name, characterID, readyToBegin);
    }
    
    /// <summary>
    /// 点击准备按钮
    /// </summary>
    public void OnClientReady()
    {
        if (readyToBegin)
        {
            SendNotReadyToBeginMessage();
            Refresh(name, characterID, false);
        }
        else
        {
            SendReadyToBeginMessage();
        }
        
        
    }
}
