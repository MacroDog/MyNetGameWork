using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ConnetInfoPanel : BaseUI
{
    [SerializeField]
    private Button cancel;
    [SerializeField]
   

    public void setCancel(UnityEngine.Events.UnityAction buttonClbk)
    {

    }
    public override EnumUIType getUIType()
    {
        return EnumUIType.ConnetInfoPanel;
    }

}
