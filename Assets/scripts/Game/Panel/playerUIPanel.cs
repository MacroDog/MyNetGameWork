using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class playerUIPanel : MonoBehaviour
{
    public Text Hp;
    public bool havePlayer = false;
    private GameObject[] players;
    private NetCharacter player;
	// Use this for initialization
	void Start () {
       
	}
	void Init()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        if (players != null)
        {
            for (int i = 0; i < players.Length - 1; i++)
            {
                if (players[i].GetComponent<NetCharacter>().isLocalPlayer == true)
                {
                    player = players[i].GetComponent<NetCharacter>();
                    havePlayer = true;
                    break;
                }
            }
        }
    }
	// Update is called once per frame
	void Update ()
    {
        if (havePlayer==false)
        {
            Init();
        }
        else
        {
            Hp.text = player.CharacterHp.ToString();
        }
	}
}
