using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterChoosePanel : BaseUI
{

    private MyNetLobbyPlayer localPlayer;
   // private 
    public override EnumUIType getUIType()
    {
        return EnumUIType.CharacterChoosePanel;
    }

    protected override void OnStart()
    {
        
    }
    public void Init(MyNetLobbyPlayer player)
    {
        
    }


    /// <summary>
    /// 得到数据
    /// </summary>
    /// <param name="uiParams"></param>
    protected override void SetUI(params object[] uiParams)
    {
        if (uiParams[0] != null)
        {
            localPlayer = uiParams[0] as MyNetLobbyPlayer;
        }
    }
    /// <summary>
    /// 改变loobyplayer选择的character
    /// </summary>
    /// <param name="i"></param>
    public void ChoseCharacter(int i)
    {
        localPlayer.CmdChangeCharacter(i);
    }
    /// <summary>
    /// 关闭面板
    /// </summary>
    public void Close()
    {
        UIManager.Instance.CloseUI(EnumUIType.CharacterChoosePanel);
    }
}
