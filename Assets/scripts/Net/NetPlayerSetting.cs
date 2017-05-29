using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class NetPlayerSetting : NetworkBehaviour
{
    [SerializeField]
    string remoteLayerName="RemotePlayer";
    [SerializeField]
    Behaviour[] ComponentsToDisable;
    [SerializeField]
    Camera sceneCamera;
    [SerializeField]

    private GameObject[] modle; 
   // public NetPlayer user = null;
   
    void Start()
    {
        Debug.Log(isLocalPlayer);
        if (!GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            //AssignRemoteLayer();
            DisableComponents();
        }
        else
        {
            
        }
        //RegisterPlayer();
    }
    void RegisterPlayer()
    {
        string _ID = "Player" + this.GetComponent<NetworkIdentity>().netId;
        transform.name = _ID;
    }



    void AssignRemoteLayer()
    {
       // gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    /// <summary>
    /// 
    /// </summary>
    void DisableComponents()
    {
        for (int i = 0; i < ComponentsToDisable.Length; i++)
        {
            ComponentsToDisable[i].enabled = false;

        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        string _netID = this.GetComponent<NetworkIdentity>().netId.ToString();
        NetCharacter _player = GetComponent<NetCharacter>();
        GameManager.Instance.RegisterPlayer(_netID, _player);
    }
    void OnDisable()
    {
        GameManager.Instance.RemovePlayer(GetComponent<NetworkIdentity>().netId.ToString());
    }
}
