using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    public static GameManager Instance
    {
        get
        {
            if (_instance==null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    GameObject temp = new GameObject("GameManager");
                    _instance= temp.AddComponent<GameManager>();   
                }
            }
            return _instance;
            
        }
    }
    private Dictionary<string, NetCharacter> players=new Dictionary<string, NetCharacter> ();
    public Dictionary<string, NetCharacter> Players
    {
        get
        {
            return players;
        }
    }
    public void RegisterPlayer( string _netId,NetCharacter player)
    {
        players.Add(_netId, player);
        
    }
    public void UnRegisterPlayer(string _playerId)
    {
        players.Remove(_playerId);
    }
    public void RemovePlayer(string _netId)
    {
        players.Remove(_netId);
    }

    public NetCharacter GetNetCharacter(string netId)
    {
        NetCharacter temp;
        players.TryGetValue(netId, out temp);
        return temp;
    }

    private void OnApplicationQuit()
    {
        _instance = null;
        Destroy(this.gameObject);
    }
   
}
