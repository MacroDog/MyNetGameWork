using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum AmmoType
{
    handgun,
    rifle,
}
public class Ammo : Equipments
{
    public  AmmoType _AmmoType;
    private AmmoType ammoType;
    public  AmmoType MyAmmoType
    {
        get
        {
            return ammoType;
        }
    }
	// Use this for initialization
	void Start () {
        ammoType = _AmmoType;

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
