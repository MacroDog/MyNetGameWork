using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class testWeapon : NetworkBehaviour
{
    public Camera shootView;//设计摄像机
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log("fire");
            CmdFire();
        }
    }

    [Command]
    private void CmdFire()
    {
        RaycastHit hit;
        if (Physics.Raycast(shootView.transform.position,shootView.transform.forward,out hit, 100))
        {
            Debug.Log(hit.collider.name);
        }
    }
}
