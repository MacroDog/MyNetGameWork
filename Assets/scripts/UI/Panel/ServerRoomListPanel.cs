using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
public class ServerRoomListPanel : BaseUI
{
    
    private MyNetManager NetManager;
    [SerializeField]
    private ScrollRect RoomScroll;
    [SerializeField]
    private GameObject RoomUI;
    [SerializeField]
    private Button nextPage;
    [SerializeField]
    private Button lastPage;
    [SerializeField]
    private Button refresh;
    [SerializeField]
    private Button backBuuton;
    private int nowPage=0;
    private int Ypos = 0;
   

    public void Init()
    {
        NetManager = GameObject.FindObjectOfType<MyNetManager>();
        nextPage.onClick.AddListener(() => { GetNextPage(); });
        lastPage.onClick.AddListener(() =>{ GetLastPage(); });
        refresh.onClick.AddListener(() => { Refresh(); });
        backBuuton.onClick.AddListener(() => { Back(); });
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
    private void OnShowMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        NetManager.isMatch = false;
        Debug.Log(matches.Count);
        if (matches.Count == 0)
        {
          
            return;
        }
        for (int i = 0; i < RoomScroll.content.GetComponentsInChildren<RoomUI>().Length; i++)//摧毁之前的RoomUI
        {
            Destroy(RoomScroll.content.GetComponentsInChildren<RoomUI>()[i].gameObject);
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
    private void GetServerPage(int page)
    {
        if (page>=0)
        {
            NetManager.matchMaker.ListMatches(page, 6, "", true, 0, 0, OnShowMatchList);
            NetManager.isMatch = true;
            nowPage = page;
            Debug.Log("get list" + page);
        }
       
    }
    /// <summary>
    /// 增加房间
    /// </summary>
    /// <param name="match"></param>
    private void AddRoom(MatchInfoSnapshot match)
    {
        GameObject ga = Instantiate(RoomUI, RoomScroll.content, false) as GameObject;
        ga.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 100);
        ga.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, Ypos, 0);
        Ypos -= (int)ga.GetComponent<RectTransform>().rect.height;
        ga.GetComponent<RoomUI>().Jion.onClick.AddListener(() => { JionRoom(match); });
        Debug.Log(match.name + match.currentSize);
        ga.GetComponent<RoomUI>().Init(match.name,match.currentSize,match.maxSize);
        Debug.Log("添加房间  " + match.name + ga.GetComponent<RectTransform>().sizeDelta);
    }

    public void Refresh()
    {
        GetServerPage(nowPage);
    }
    public void GetNextPage()
    {
        GetServerPage(nowPage + 1);
    }
    public void GetLastPage()
    {
        GetServerPage(nowPage - 1);
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

    public void Back()
    {
        UIManager.Instance.OpenUICloseOthers(EnumUIType.LobbyMianPanel);
    }
    public override EnumUIType getUIType()
    {
        return EnumUIType.ServerRoomListPanel;
    }
}
