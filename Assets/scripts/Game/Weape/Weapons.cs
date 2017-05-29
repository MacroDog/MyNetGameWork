using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class Weapons : NetworkBehaviour
{
    [SerializeField]
    private Transform handGunPos;
    [SerializeField]
    private Transform riflePos;
    private GameObject UseWeapon;
    private Animator player;
    Dictionary<AmmoType, int> AmmosDic = new Dictionary<AmmoType, int>();
    int currentGun = -1;
    [SerializeField]
    Camera ShootCamera;
    public List<ArmControllerScript> weapons;
    public int MaxGuns;//最多含有的枪
    public int MaxDistance;
    // Use this for initialization

    void Start()
    {
        player = GetComponent<Animator>();
        ChangeTo(0);
    }
    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            //if (Input.GetKeyDown(KeyCode.Alpha1) && weapons.Count >= 1)
            //{
            //    ChangeTo(0);
            //}
            //if (Input.GetKeyDown(KeyCode.Alpha2) && weapons.Count >= 2)
            //{
            //    ChangeTo(1);
            //}
            //if (Input.GetKeyDown(KeyCode.Alpha3) && weapons.Count >= 3)
            //{
            //    ChangeTo(2);
            //}
            //RaycastHit hit;
            //if (Physics.Raycast(ShootCamera.transform.position, ShootCamera.transform.forward, out hit, 2f))
            //{
            //    Debug.Log(hit.collider.name);
            //    if (Input.GetKeyDown(KeyCode.E) && hit.collider.GetComponent<Equipments>())
            //    {
            //        addThings(hit.collider.GetComponent<Equipments>());
            //        Debug.Log("Change");
            //    }

            //}
            if (Input.GetMouseButtonDown(0))
            {
                weapons[currentGun].ClietFire();
            }
            if (Input.GetMouseButton(1))
            {
                weapons[currentGun].aim();
            }
            else
            {
                weapons[currentGun].CancelAim();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                weapons[currentGun].ReLoad();
            }
        }
       

    }

    /// <summary>
    /// 添加枪械
    /// </summary>
    /// <param name="gun"></param>
    public void addGun(ArmControllerScript gun)
    {
        if (weapons.Count < MaxGuns)
        {
            weapons.Add(gun);
        }
    }


    private void addThings(Equipments eq)
    {
        if (eq.GetComponent<Ammo>())
        {
            Ammo temp = eq.GetComponent<Ammo>();
            if (AmmosDic.ContainsKey(temp.MyAmmoType))
            {
                int am;
                AmmosDic.TryGetValue(temp.MyAmmoType, out am);
                AmmosDic.Remove(temp.MyAmmoType);
                AmmosDic.Add(temp.MyAmmoType, temp.quantity + am);
            }
        }
        if (eq.GetComponent<DropWeapon>())
        {
            if (weapons.Count < MaxGuns)
            {
                DropWeapon temp = eq.GetComponent<DropWeapon>();
                weapons.Add(Instantiate(temp.prefabs, ShootCamera.transform).GetComponent<ArmControllerScript>());
                Destroy(temp.gameObject);
                ChangeTo(weapons.Count - 1);//切换到捡起的枪械
            }
            else
            {
                DropWeapon temp = eq.GetComponent<DropWeapon>();
                Instantiate(weapons[currentGun].model, temp.transform.position, temp.transform.rotation);
                Destroy(weapons[currentGun].gameObject);
                weapons[currentGun] = Instantiate(temp.prefabs, weapons[currentGun].transform.parent).GetComponent<ArmControllerScript>();//替换手上的枪械
                CmdDestroy(temp.gameObject);
                ChangeTo(currentGun);
            }
        }

    }
    /// <summary>
    /// change Gun
    /// </summary>
    /// <param name="a"></param>
    public void ChangeTo(int a)
    {
        if (weapons[a] != null && a != currentGun)
        {
           // UseWeapon = weapons[a].model.gameObject;
            weapons[a].GetComponent<AimScript>().gunCamera = ShootCamera;
            if (isLocalPlayer)
            {
                weapons[a].gameObject.SetActive(true);
                weapons[a].user = this. GetComponent<NetCharacter>();
            }
            else
            {
                weapons[a].gameObject.SetActive(false);
            }
           
            weapons[a].GetComponent<Animator>().Play("Draw");
            currentGun = a;//当前枪械
            for (int i = 0; i < weapons.Count; i++)
            {
                if (i != a && weapons[i] != null)
                {
                    weapons[i].gameObject.SetActive(false);
                }
            }
            switch (weapons[a]._type)
            {
                
                case weaponType.HandGun:
                    player.SetInteger("GunType", 1);
                    break;
                case weaponType.Rifle:
                    player.SetInteger("GunType", 2);
                    break;
                default:
                    break;
            }
            CmdWeaponChange(a, weapons[a]._type);

        }
    }
    [Command]
    void CmdWeaponChange(int a ,weaponType _type)
    {
        RpcClientChange(a, _type);
    }
    [ClientRpc]
    void RpcClientChange(int a, weaponType _type)
    {
       
        if (UseWeapon != null)
        {
            Destroy(UseWeapon);
        }
        switch (_type)
        {
            case weaponType.HandGun:
                UseWeapon = Instantiate(weapons[a].model.gameObject, handGunPos.transform, handGunPos);
                UseWeapon.GetComponent<Transform>().position = new Vector3(0, 0, 0);
                if (UseWeapon.GetComponent<Rigidbody>())
                {
                    Destroy(UseWeapon.GetComponent<Rigidbody>());
                }
                UseWeapon.transform.rotation = handGunPos.rotation;
                break;
            case weaponType.Rifle:
                UseWeapon = Instantiate(weapons[a].model.gameObject, riflePos.transform, handGunPos);
                UseWeapon.GetComponent<Transform>().position = new Vector3(0, 0, 0);
                if (UseWeapon.GetComponent<Rigidbody>())
                {
                    Destroy(UseWeapon.GetComponent<Rigidbody>());
                }
                UseWeapon.transform.rotation = riflePos.transform.rotation;
                break;
            default:
                break;
        }


    }

  


    [Command]
    public void CmdDestroy(GameObject ga)
    {
        RpcDestroy(ga);
    }
    [ClientRpc]
    public void RpcDestroy(GameObject ga)
    {
        Destroy(ga);
    }
    
}
