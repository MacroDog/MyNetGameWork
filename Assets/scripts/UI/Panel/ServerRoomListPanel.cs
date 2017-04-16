using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
public class ServerRoomListPanel : BaseUI
{

    public MyNetManager NetManager;
    public ScrollRect RoomScroll;
    public GameObject RoomUI;
    protected int currentPage = 0;
    protected int previousPage = 0;

    private int Ypos = 0;
   

    public void Init()
    {
        NetManager = GameObject.FindObjectOfType<MyNetManager>();
        GetServerPage(0);

    }

    protected override void OnStart()
    {
        Init();
    }
    /// <summary>
    /// 得到服务器房间列表
    /// </summary>
    /// <param name="success"></param>
    /// <param name="extendedInfo"></param>
    /// <param name="matches"></param>
    public void OnShowMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        Debug.Log(matches.Count);
        if (matches.Count == 0)
        {
            currentPage = previousPage;
            return;
        }
        for (int i = 0; i < RoomScroll.content.GetComponentsInChildren<RoomUI>().Length; i++)
        {
            Destroy(RoomScroll.content.GetComponentsInChildren<RoomUI>()[i]);
        }
        Ypos = 0;
        for (int i = 0; i < matches.Count; i++)
        {
            AddRoom(matches[i]);
        }
    }

    /// <summary>
    /// 跳转到分页
    /// </summary>
    /// <param name="page"></param>
    public void GetServerPage(int page)
    {
        previousPage = currentPage;
        currentPage = page;
        NetManager.matchMaker.ListMatches(page, 6, "", true, 0, 0, OnShowMatchList);
        Debug.Log("get list"+page);
    }



    /// <summary>
    /// 增加房间
    /// </summary>
    /// <param name="match"></param>
    private void AddRoom(MatchInfoSnapshot match)
    {
        GameObject ga = Instantiate(RoomUI, RoomScroll.content, false) as GameObject;
        ga.GetComponent<RectTransform>().sizeDelta= new Vector2(0,100);
        ga.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, Ypos, 0);
        Ypos -= (int)ga.GetComponent<RectTransform>().rect.height;
        ga.GetComponent<RoomUI>().Jion.onClick.AddListener(() => { JionRoom(match); });
        Debug.Log(match.name);
        ga.GetComponent<RoomUI>().Init(match.name);
        Debug.Log("添加房间  " + match.name+ga.GetComponent<RectTransform>().sizeDelta);
    }


    /// <summary>
    /// 加入房间方法
    /// </summary>
    public void JionRoom(MatchInfoSnapshot match)
    {
        if (NetManager == null)
        {
            return;
        }
        else
        {
            NetManager.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, NetManager.OnMatchJoined);
            UIManager.Instance.OpenUICloseOthers(EnumUIType.RoomPanel);
        }
    }

    public override EnumUIType getUIType()
    {
        return EnumUIType.ServerRoomListPanel;
    }
}
