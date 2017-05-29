using System.Collections;
using System.Collections.Generic;
using UnityEngine;




#region Global delegate 全局委托
public delegate void StateChanageEvent(object sender, EnumObjectState newState, EnumObjectState oldState);
#endregion 

#region Globaln enum 全局枚举
public enum EnumObjectState
{
    None,
    Initial,
    Loading,
    Ready,
    Disabled,
    Closing
}
public enum EnumUIType : int
{
    None=-1,
    LobbyMianPanel=0,
    ServerRoomListPanel=1,
    RoomPanel=2,
    CharacterChoosePanel=3,
    ConnetInfoPanel=4
}
public enum EnumSceneType
{
    None=0,
    StartGame,
    LoadingScene,
    LoginScene,
    
}
public static class UIPathDefines
{
    /// <summary>
    /// UI预设
    /// </summary>
    public const string UI_PREFAB = "UIPrefabs/";
    /// <summary>
    /// UI小控件
    /// </summary>
    public const string UI_CONTROLS_PREFAB = "UIPrefab/Control/";
    /// <summary>
    /// UI子页面
    /// </summary>
    public const string UI_SUBUI_PREFAB = "UIPrefab/SubUI";
    /// <summary>
    /// Icon路径
    /// </summary>
    public const string UI_IOCN_PATH = "UI/Icon/";

    public static string GetPrefabPathByType(EnumUIType _uiType)
    {
        string _path = string.Empty;
        switch (_uiType)
        {
            case EnumUIType.None:
                break;
            case EnumUIType.LobbyMianPanel:
                _path = UI_PREFAB + "LobbyMianPanel";
                break;
            case EnumUIType.ServerRoomListPanel:
                _path = UI_PREFAB + "ServerRoomListPanel";
                break;
            case EnumUIType.RoomPanel:
                _path = UI_PREFAB + "RoomPanel";
                break;
            case EnumUIType.CharacterChoosePanel:
                _path = UI_PREFAB + "CharacterChoosePanel";
                break;
            case EnumUIType.ConnetInfoPanel:
                _path = UI_PREFAB + "ConnetInfoPanel";
                break;
            default:
                break;
        }
        return _path;
    }
    public static System.Type GetUIScriptByType(EnumUIType _uiType)
    {
        System.Type _scriptType = null;
        switch (_uiType)
        {
            case EnumUIType.LobbyMianPanel:
                _scriptType = typeof(LobbyMianPanel);
                break;
            case EnumUIType.ServerRoomListPanel:
                _scriptType = typeof(ServerRoomListPanel);
                break;
            case EnumUIType.None:
                break;
            case EnumUIType.RoomPanel:
                _scriptType = typeof(RoomPanel);
                break;
            case EnumUIType.CharacterChoosePanel:
                _scriptType = typeof(CharacterChoosePanel);
                break;
           
            default:
                Debug.Log("Not Find EnumUIType! type" + _uiType.ToString());
                break;
        }
        return _scriptType;
    }

}
#endregion



public class Defines : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
