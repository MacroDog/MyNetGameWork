using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public enum weaponType
{
    HandGun,
    Rifle
}
public class ArmControllerScript : MonoBehaviour
{
    public NetCharacter user;
    public int Damage;
    public weaponType _type;
	Animator anim;
    public DropWeapon model;//掉落在地上的模型
	bool isReloading;
	bool outOfAmmo;
	
	bool isShooting;
	bool isAimShooting;
	bool isAiming;
	bool isDrawing;
	bool isRunning;
	bool isJumping;
	
	//用于射速
	float lastFired;
	
	//剩余弹药
	public int currentAmmo;
	
	[System.Serializable]
	public class shootSettings
	{  
		[Header("Ammo")]
		//总弹药
		public int ammo;
		
		[Header("Fire Rate & Bullet Settings")]
		public bool automaticFire;
		public float fireRate;
		
		[Space(10)]
		
		//How far the raycast will reach
		public float bulletDistance = 500.0f;
		//How much force will be applied to rigidbodies 
		//by the bullet raycast
		public float bulletForce = 500.0f;
		
		[Header("Shotgun Settings")]
		public bool useShotgunSpread;
		//How big the pellet spread area will be
		public float spreadSize = 2.0f;    
		//How many pellets to shoot
		public int pellets = 30;
		
		[Header("Projectile Weapon Settings")]
		
		//If the current weapon is a projectile weapon (rpg, bazooka, etc)
		public bool projectileWeapon;
		
		//The projectile spawned when shooting
		public Transform projectile;
		//The static projectile on the weapon
		//This will be hidden when shooting
		public Transform currentProjectile;
		
		//How long after shooting the reload will start
		public float reloadTime;
		
	}
	public shootSettings ShootSettings;
	
	[System.Serializable]
	public class reloadSettings
	{  
		[Header("Reload Settings")]
		public bool casingOnReload;
		public float casingDelay;
		
		[Header("Bullet In Mag")]
		public bool hasBulletInMag;
		public Transform[] bulletInMag;
		public float enableBulletTimer = 1.0f;

		[Header("Bullet Or Shell Insert")]
		//If the weapon uses a bullet/shell insert style reload
		//Used for the bolt action sniper and pump shotgun for example
		public bool usesInsert;
		
	}
	public reloadSettings ReloadSettings;
	
	[System.Serializable]
	public class impactTags
	{  
		[Header("Impact Tags")]
		//Default tags for bullet impacts
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
	
	//All Components
	[System.Serializable]
	public class components
	{  
		[Header("Muzzleflash Holders")]
		public bool useMuzzleflash = false;
		public GameObject sideMuzzle;
		public GameObject topMuzzle;
		public GameObject frontMuzzle;
		//Array of muzzleflash sprites
		public Sprite[] muzzleflashSideSprites;
		
		[Header("Light Front")]
		public bool useLightFlash = false;
		public Light lightFlash;
		
		[Header("Particle System")]
		public bool playSmoke = false;
		public ParticleSystem smokeParticles;
		public bool playSparks = false;
		public ParticleSystem sparkParticles;
		public bool playTracers = false;
		public ParticleSystem bulletTracerParticles;
	}
	public components Components;
	
	//All weapon types
	[System.Serializable]
	public class prefabs
	{  
		[Header("Prefabs")]
		public Transform casingPrefab;
		
		[Header("Metal")]
		[Header("Bullet Impacts & Tags")]
		public Transform metalImpactStaticPrefab;
		public Transform metalImpactPrefab;
		[Header("Wood")]
		public Transform woodImpactStaticPrefab;
		public Transform woodImpactPrefab;
		[Header("Concrete")]
		public Transform concreteImpactStaticPrefab;
		public Transform concreteImpactPrefab;
		[Header("Dirt")]
		public Transform dirtImpactStaticPrefab;
		public Transform dirtImpactPrefab;
        public bool isAim;
	}
	public prefabs Prefabs;
	
	[System.Serializable]
	public class spawnpoints
	{  
		[Header("Spawnpoints")]
		//Array holding casing spawn points 
		//(some weapons use more than one casing spawn)
		public Transform [] casingSpawnPoints;
		//Bullet raycast start point
		public Transform bulletSpawnPoint;
	}
	public spawnpoints Spawnpoints;
	
	[System.Serializable]
	public class audioClips
	{  
		[Header("Audio Source")]
		
		public AudioSource mainAudioSource;
		
		[Header("Audio Clips")]
		
		//All audio clips
		public AudioClip shootSound;
		public AudioClip reloadSound;
	}
	public audioClips AudioClips;

	public bool noSwitch = false;
	
	void Awake () {
		
		//Set the animator component
		anim = GetComponent<Animator>();
		
		//Set the ammo count
		RefillAmmo ();
		
		//Hide muzzleflash and light at start
		if (!ShootSettings.projectileWeapon) {
			
			Components.sideMuzzle.GetComponent<SpriteRenderer> ().enabled = false;
			Components.topMuzzle.GetComponent<SpriteRenderer> ().enabled = false;
			Components.frontMuzzle.GetComponent<SpriteRenderer> ().enabled = false;
		}
		
		//Disable the light flash
		Components.lightFlash.GetComponent<Light> ().enabled = false;
	}
	
	void Update () {
		
		//Check which animation 
		//is currently playing
		AnimationCheck ();

        ////Left click (if automatic fire is false)
        //if (Input.GetMouseButtonDown(0) && !ShootSettings.automaticFire
        //    //Disable shooting while running and jumping
        //    && !isReloading && !outOfAmmo && !isShooting && !isAimShooting && !isRunning && !isJumping)
        //{
        //    //If shotgun shoot is true


        //    if (ShootSettings.useShotgunSpread == true)
        //    {

        //        ShotgunShoot();
        //        Debug.Log(ShootSettings.useShotgunSpread);
        //    }
        //    //If projectile weapon is false
        //    if (!ShootSettings.projectileWeapon && !ShootSettings.useShotgunSpread)
        //    {
        //        Shoot();
        //        //If projectile weapon is true
        //    }
        //    else if (ShootSettings.projectileWeapon == true)
        //    {
        //        StartCoroutine(ProjectileShoot());
        //    }
        //}

        ////Left click hold (if automatic fire is true)
        //if (Input.GetMouseButton(0) && ShootSettings.automaticFire == true
        //    //Disable shooting while running and jumping
        //    && !isReloading && !outOfAmmo && !isShooting && !isAimShooting && !isRunning && !isJumping)
        //{
        //    //Shoot automatic
        //    if (Time.time - lastFired > 1 / ShootSettings.fireRate)
        //    {
        //        Shoot();
        //        lastFired = Time.time;
        //    }
        //}

        ////Right click hold to aim
        //if (Input.GetMouseButton(1))
        //{
        //    anim.SetBool("isAiming", true);
        //}
        //else
        //{
        //    anim.SetBool("isAiming", false);
        //}

        ////R key to reload
        ////Not used for projectile weapons
        //if (Input.GetKeyDown(KeyCode.R) && !isReloading && !ShootSettings.projectileWeapon)
        //{
        //    Reload();
        //}

        ////Run when holding down left shift and moving
        //if (Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Vertical") > 0)
        //{
        //    anim.SetFloat("Run", 0.2f);
        //}
        //else
        //{
        //    //Stop running
        //    anim.SetFloat("Run", 0.0f);
        //}

        ////Space key to jump
        //if (Input.GetKeyDown(KeyCode.Space) && !isReloading)
        //{
        //    //Play jump animation
        //    anim.Play("Jump");
        //}

        //If out of ammo
        if (currentAmmo == 0) {
			outOfAmmo = true;
			//if ammo is higher than 0
		} else if (currentAmmo > 0) {
			outOfAmmo = false;
		}
	}

    /// <summary>
    /// 当按下开火键
    /// </summary>
    public  void ClietFire()
    {
        if ( !ShootSettings.automaticFire
            //Disable shooting while running and jumping
            && !isReloading && !outOfAmmo && !isShooting && !isAimShooting && !isRunning && !isJumping)
        {
            //If shotgun shoot is true


            if (ShootSettings.useShotgunSpread == true)
            {

                ShotgunShoot();
                Debug.Log(ShootSettings.useShotgunSpread);
            }
            //If projectile weapon is false
            if (!ShootSettings.projectileWeapon && !ShootSettings.useShotgunSpread)
            {
                Shoot();
                //If projectile weapon is true
            }
            else if (ShootSettings.projectileWeapon == true)
            {
                StartCoroutine(ProjectileShoot());
            }
        }

        //Left click hold (if automatic fire is true)
        if (ShootSettings.automaticFire == true
            //Disable shooting while running and jumping
            && !isReloading && !outOfAmmo && !isShooting && !isAimShooting && !isRunning && !isJumping)
        {
            //Shoot automatic
            if (Time.time - lastFired > 1 / ShootSettings.fireRate)
            {
                Shoot();
                lastFired = Time.time;
            }
        }
    }
   /// <summary>
   /// 填弹药
   /// </summary>
    public void ReLoad()
    {
        if (!isReloading && !ShootSettings.projectileWeapon)
        {
            Reload();
        }
       
    }
    /// <summary>
    /// 瞄准
    /// </summary>
    public void aim()
    {
        anim.SetBool("isAiming", true); 
        
    }
    public void CancelAim()
    {
        anim.SetBool("isAiming", false);
    }
    /// <summary>
    /// 跳跃动画
    /// </summary>
    public void Jump()
    {
        anim.Play("Jump");
    }
    /// <summary>
    /// 跑动
    /// </summary>
    public void Run()
    {
        if (isRunning==false)
        {
            anim.SetFloat("Run", 0.2f);
            isRunning = true;
        }
        else
        {
            //Stop running
            anim.SetFloat("Run", 0.0f);
            isRunning = false;
        }
    }
	//Muzzleflash
	IEnumerator MuzzleFlash () {
		
		//Show muzzleflash if useMuzzleFlash is true
		if (!ShootSettings.projectileWeapon && Components.useMuzzleflash == true) {
			//Show a random muzzleflash from the array
			Components.sideMuzzle.GetComponent<SpriteRenderer> ().sprite = Components.muzzleflashSideSprites 
				[Random.Range (0, Components.muzzleflashSideSprites.Length)];
			Components.topMuzzle.GetComponent<SpriteRenderer> ().sprite = Components.muzzleflashSideSprites 
				[Random.Range (0, Components.muzzleflashSideSprites.Length)];
			
			//Show the muzzleflashes
			Components.sideMuzzle.GetComponent<SpriteRenderer> ().enabled = true;
			Components.topMuzzle.GetComponent<SpriteRenderer> ().enabled = true;
			Components.frontMuzzle.GetComponent<SpriteRenderer> ().enabled = true;
		}
		
		//Enable the light flash if true
		if (Components.useLightFlash == true) {
			Components.lightFlash.GetComponent<Light> ().enabled = true;
		}
		
		//Play smoke particles if true
		if (Components.playSmoke == true) {
			Components.smokeParticles.Play ();
		}
		//Play spark particles if true
		if (Components.playSparks == true) {
			Components.sparkParticles.Play ();
		}
		//Play bullet tracer particles if true
		if (Components.playTracers == true) {
			Components.bulletTracerParticles.Play();
		}
		
		//Show the muzzleflash for 0.02 seconds
		yield return new WaitForSeconds (0.02f);
		
		if (!ShootSettings.projectileWeapon && Components.useMuzzleflash == true) {
			//Hide the muzzleflashes
			Components.sideMuzzle.GetComponent<SpriteRenderer> ().enabled = false;
			Components.topMuzzle.GetComponent<SpriteRenderer> ().enabled = false;
			Components.frontMuzzle.GetComponent<SpriteRenderer> ().enabled = false;
		}
		
		//Disable the light flash if true
		if (Components.useLightFlash == true) {
			Components.lightFlash.GetComponent<Light> ().enabled = false;
		}
	}
	
	//Projectile shoot
	IEnumerator ProjectileShoot () {
		
		//Play shoot animation
		if (!anim.GetBool ("isAiming")) {
			anim.Play ("Fire");
		} else {
			anim.SetTrigger("Shoot");
		}

		//Remove 1 bullet
		currentAmmo -= 1;

		//Play shoot sound
		AudioClips.mainAudioSource.clip = AudioClips.shootSound;
		AudioClips.mainAudioSource.Play();
		
		StartCoroutine (MuzzleFlash ());
		
		//Spawn the projectile
		Instantiate (ShootSettings.projectile, 
		             Spawnpoints.bulletSpawnPoint.transform.position, 
		             Spawnpoints.bulletSpawnPoint.transform.rotation);
		
		//Hide the current projectile mesh
		ShootSettings.currentProjectile.GetComponent
			<SkinnedMeshRenderer> ().enabled = false;
		
		yield return new WaitForSeconds (ShootSettings.reloadTime);
		
		//Play reload animation
		anim.Play ("Reload");

		//Play shoot sound
		AudioClips.mainAudioSource.clip = AudioClips.reloadSound;
		AudioClips.mainAudioSource.Play();
		
		//Show the current projectile mesh
		ShootSettings.currentProjectile.GetComponent
			<SkinnedMeshRenderer> ().enabled = true;
		
	}
	
	//Shotgun shoot
	void ShotgunShoot() {
		
		//Play shoot animation
		if (!anim.GetBool ("isAiming")) {
			anim.Play ("Fire");
		} else {
			anim.SetTrigger("Shoot");
		}
		
		//Remove 1 bullet
		currentAmmo -= 1;
		
		//Play shoot sound
		AudioClips.mainAudioSource.clip = AudioClips.shootSound;
		AudioClips.mainAudioSource.Play();
		
		//Start casing instantiate
		if (!ReloadSettings.casingOnReload) {
			StartCoroutine (CasingDelay ());
		}
		
		//Show the muzzleflash
		StartCoroutine (MuzzleFlash ());
		
		//Send out shotgun raycast with set amount of pellets
		for (int i = 0; i < ShootSettings.pellets; ++i) {
			
			float randomRadius = Random.Range 
				(0, ShootSettings.spreadSize);        
			float randomAngle = Random.Range 
				(0, 2 * Mathf.PI);
			
			//Raycast direction
			Vector3 direction = new Vector3 (
				randomRadius * Mathf.Cos (randomAngle),
				randomRadius * Mathf.Sin (randomAngle),
				15);
			
			direction = transform.TransformDirection (direction.normalized);
			
			RaycastHit hit;
            Debug.Log("hit");
            if (Physics.Raycast (Spawnpoints.bulletSpawnPoint.transform.position, direction, 
			                     out hit, ShootSettings.bulletDistance)) {
              
                


                if (hit.collider.tag=="Player")
                {
                    Debug.Log(hit.collider.name);
                    user.Shoot(hit.collider.GetComponent<NetworkIdentity>(), Damage);

                }
                //If a rigibody is hit, add bullet force to it
                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(direction * ShootSettings.bulletForce);
                   
                }
				//********** USED IN THE DEMO SCENES **********
				//If the raycast hit the tag "Target"
				if (hit.transform.tag == " f") {
					//Spawn bullet impact on surface
					Instantiate (Prefabs.metalImpactPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
					//Toggle the isHit bool on the target object
					hit.transform.gameObject.GetComponent<TargetScript>().isHit = true;
				}
				
				//********** USED IN THE DEMO SCENES **********
				//If the raycast hit the tag "ExplosiveBarrel"
				if (hit.transform.tag == "ExplosiveBarrel") {
					//Toggle the explode bool on the explosive barrel object
					hit.transform.gameObject.GetComponent<ExplosiveBarrelScript>().explode = true;
					//Spawn metal impact on surface of the barrel
					Instantiate (Prefabs.metalImpactPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}
				
				//********** USED IN THE DEMO SCENES **********
				//If the raycast hit the tag "GasTank"
				if (hit.transform.tag == "GasTank") {
					//Toggle the explode bool on the explosive barrel object
					hit.transform.gameObject.GetComponent<GasTankScript>().isHit = true;
					//Spawn metal impact on surface of the gas tank
					Instantiate (Prefabs.metalImpactPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}
				
				//If the raycast hit the tag "Metal (Static)"
				if (hit.transform.tag == ImpactTags.metalImpactStaticTag) {
					//Spawn bullet impact on surface
					Instantiate (Prefabs.metalImpactStaticPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}
				
				//If the raycast hit the tag "Metal"
				if (hit.transform.tag == ImpactTags.metalImpactTag) {
					//Spawn bullet impact on surface
					Instantiate (Prefabs.metalImpactPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}
				
				//If the raycast hit the tag "Wood (Static)"
				if (hit.transform.tag == ImpactTags.woodImpactStaticTag) {
					//Spawn bullet impact on surface
					Instantiate (Prefabs.woodImpactStaticPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}
				
				//If the raycast hit the tag "Wood"
				if (hit.transform.tag == ImpactTags.woodImpactTag) {
					//Spawn bullet impact on surface
					Instantiate (Prefabs.woodImpactPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}
				
				//If the raycast hit the tag "Concrete (Static)"
				if (hit.transform.tag == ImpactTags.concreteImpactStaticTag) {
					//Spawn bullet impact on surface
					Instantiate (Prefabs.concreteImpactStaticPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}
				
				//If the raycast hit the tag "Concrete"
				if (hit.transform.tag == ImpactTags.concreteImpactTag) {
					//Spawn bullet impact on surface
					Instantiate (Prefabs.concreteImpactPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}
				
				//If the raycast hit the tag "Dirt (Static)"
				if (hit.transform.tag == ImpactTags.dirtImpactStaticTag) {
					//Spawn bullet impact on surface
					Instantiate (Prefabs.dirtImpactStaticPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}
				
				//If the raycast hit the tag "Dirt"
				if (hit.transform.tag == ImpactTags.dirtImpactTag) {
					//Spawn bullet impact on surface
					Instantiate (Prefabs.dirtImpactPrefab, hit.point, 
					             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				}
			}    
		}
	}
	
	//Shoot
	void Shoot() {
		
		//Play shoot animation
		if (!anim.GetBool ("isAiming")) {
			anim.Play ("Fire");
		} else {
			anim.SetTrigger("Shoot");
		}
		
		//Remove 1 bullet
		currentAmmo -= 1;
		
		//Play shoot sound
		AudioClips.mainAudioSource.clip = AudioClips.shootSound;
		AudioClips.mainAudioSource.Play();
		
		//Start casing instantiate
		if (!ReloadSettings.casingOnReload) {
			StartCoroutine (CasingDelay ());
		}
		
		//Show the muzzleflash
		StartCoroutine (MuzzleFlash ());
		
		//Raycast bullet
		RaycastHit hit;
		Ray ray = new Ray (transform.position, transform.forward);
		
		//Send out the raycast from the "bulletSpawnPoint" position
		if (Physics.Raycast (Spawnpoints.bulletSpawnPoint.transform.position, 
		                     Spawnpoints.bulletSpawnPoint.transform.forward, out hit, ShootSettings.bulletDistance)) {
           
            if (hit.collider.tag == "Player")
            {
                Debug.Log(hit.collider.name);
                user.Shoot(hit.collider.GetComponent<NetworkIdentity>(), Damage);
                Debug.Log(hit.collider.name);

            }
            //If a rigibody is hit, add bullet force to it
            if (hit.rigidbody != null)
				hit.rigidbody.AddForce (ray.direction * ShootSettings.bulletForce);
			
			//********** USED IN THE DEMO SCENES **********
			//If the raycast hit the tag "Target"
			if (hit.transform.tag == "Target") {
				//Spawn bullet impact on surface
				Instantiate (Prefabs.metalImpactPrefab, hit.point, 
				             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
				//Toggle the isHit bool on the target object
				hit.transform.gameObject.GetComponent<TargetScript>().isHit = true;
			}
			
			//********** USED IN THE DEMO SCENES **********
			//If the raycast hit the tag "ExplosiveBarrel"
			if (hit.transform.tag == "ExplosiveBarrel") {
				//Toggle the explode bool on the explosive barrel object
				hit.transform.gameObject.GetComponent<ExplosiveBarrelScript>().explode = true;
				//Spawn metal impact on surface of the barrel
				Instantiate (Prefabs.metalImpactPrefab, hit.point, 
				             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
			}
			
			//********** USED IN THE DEMO SCENES **********
			//If the raycast hit the tag "GasTank"
			if (hit.transform.tag == "GasTank") {
				//Toggle the explode bool on the explosive barrel object
				hit.transform.gameObject.GetComponent<GasTankScript>().isHit = true;
				//Spawn metal impact on surface of the gas tank
				Instantiate (Prefabs.metalImpactPrefab, hit.point, 
				             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
			}
			
			//If the raycast hit the tag "Metal (Static)"
			if (hit.transform.tag == ImpactTags.metalImpactStaticTag) {
				//Spawn bullet impact on surface
				Instantiate (Prefabs.metalImpactStaticPrefab, hit.point, 
				             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
			}
			
			//If the raycast hit the tag "Metal"
			if (hit.transform.tag == ImpactTags.metalImpactTag) {
				//Spawn bullet impact on surface
				Instantiate (Prefabs.metalImpactPrefab, hit.point, 
				             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
			}
			
			//If the raycast hit the tag "Wood (Static)"
			if (hit.transform.tag == ImpactTags.woodImpactStaticTag) {
				//Spawn bullet impact on surface
				Instantiate (Prefabs.woodImpactStaticPrefab, hit.point, 
				             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
			}
			
			//If the raycast hit the tag "Wood"
			if (hit.transform.tag == ImpactTags.woodImpactTag) {
				//Spawn bullet impact on surface
				Instantiate (Prefabs.woodImpactPrefab, hit.point, 
				             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
			}
			
			//If the raycast hit the tag "Concrete (Static)"
			if (hit.transform.tag == ImpactTags.concreteImpactStaticTag) {
				//Spawn bullet impact on surface
				Instantiate (Prefabs.concreteImpactStaticPrefab, hit.point, 
				             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
			}
			
			//If the raycast hit the tag "Concrete"
			if (hit.transform.tag == ImpactTags.concreteImpactTag) {
				//Spawn bullet impact on surface
				Instantiate (Prefabs.concreteImpactPrefab, hit.point, 
				             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
			}
			
			//If the raycast hit the tag "Dirt (Static)"
			if (hit.transform.tag == ImpactTags.dirtImpactStaticTag) {
				//Spawn bullet impact on surface
				Instantiate (Prefabs.dirtImpactStaticPrefab, hit.point, 
				             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
			}
			
			//If the raycast hit the tag "Dirt"
			if (hit.transform.tag == ImpactTags.dirtImpactTag) {
				//Spawn bullet impact on surface
				Instantiate (Prefabs.dirtImpactPrefab, hit.point, 
				             Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
			}
		}
	}
	
	//Refill ammo
	void RefillAmmo () {
		
		currentAmmo = ShootSettings.ammo;
	}
	
	//Reload
	void Reload () {
		
		//Play reload animation
		anim.Play ("Reload");
		
		//Play reload sound
		AudioClips.mainAudioSource.clip = AudioClips.reloadSound;
		AudioClips.mainAudioSource.Play();
		
		//Spawn casing on reload, only used on some weapons
		if (ReloadSettings.casingOnReload == true) {
			StartCoroutine(CasingDelay());
		}
		
		if (outOfAmmo == true && ReloadSettings.hasBulletInMag == true) {
			//Hide the bullet inside the mag if ammo is 0
			for (int i = 0; i < ReloadSettings.bulletInMag.Length; i++) {
				ReloadSettings.bulletInMag[i].GetComponent
					<MeshRenderer> ().enabled = false;
			}
			//Start the "show bullet" timer
			StartCoroutine (BulletInMagTimer ());
		}
	}
	
	IEnumerator BulletInMagTimer () {
		//Wait for set amount of time 
		yield return new WaitForSeconds 
			(ReloadSettings.enableBulletTimer);
		
		//Show the bullet inside the mag
		for (int i = 0; i < ReloadSettings.bulletInMag.Length; i++) {
			ReloadSettings.bulletInMag[i].GetComponent
				<MeshRenderer> ().enabled = true;
		}
	}
	
	IEnumerator CasingDelay () {
		//Wait set amount of time for casing to spawn
		yield return new WaitForSeconds (ReloadSettings.casingDelay);
		//Spawn a casing at every casing spawnpoint
		for (int i = 0; i < Spawnpoints.casingSpawnPoints.Length; i++) {
			Instantiate (Prefabs.casingPrefab, 
			             Spawnpoints.casingSpawnPoints [i].transform.position, 
			             Spawnpoints.casingSpawnPoints [i].transform.rotation);
           
		}
	}
	
	//Check current animation playing
	void AnimationCheck () {
		
		//Check if shooting
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Fire")) {
			isShooting = true;
		} else {
			isShooting = false;
		}

		//Check if shooting while aiming down sights
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Aim Fire")) {
			isAimShooting = true;
		} else {
			isAimShooting = false;
		}

		//Check if running
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Run")) {
			isRunning = true;
		} else {
			isRunning = false;
		}
		
		//Check if jumping
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Jump")) {
			isJumping = true;
		} else {
			isJumping = false;
		}

		//Check if finsihed reloading when using "insert" style reload
		//Used for bolt action sniper and pump shotgun for example
		if (ReloadSettings.usesInsert == true && 
		    anim.GetCurrentAnimatorStateInfo (0).IsName ("Idle")) {
			isReloading = false;
			//Used in the demo scnes
			noSwitch = false;
		}
		
		//Check if reloading
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Reload")) {
			// If reloading
			isReloading = true;
			//Refill ammo
			RefillAmmo();
			//Used in the demo scenes
			noSwitch = true;
		} else {
			//If not using "insert" style reload
			if (!ReloadSettings.usesInsert) {
				//If not reloading
				isReloading = false;
				//Used in the demo scenes
				noSwitch = false;
			}
		}
	}


    void DamageToPlayer()
    {

    }
}