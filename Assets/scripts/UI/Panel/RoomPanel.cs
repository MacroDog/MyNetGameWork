using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
/// <summary>
/// 房间面板 用于显示当前房间情况 房间名 玩家数量等等
/// </summary>
public class RoomPanel : BaseUI
{

    [SerializeField]
    private ScrollRect playerRect;//玩家list

    [SerializeField]
    private PlayerUI PlayerUI;//玩家信息UI
    private List<MyNetLobbyPlayer> playerList;
    //private Dictionary<PlayerUI,MyNetLobbyPlayer> playerUIs;
    private MyNetManager netmanage;
    protected override void OnStart()
    {
        netmanage = GameObject.FindObjectOfType<MyNetManager>();
        // playerUIs = new Dictionary<PlayerUI, MyNetLobbyPlayer>();
        Init();
    }
    private float Ypos = 0;

    protected override void OnUpdate(float deltaTime)
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("刷新");
            Freshen();
        }
    }


    /// <summary>
    /// 房主移除玩家
    /// </summary>
    public void RemovePlayer()
    {

    }


    /// <summary>
    /// 初始化
    /// </summary>
    void Init()
    {

        if (GameObject.FindObjectOfType<MyNetManager>())
        {
            netmanage = GameObject.FindObjectOfType<MyNetManager>();
        }
        else
        {
            return;
        }
        netmanage.Flash();
        playerList = netmanage.LobbyPlayers;
        Debug.Log(netmanage.LobbyPlayers.Count);

        Debug.Log(netmanage.lobbySlots.Length);
        // Debug.Log(netmanage)
        for (int i = 0; i < playerList.Count; i++)
        {
            AddPlayer(playerList[i]);
        }

    }

    private void Freshen()
    {
        if (netmanage != null)
        {
            netmanage.Flash();
            playerList = netmanage.LobbyPlayers;
            Debug.Log(playerList.Count);
            PlayerUI[] temp = playerRect.content.GetComponentsInChildren<PlayerUI>();
            for (int i = 0; i < temp.Length; i++)
            {
                Destroy(temp[i]);
            }


            for (int i = 0; i < playerList.Count; i++)
            {
                AddPlayer(playerList[i]);
            }
        }
    }
    public override EnumUIType getUIType()
    {
        return EnumUIType.RoomPanel;
    }

    private void AddPlayer(MyNetLobbyPlayer lobbyPlayer)
    {
        GameObject ga = Instantiate(PlayerUI.gameObject, playerRect.content.transform, false) as GameObject;
        ga.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 100);
        ga.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, Ypos, 0);
        Ypos -= (int)ga.GetComponent<RectTransform>().rect.height;
        ga.GetComponent<PlayerUI>().Init(lobbyPlayer);
        if (lobbyPlayer.isLocalPlayer==true)
        {
            Debug.Log(lobbyPlayer.netId);
        }
        else
        {
            Debug.Log("Not Local Player" + lobbyPlayer.netId);
        }
        //playerUIs.Add(ga.GetComponent<PlayerUI>(), lobbyPlayer);

    }

}
