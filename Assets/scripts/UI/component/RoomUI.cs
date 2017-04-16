using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RoomUI : MonoBehaviour
{
    public Button Jion;
    public Text PlayerNumber;
    public Text RoomName;


    public void Init(string name,int playerNumber)
    {
        RoomName.text = name;
        PlayerNumber.text = playerNumber.ToString();
    }
    public void Init(string name)
    {
        RoomName.text = name;
    }

   
}
