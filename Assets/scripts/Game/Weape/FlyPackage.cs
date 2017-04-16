using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyPackage : MonoBehaviour {
    private float CD;//使用CD

    private float cdTime;//当前使用剩余cd时间
    [SerializeField]
    private float upForce;
    [SerializeField]
    private float forwardForce;
    private bool isUes = false;
    private Rigidbody Player;

    private bool canUse;// 是否能够使用
	// Use this for initialization
	void Start () {
        Player = transform.parent.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isUes = true;
        }
	}

    void FixedUpdate()
    {
        if (isUes==true)
        {
            JumpingFly();
        }
    }
    private void JumpingFly()
    {
        Vector3 temp = Player.transform.forward * forwardForce + Player.transform.up * upForce;
        Player.AddForce(temp, ForceMode.Impulse);
        // Player.velocity = new Vector3(Player.velocity.x, 30f, Player.velocity.z);
        Debug.Log("Jumpping");
        isUes = false;
    } 
}
