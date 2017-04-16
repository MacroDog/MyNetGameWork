using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{


    public Text PlayerName;

    public Text HeroID;

    public Button ReadyButton;

    public Button ChangeButton;

    public MyNetLobbyPlayer Player;


   
    /// <summary>
    /// 初始化Player
    /// </summary>
    public void Init(MyNetLobbyPlayer player)
    {
        Player = player;
        PlayerName.text = player.ID;
        HeroID.text= "Character"+ player.CharacterID.ToString();
        player.Refresh += Refresh;
        if (Player.isLocalPlayer==false)
        {
            ChangeButton.interactable=false;
            ReadyButton.interactable=false;
        }
        else
        {
            ChangeButton.onClick.AddListener(()=>{ ChangeHero(); });
            ReadyButton.onClick.AddListener(() => { OnClentReady(); });
        }
    }


    /// <summary>
    /// 打开修改人物面板
    /// </summary>
    public void ChangeHero()
    {
        UIManager.Instance.OpenUI(EnumUIType.CharacterChoosePanel, Player);
    }



    /// <summary>
    /// 准备按钮
    /// </summary>
    public void OnClentReady()
    {
        Player.OnClientReady();
       
    }


    /// <summary>
    /// 点击准备按钮
    /// </summary>
    public void ClinetReady()
    {
        
        ReadyButton.GetComponentInChildren<Text>().text = "已准备";
    }


    /// <summary>
    /// 非LocaPlayer 取消准备的时候调用
    /// </summary>
    public void ClinetNoReady()
    {
        ReadyButton.GetComponentInChildren<Text>().text = "未准备";
    }

    /// <summary>
    /// 刷新UI部分数据
    /// </summary>
    /// <param name="name"></param>
    /// <param name="charactID"></param>
    /// <param name="isReady"></param>
    private void Refresh(string name, int charactID, bool isReady)
    {
        PlayerName .text= name;
        HeroID.text = "charact" + charactID.ToString();
        if (isReady==false)
        {
            ClinetNoReady();
        }
        else
        {
            ClinetReady();
        }
        
    }



}
