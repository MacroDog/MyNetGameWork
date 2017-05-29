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
            Fire();
        }
    }


    /// <summary>
    /// 
    /// </summary>
    private void Fire()
    {
        RaycastHit hit;
        if (Physics.Raycast(shootView.transform.position,shootView.transform.forward,out hit, 100))
        {
          
            if (hit.collider.tag=="Player")
            {
                hit.collider.GetComponent<NetCharacter>().GetDamage(10);

            }
            Debug.Log(hit.collider.name);
        }
    }

    [Command]
    private void CmdPlayer(string _playerID,int _damage)
    {
        Debug.Log(_playerID + " " + _damage);
        GameManager.Instance.GetNetCharacter(_playerID).GetDamage(_damage);

    }
}
