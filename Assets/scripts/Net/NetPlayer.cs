using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class NetPlayer : NetworkLobbyPlayer
{
    public GameObject ChoseCharacter;//选择的角色
    private int id;//id
    public string PlayerName;
    private GameObject Character;
    public int Hero;//加载预制体编号

    ///// <summary>
    ///// 创建角色
    ///// </summary>
    //[Command]
    //public void CmdCreatCharacter()
    //{
    //    if (ChoseCharacter != null && Character == null)
    //    {
    //        var go = (GameObject)Instantiate(ChoseCharacter, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
    //        go.GetComponent<NetPlayerSetting>().user = this;
    //        NetworkServer.Spawn(go);
    //    }
    //    if (Character != null)
    //    {
    //        NetworkServer.Destroy(Character);
    //        NetworkServer.Spawn(ChoseCharacter);
    //        Character = ChoseCharacter;
    //    }

    //}


    ///// <summary>
    ///// 改变角色
    ///// </summary>
    ///// <param name="sf"></param>
    //public void changeCharacter(GameObject sf)
    //{
    //    if (sf.GetComponent<NetworkIdentity>() != null)
    //    {
    //        return;
    //    }
    //    else
    //    {
    //        ChoseCharacter = sf;
    //    }
    //}




}
