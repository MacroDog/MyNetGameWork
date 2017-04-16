using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class MyNetManager :NetworkLobbyManager
{


    /// <summary>
    /// 大厅中的玩家
    /// </summary>
    //Dictionary< NetworkConnection,MyNetLobbyPlayer> lobbyPlayers=new Dictionary<NetworkConnection, MyNetLobbyPlayer> ();
    //public Dictionary<NetworkConnection, MyNetLobbyPlayer> LobbyPlayers
    //{
    //    get
    //    {
    //        return lobbyPlayers;
    //    }
    //}
    //public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    //{
    //    GameObject go = null;
    //    if (spawnPrefabs.Count > 0)
    //        go = spawnPrefabs[spawnPrefabs.Count - 1];
    //    else
    //        go = playerPrefab;

    //    var player = (GameObject)GameObject.Instantiate(go,new Vector3(0,0,0), Quaternion.identity);
    //    NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    //}

    List<MyNetLobbyPlayer> lobbyPlayers;
    public List<MyNetLobbyPlayer> LobbyPlayers
    {
        get
        {
            return lobbyPlayers;
        }
    }
    public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
    {
        lobbyPlayers =new List<MyNetLobbyPlayer>();
        Debug.Log("Creat looby player");
        var temp = Instantiate(lobbyPlayerPrefab.gameObject) as GameObject;
        return temp;
    }
    public override void OnLobbyClientConnect(NetworkConnection conn)
    {
        lobbyPlayers = new List<MyNetLobbyPlayer>(GameObject.FindObjectsOfType<MyNetLobbyPlayer>());

        base.OnLobbyClientConnect(conn);
        Debug.Log(lobbyPlayers.Count);

    }
    public override void OnLobbyClientAddPlayerFailed()
    {
       
        base.OnLobbyClientAddPlayerFailed();
    }

    /// <summary>
    /// 产生角色
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="playerControllerId"></param>
    /// <returns></returns>
    public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject palyer = null;
        for (int i = 0; i < lobbyPlayers.Count; i++)
        {
            if (lobbyPlayers[i].connectionToClient==conn||lobbyPlayers[i].connectionToServer==conn)
            {
                 palyer = GameObject.Instantiate(spawnPrefabs[lobbyPlayers[i].CharacterID], startPositions[conn.connectionId].position, Quaternion.identity);
                break;
            }
        }
        Debug.Log("Creat Player netid="+ palyer.GetComponent<NetworkIdentity>().netId+" "+ palyer.name);
      
        return palyer;
    }

    
    public override void OnLobbyClientSceneChanged(NetworkConnection cont)
    {
        //Debug.Log("change");
    }

    /// <summary>
    /// 回调函数 当当房间被创建完成时调用
    /// </summary>
    /// <param name="success"></param>
    /// <param name="extendedInfo"></param>
    /// <param name="matchInfo"></param>
    public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        base.OnMatchCreate(success, extendedInfo, matchInfo);
        //TryToAddPlayer();
        Debug.Log("OnMatchCreate");
        UIManager.Instance.OpenUICloseOthers(EnumUIType.RoomPanel);
       
    }

    /// <summary>
    /// 当以主机开始的时候
    /// </summary>
    public override void OnStartHost()
    {
        base.OnStartHost();
        Debug.Log("主机创建完成房间");
       
    }
    
    public void Flash()
    {
        lobbyPlayers = new List<MyNetLobbyPlayer>(GameObject.FindObjectsOfType<MyNetLobbyPlayer>());
        Debug.Log(lobbyPlayers.Count);
    }
    



}
