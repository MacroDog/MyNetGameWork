using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Player:NetworkBehaviour
{
    [SyncVar]
    private int health;
    public int Health
    {
        get
        {
            return health;
        }
    }


    [SyncVar]
    private bool isDeath = false;
    public bool IsDeath
    {
        get
        {
            return IsDeath;
        }
    }
    
}
