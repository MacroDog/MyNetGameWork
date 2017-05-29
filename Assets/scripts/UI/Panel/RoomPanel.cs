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
    private PlayerUI PlayerUI;//玩家信息UI预制体
    [SerializeField]
    private Text playerNumber;//当前玩家信息
    [SerializeField]
    private Button freshenButton;//刷新按钮
    [SerializeField]
    private Button beginGameButton;
    [SerializeField]
    private Button backButton;
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
            netmanage.FrenshList();
            freshenButton.onClick.AddListener(() => { Freshen(); });
            beginGameButton.onClick.AddListener(() => { OnClinkBeginButton(); });
            backButton.onClick.AddListener(() => { OnBack(); });
            //netmanage.OnLoobyPlayerCreat += Freshen;
            if (netmanage.IsHost ==true)
            {
                beginGameButton.interactable = true;
            }
            else
            {
                beginGameButton.interactable = false;
            }
        }
        else
        {
            return;
        }
       
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
        Ypos = 0;
        if (netmanage != null)
        {
            netmanage.FrenshList();
            playerList = netmanage.LobbyPlayers;
            Debug.Log(playerList.Count);
            PlayerUI[] temp = playerRect.content.GetComponentsInChildren<PlayerUI>();
            Debug.Log(temp.Length);
            for (int i = 0; i < temp.Length; i++)
            {
                Destroy(temp[i].gameObject);
            }
            for (int i = 0; i < playerList.Count; i++)
            {
                AddPlayer(playerList[i]);
            }
            playerNumber.text = playerList.Count.ToString();
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

    /// <summary>
    /// 开始游戏
    /// </summary>
    public void OnClinkBeginButton()
    {
        if (netmanage.IsHost)
        {
            netmanage.CheckReadyToBegin();
        }
    }

    public void OnBack()
    {
        //if (netmanage.IsHost)
        //{
        //    netmanage.StopHost();
        //}
        //else
        //{
        //    netmanage.StopClient();
        //}

        netmanage.StopLobby();
        
    }
}
