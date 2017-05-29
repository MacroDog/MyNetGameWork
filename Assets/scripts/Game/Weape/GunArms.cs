using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunArms : MonoBehaviour
{

    private Animator anim;//手臂动画animator
    private bool outOfAmmo;
    private bool isReloading;//
    private bool isShooting;//
    private bool isAimShooting;//瞄准射击
    private bool isAiming;//瞄准
    //bool isDrawing;
    private bool isRunning;//
    private bool isJumping;

    private float lastFired;//射击速度
    private int currentAmmo;//当前弹药

    [System.Serializable]
    public class shootSettings
    {
        public int ammo;//当前弹药总和

        public bool automaticFire;//是否能自动开火
        public float fireRate;//开火速度
        public float bulletDistance = 500;
        public float bulletForce = 500;//子弹的是速度

        //用于霰弹枪
        public bool useShotgunSpread;//是否扩散子弹
        public float spreadSize = 2.0f;//子弹扩散的size
        public int pellets = 30;

        public bool projectileWeapon;//抛射武器例如榴弹枪
        public Transform projectile;//
        public Transform currentProjectile;

        public float reloadTime;
    }
    public shootSettings ShootSettings;
    public class reloadSettings
    {
        public bool casingOnReload;//
        public float casingDelay;//装弹延迟
        public bool hasBulletInMag;//弹夹是否包含子弹
        public Transform[] bulletInMag;
        public float enableBulletTimer = 1.0f;

        public bool userInsert;//
    }
    public reloadSettings ReloadSettings;
    [System.Serializable]
    public class impactTags
    {
        public string metalImpactStaticTag = "Metal (Static)";
        public string metalImpactTag = "Metal";
        public string woodImpactStaticTag = "Wood (Static)";
        public string woodImpactTag = "Wood";
        public string concreteImpactStaticTag = "Concrete (Static)";
        public string concreteImpactTag = "Concrete";
        public string dirtImpactStaticTag = "Dirt (Static)";
        public string dirtImpactTag = "Dirt";
    }
    public impactTags ImpactTags;
    public class components
    {
        public bool userMuzzleflash = false;//枪口火焰
        public bool useMuzzleflash = false;
        public GameObject sideMuzzle;
        public GameObject topMuzzle;
        public GameObject frontMuzzle;

        public Sprite[] muzzleflashSideSprites;

        public bool userLigthFlash = false;
        public Light lightFlash;

        public bool playSmoke = false; //是否使用枪口烟雾
        public ParticleSystem smokeParticles;
        public bool playSparks = false;//枪口火花
        public ParticleSystem sparkParticles;
        public bool playTracers = false;//轨迹
        public ParticleSystem bulletTracerParticles;

    }
    public components Components;
    public class prefabs//z
    {
        public Transform casingPrefab;//弹壳

        public Transform metalImpactStaticPrefab;//金属上的贴图改变
        public Transform metalImpactPrefab;//金属上的声音

        public Transform woodImpactStaticPrefab;
        public Transform woodImpactPrefab;

        public Transform concreteImpactStaticPrefab;
        public Transform concreteImpactPrefab;

        public Transform dirtImpactStaticPrefab;
        public Transform dirtImpactPrefab;
    }
    public prefabs Prefabs;

    public class spawnpoints
    {
        public Transform[] casingSpawnPoints;//弹壳产生点
        public Transform bulletSpawnPoint;//子弹产生点

    }
    public spawnpoints Spawnpoints;

    public class audioClips
    {
        public AudioSource mainAudioSource;
        public AudioClip shootSound;
        public AudioClip reloadSound;
    }

    public audioClips AudioClips;
    public bool noSwitch = false;


    void Awake()
    {
        anim = GetComponent<Animator>();
        RefillAmmo();
        if (!ShootSettings.projectileWeapon)
        {
            Components.sideMuzzle.GetComponent<SpriteRenderer>().enabled = false;
            Components.topMuzzle.GetComponent<SpriteRenderer>().enabled = false;
            Components.frontMuzzle.GetComponent<SpriteRenderer>().enabled = false;
        }
        Components.lightFlash.GetComponent<Light>().enabled = false;
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !ShootSettings.automaticFire && !isReloading && !outOfAmmo && !isShooting && !isAimShooting && !isJumping)
        {
            if (Time.time-lastFired>1/ShootSettings.fireRate)
            {
                Shoot();
                lastFired = Time.time;
            }
        }
        if (Input.GetMouseButton(1))
        {
            anim.SetBool("isAimng", true);
        }
        else
        {
            anim.SetBool("isAiming", false);
        }
        if (Input .GetKeyDown(KeyCode.R)&&!isReloading&&!ShootSettings.projectileWeapon)
        {
            Reload();
        }
        if (Input.GetKey(KeyCode.LeftShift)&&Input.GetAxis("Vertical")>0)
        {
            anim.SetFloat("Run", 0.2f);
        }
        else
        {
            anim.SetFloat("Run", 0.0f);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.Play("Jump");
        }
        if (currentAmmo==0)
        {
            outOfAmmo = true;
        }
        else if(currentAmmo>0)
        {
            outOfAmmo = false;
        }
        
    }
    IEnumerator MuzzleFlash()
    {
        if (!ShootSettings.projectileWeapon&&Components.useMuzzleflash==true )
        {
            Components.sideMuzzle.GetComponent<SpriteRenderer>().sprite = Components.muzzleflashSideSprites[Random.Range(0, Components.muzzleflashSideSprites.Length)];
            Components.topMuzzle.GetComponent<SpriteRenderer>().sprite = Components.muzzleflashSideSprites[Random.Range(0, Components.muzzleflashSideSprites.Length)];
            Components.sideMuzzle.GetComponent<SpriteRenderer>().enabled = true;
            Components.topMuzzle.GetComponent<SpriteRenderer>().enabled = true;
            Components.frontMuzzle.GetComponent<SpriteRenderer>().enabled = true;
            if (Components.userLigthFlash==true)
            {
                Components.lightFlash.GetComponent<Light>().enabled = true;
            }
            if (Components.playSmoke==true)
            {
                Components.smokeParticles.Play();
            }
            if (Components.playSparks==true)
            {
                Components.sparkParticles.Play();
            }
            if (Components.playTracers)
            {
                Components.bulletTracerParticles.Play();
            }
            yield return new WaitForSeconds(0.02f);
            if (!ShootSettings.projectileWeapon&&Components.useMuzzleflash==true)
            {
                Components.sideMuzzle.GetComponent<SpriteRenderer>().enabled = false;
                Components.topMuzzle.GetComponent<SpriteRenderer>().enabled = false;
                Components.frontMuzzle.GetComponent<SpriteRenderer>().enabled = false;
            }
            if (Components.userLigthFlash == true)
            {
                Components.lightFlash.GetComponent<Light>().enabled = false;
            }
        }
    }
    /// <summary>
    /// 抛弹壳
    /// </summary>
    /// <returns></returns>
    IEnumerator ProjectilShoot()
    {
        if (!anim.GetBool("isAiming"))
        {
            anim.Play("Fire");
        }
        else
        {
            anim.SetTrigger("Shoot");
        }
        currentAmmo -= 1;
        AudioClips.mainAudioSource.clip = AudioClips.shootSound;
        AudioClips.mainAudioSource.Play();
        StartCoroutine(MuzzleFlash());

        Instantiate(ShootSettings.projectile,
                    Spawnpoints.bulletSpawnPoint.transform.position,
                    Spawnpoints.bulletSpawnPoint.transform.rotation);
        ShootSettings.currentProjectile.GetComponent<SkinnedMeshRenderer>().enabled = false;
        yield return new WaitForSeconds(ShootSettings.reloadTime);
        anim.Play("Reload");
        AudioClips.mainAudioSource.clip = AudioClips.reloadSound;
        AudioClips.mainAudioSource.Play();
        ShootSettings.currentProjectile.GetComponent<SkinnedMeshRenderer>().enabled = true;

    }
    //shotgun shoot
    void ShotGunShoot()
    {
        if (!anim.GetBool("isAiming"))
        {
            anim.Play("Fire");
        }
        else
        {
            anim.SetTrigger("Shoot");
        }
        currentAmmo -= 1;
        AudioClips.mainAudioSource.clip = AudioClips.shootSound;
        AudioClips.mainAudioSource.Play();
        if (!ReloadSettings.casingOnReload)
        {
            StartCoroutine(CasingDelay());
        }
        StartCoroutine(MuzzleFlash());
        for (int i = 0; i < ShootSettings .pellets; i++)
        {
            float randomRadius = Random.Range(0, ShootSettings.spreadSize);
            float randomAngle = Random.Range(0, 2 * Mathf.PI);

            Vector3 direction = new Vector3(
                randomRadius * Mathf.Cos(randomAngle),
                randomRadius * Mathf.Sin(randomAngle),
                15);
            direction = transform.TransformDirection(direction.normalized);
            RaycastHit hit;
            // Debug.Log("hi")
            if (Physics.Raycast(Spawnpoints.bulletSpawnPoint.transform.position,direction,out hit,ShootSettings.bulletDistance))
            {
                if (hit.rigidbody!=null)
                {
                    hit.rigidbody.AddForce(direction * ShootSettings.bulletForce);
                }
                if (hit.transform.tag=="Target")
                {
                    Instantiate(Prefabs.metalImpactPrefab,hit.point,Quaternion.FromToRotation(Vector3.forward,hit.normal));
                }
                if (hit.transform.tag== "ExplosiveBarrel")
                {
                    hit.transform.gameObject.GetComponent<ExplosiveBarrelScript>().explode = true;
                    Instantiate(Prefabs.metalImpactPrefab, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                }
                if (hit.transform.tag == "GasTank") {
					//Toggle the explode bool on the explosive barrel object
					hit.transform.gameObject.GetComponent<GasTankScript>().isHit = true;
					//Spawn metal impact on surface of the gas tank
					Instantiate (Prefabs.metalImpactPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}
                if (hit.transform.tag == ImpactTags.metalImpactStaticTag)
                {
                    //Spawn bullet impact on surface
                    Instantiate(Prefabs.metalImpactStaticPrefab, hit.point,
                                 Quaternion.FromToRotation(Vector3.forward, hit.normal));
                }

                //If the raycast hit the tag "Metal"
                if (hit.transform.tag == ImpactTags.metalImpactTag)
                {
                    //Spawn bullet impact on surface
                    Instantiate(Prefabs.metalImpactPrefab, hit.point,
                                 Quaternion.FromToRotation(Vector3.forward, hit.normal));
                }

                //If the raycast hit the tag "Wood (Static)"
                if (hit.transform.tag == ImpactTags.woodImpactStaticTag)
                {
                    //Spawn bullet impact on surface
                    Instantiate(Prefabs.woodImpactStaticPrefab, hit.point,
                                 Quaternion.FromToRotation(Vector3.forward, hit.normal));
                }

                //If the raycast hit the tag "Wood"
                if (hit.transform.tag == ImpactTags.woodImpactTag)
                {
                    //Spawn bullet impact on surface
                    Instantiate(Prefabs.woodImpactPrefab, hit.point,
                                 Quaternion.FromToRotation(Vector3.forward, hit.normal));
                }

                //If the raycast hit the tag "Concrete (Static)"
                if (hit.transform.tag == ImpactTags.concreteImpactStaticTag)
                {
                    //Spawn bullet impact on surface
                    Instantiate(Prefabs.concreteImpactStaticPrefab, hit.point,
                                 Quaternion.FromToRotation(Vector3.forward, hit.normal));
                }

                //If the raycast hit the tag "Concrete"
                if (hit.transform.tag == ImpactTags.concreteImpactTag)
                {
                    //Spawn bullet impact on surface
                    Instantiate(Prefabs.concreteImpactPrefab, hit.point,
                                 Quaternion.FromToRotation(Vector3.forward, hit.normal));
                }

                //If the raycast hit the tag "Dirt (Static)"
                if (hit.transform.tag == ImpactTags.dirtImpactStaticTag)
                {
                    //Spawn bullet impact on surface
                    Instantiate(Prefabs.dirtImpactStaticPrefab, hit.point,
                                 Quaternion.FromToRotation(Vector3.forward, hit.normal));
                }

                //If the raycast hit the tag "Dirt"
                if (hit.transform.tag == ImpactTags.dirtImpactTag)
                {
                    //Spawn bullet impact on surface
                    Instantiate(Prefabs.dirtImpactPrefab, hit.point,
                                 Quaternion.FromToRotation(Vector3.forward, hit.normal));
                }
            }
        }
    }
    void Shoot()
    {
        if (!anim.GetBool("isAiming"))
        {
            anim.Play("Fire");
        }
        else
        {
            anim.SetTrigger("Shoot");
        }
        currentAmmo -= 1;

        //Play shoot sound
        AudioClips.mainAudioSource.clip = AudioClips.shootSound;
        AudioClips.mainAudioSource.Play();

        //Start casing instantiate
        if (!ReloadSettings.casingOnReload)
        {
            StartCoroutine(CasingDelay());
        }

        //Show the muzzleflash
        StartCoroutine(MuzzleFlash());

        //Raycast bullet
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);

        //Send out the raycast from the "bulletSpawnPoint" position
        if (Physics.Raycast(Spawnpoints.bulletSpawnPoint.transform.position,
                             Spawnpoints.bulletSpawnPoint.transform.forward, out hit, ShootSettings.bulletDistance))
        {

            //If a rigibody is hit, add bullet force to it
            if (hit.rigidbody != null)
                hit.rigidbody.AddForce(ray.direction * ShootSettings.bulletForce);

            //********** USED IN THE DEMO SCENES **********
            //If the raycast hit the tag "Target"
            if (hit.transform.tag == "Target")
            {
                //Spawn bullet impact on surface
                Instantiate(Prefabs.metalImpactPrefab, hit.point,
                             Quaternion.FromToRotation(Vector3.forward, hit.normal));
                //Toggle the isHit bool on the target object
                hit.transform.gameObject.GetComponent<TargetScript>().isHit = true;
            }

            //********** USED IN THE DEMO SCENES **********
            //If the raycast hit the tag "ExplosiveBarrel"
            if (hit.transform.tag == "ExplosiveBarrel")
            {
                //Toggle the explode bool on the explosive barrel object
                hit.transform.gameObject.GetComponent<ExplosiveBarrelScript>().explode = true;
                //Spawn metal impact on surface of the barrel
                Instantiate(Prefabs.metalImpactPrefab, hit.point,
                             Quaternion.FromToRotation(Vector3.forward, hit.normal));
            }

            //********** USED IN THE DEMO SCENES **********
            //If the raycast hit the tag "GasTank"
            if (hit.transform.tag == "GasTank")
            {
                //Toggle the explode bool on the explosive barrel object
                hit.transform.gameObject.GetComponent<GasTankScript>().isHit = true;
                //Spawn metal impact on surface of the gas tank
                Instantiate(Prefabs.metalImpactPrefab, hit.point,
                             Quaternion.FromToRotation(Vector3.forward, hit.normal));
            }

            //If the raycast hit the tag "Metal (Static)"
            if (hit.transform.tag == ImpactTags.metalImpactStaticTag)
            {
                //Spawn bullet impact on surface
                Instantiate(Prefabs.metalImpactStaticPrefab, hit.point,
                             Quaternion.FromToRotation(Vector3.forward, hit.normal));
            }

            //If the raycast hit the tag "Metal"
            if (hit.transform.tag == ImpactTags.metalImpactTag)
            {
                //Spawn bullet impact on surface
                Instantiate(Prefabs.metalImpactPrefab, hit.point,
                             Quaternion.FromToRotation(Vector3.forward, hit.normal));
            }

            //If the raycast hit the tag "Wood (Static)"
            if (hit.transform.tag == ImpactTags.woodImpactStaticTag)
            {
                //Spawn bullet impact on surface
                Instantiate(Prefabs.woodImpactStaticPrefab, hit.point,
                             Quaternion.FromToRotation(Vector3.forward, hit.normal));
            }

            //If the raycast hit the tag "Wood"
            if (hit.transform.tag == ImpactTags.woodImpactTag)
            {
                //Spawn bullet impact on surface
                Instantiate(Prefabs.woodImpactPrefab, hit.point,
                             Quaternion.FromToRotation(Vector3.forward, hit.normal));
            }

            //If the raycast hit the tag "Concrete (Static)"
            if (hit.transform.tag == ImpactTags.concreteImpactStaticTag)
            {
                //Spawn bullet impact on surface
                Instantiate(Prefabs.concreteImpactStaticPrefab, hit.point,
                             Quaternion.FromToRotation(Vector3.forward, hit.normal));
            }

            //If the raycast hit the tag "Concrete"
            if (hit.transform.tag == ImpactTags.concreteImpactTag)
            {
                //Spawn bullet impact on surface
                Instantiate(Prefabs.concreteImpactPrefab, hit.point,
                             Quaternion.FromToRotation(Vector3.forward, hit.normal));
            }

            //If the raycast hit the tag "Dirt (Static)"
            if (hit.transform.tag == ImpactTags.dirtImpactStaticTag)
            {
                //Spawn bullet impact on surface
                Instantiate(Prefabs.dirtImpactStaticPrefab, hit.point,
                             Quaternion.FromToRotation(Vector3.forward, hit.normal));
            }

            //If the raycast hit the tag "Dirt"
            if (hit.transform.tag == ImpactTags.dirtImpactTag)
            {
                //Spawn bullet impact on surface
                Instantiate(Prefabs.dirtImpactPrefab, hit.point,
                             Quaternion.FromToRotation(Vector3.forward, hit.normal));
            }
        }
    }

   
  
   
   
       
    void RefillAmmo()
    {
        currentAmmo = ShootSettings.ammo;
    }
    void Reload()
    {
        anim.Play("Reload");
        AudioClips.mainAudioSource.clip = AudioClips.reloadSound;
        AudioClips.mainAudioSource.Play();
        if (ReloadSettings.casingOnReload==true)
        {
            StartCoroutine(CasingDelay());
        }
        if (outOfAmmo==true&&ReloadSettings .hasBulletInMag==true)
        {
            for (int i = 0; i < ReloadSettings.bulletInMag.Length; i++)
            {
                ReloadSettings.bulletInMag[i].GetComponent<MeshRenderer>().enabled = false;
            }
            StartCoroutine(BulletInMagTimer());
        }
    }
    IEnumerator BulletInMagTimer()
    {
        yield return new WaitForSeconds(ReloadSettings.enableBulletTimer);
        for (int i = 0; i < ReloadSettings.bulletInMag.Length; i++)
        {
            ReloadSettings.bulletInMag[i].GetComponent
                <MeshRenderer>().enabled = true;
        }
    }
    IEnumerator CasingDelay()
    {
        yield return new WaitForSeconds(ReloadSettings.casingDelay);
        for (int i = 0; i < Spawnpoints.casingSpawnPoints.Length; i++)
        {
            Instantiate(Prefabs.casingPrefab,
                        Spawnpoints.casingSpawnPoints[i].transform.position,
                        Spawnpoints.casingSpawnPoints[i].transform.rotation);
        }
    }
    void AnimationCheck()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Fire"))
        {
            isShooting = true;
        }
        else
        {
            isShooting = false;

        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Aim Fire"))
        {
            isAimShooting = true;
        }
        else
        {
            isAimShooting = false;
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Aim Fire"))
        {
            isAimShooting = true;
        }
        else
        {
            isAimShooting = false;
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Run"))
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
            isJumping = true;
        }
        else
        {
            isJumping = false;
        }
        if (ReloadSettings.userInsert==true&&anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            isReloading = false;
            noSwitch = false;
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Reload"))
        {
            isReloading = true;
            RefillAmmo();
            noSwitch = true;
        }
        else
        {
            if (ReloadSettings.userInsert)
            {
                isReloading = false;noSwitch = false;
            }
        }
    }
}
