using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
/// <summary>
/// 角色状态
/// </summary>
/// 
public enum CharacterEnumState
{
    None,
    Alive,
    Death,
}
public class NetCharacter :NetworkBehaviour
{
    
    public Animator anim;
    public Action<int> OnGetHurt;//当收到伤害的时候
    private CharacterEnumState _state;
    [SerializeField]
    Behaviour[] ComponentsToDisable;
    [SerializeField]
    [SyncVar]
    private int characterHp=100;
    [SerializeField]
    private GameObject[] modles;
    public int CharacterHp
    {
        get
        {
            return characterHp;
        }
    }

    void Start()
    {
        Debug.Log(GetComponent<NetworkIdentity>().isLocalPlayer);
        if (!GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            //AssignRemoteLayer();
            DisableComponents();
            AssignRemoteLayer();
        }
        else
        {
           
        }
        //RegisterPlayer();
    }
    void Update()
    {

        anim.SetFloat("speed", GetComponent<Rigidbody>().velocity.x);
        
        
    }
    void AssignRemoteLayer()
    {
        for (int i = 0; i < modles.Length; i++)
        {
            
            foreach (Transform tran in modles[i].GetComponentsInChildren<Transform>())
            {//遍历当前物体及其所有子物体  
                tran.gameObject.layer = LayerMask.NameToLayer("RemotePlayer"); ;//更改物体的Layer层  
            }
        }
    }
    /// <summary>
    /// 收到伤害
    /// </summary>
    /// <param name="hurt"></param>
    public void GetDamage(int hurt)
    {
        int temp = characterHp;
        if (temp-hurt<=0)
        {
            _state = CharacterEnumState.Death;
        }
        else
        {
            characterHp -= hurt;
        }
        //OnGetHurt(hurt);
        Debug.Log(characterHp.ToString());
    }
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

    [Client]
    public void Shoot(NetworkIdentity netId, int damage)
    {
        CmdDamagePlayer( netId,  damage);
    }
    /// <summary>
    /// 对玩家造成伤害
    /// </summary>
    [Command]
    public void CmdDamagePlayer(NetworkIdentity netId,int damage)
    {
        NetCharacter player = GameManager.Instance.GetNetCharacter(netId.netId.ToString());
        player.GetDamage(damage);
    }

   
}
