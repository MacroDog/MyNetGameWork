
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class chosePanel : MonoBehaviour
{
    [SerializeField]
    private NetPlayer Player;
    [SerializeField]
    private GameObject ChangePlayer;
	void Start ()
    {
        NetPlayer[] players = GameObject.FindObjectsOfType<NetPlayer>();
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].isLocalPlayer)
            {
                Player = players[i];
                break;
            }
        }
        
        
       
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    //public void CreatPlayerC()
    //{
    //    if (Player != null)
    //    {
    //        Player.CmdCreatCharacter();
    //        Destroy(this.gameObject);
    //    }
    //}
    //public  void changePlayerc()
    //{
    //    if (Player != null)
    //    {
    //        Player.changeCharacter(ChangePlayer);
    //        Debug.Log("change");
    //        CreatPlayerC();
    //    }
    //}
}
