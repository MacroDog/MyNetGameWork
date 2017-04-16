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
}
