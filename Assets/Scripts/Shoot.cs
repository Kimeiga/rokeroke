using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Shoot : NetworkBehaviour {

    public bool canFire = true;
    public GameObject rocketPrefab;
    public Transform firePosition;
    public int maxAmmo = 10;
    private int ammo;
    public float fireDelay = 0.5f;
    private float nextFire;

    public CapsuleCollider playerCollider;


    public AudioSource audio;

    public Sway swayScript;

    public float kickback = 0.1f;
    private float kickbackAcc;
    public float kickTween = 0.3f;

    private bool fireOneshot = false;

    private Player playerScript;
    public Collider head;
    public Collider eye;

    public int Ammo
    {
        get
        {
            return ammo;
        }

        set
        {
            ammo = value;

            PlayerCanvas.canvas.SetAmmo(Ammo);
        }
    }

    private RaycastHit hit;
    public float range = 5000;
    public LayerMask headMask;

    public float damage = 10;

    public GameObject explosion;

    public AudioClip[] clips;

    // Use this for initialization
    void Start () {

        playerScript = GetComponent<Player>();

        if (!isLocalPlayer)
        {
            return;
        }

        Ammo = maxAmmo;
        nextFire = 0;
        kickbackAcc = 0;


	}
	
	// Update is called once per frame
	void Update () {

        if (!isLocalPlayer)
        {
            return;
        }

        if(Input.GetButton("Fire1") && canFire && Ammo > 0 && Time.time > nextFire)
        {

            LeanTween.cancel(gameObject);

            CmdFire(firePosition.position,firePosition.rotation);


            //audio.Play();
            CmdSound(0);

            fireOneshot = true;

            kickbackAcc -= kickback;
            //kickbackAcc = Mathf.Clamp(kickbackAcc, -3, 0);

            Ammo--;
            
            nextFire = Time.time + fireDelay;
        }


        if (Input.GetButton("Fire2") && canFire && Ammo > 9 && Time.time > nextFire)
        {

            CmdHitscan(firePosition.position, firePosition.forward);

            LeanTween.cancel(gameObject);

            fireOneshot = true;

            //audio.Play();
            CmdSound(1);


            kickbackAcc -= kickback *2;

            Ammo -= 10;

            nextFire = Time.time + fireDelay * 2;
        }




            if (fireOneshot)
        {
            
            LeanTween.value(gameObject, kickbackAcc, 0, kickTween).setEase(LeanTweenType.easeOutQuint).setOnUpdate((float val) => { kickbackAcc = val; });

            fireOneshot = false;
        }
        //LeanTween.value(gameObject, kickbackAcc, 0, 1).setOnUpdate((float val) => { kickbackAcc = val; });

        swayScript.zOffset = kickbackAcc;

    }

    //[Command]
    void CmdFire(Vector3 pos, Quaternion rot)
    {
        GameObject rocket = GameObject.Instantiate(rocketPrefab,
                               pos,
                               rot) as GameObject;
        Rocket rocketScript = rocket.GetComponent<Rocket>();
        rocketScript.owner = playerScript;

        //Simple one line fix! Don't forget to just put in Physics. and trying to find an appropriate function
        Collider rocketCol = rocket.GetComponent<Collider>();

        Physics.IgnoreCollision(rocketCol, playerCollider);
        Physics.IgnoreCollision(rocketCol, head);
        Physics.IgnoreCollision(rocketCol, eye);

        CmdMakeRocket(pos,rot);
    }

    [Command]
    void CmdMakeRocket(Vector3 pos, Quaternion rot)
    {
        GameObject rocket = GameObject.Instantiate(rocketPrefab,
                               pos,
                               rot) as GameObject;
        Rocket rocketScript = rocket.GetComponent<Rocket>();
        rocketScript.owner = playerScript;
        //
        //Simple one line fix! Don't forget to just put in Physics. and trying to find an appropriate function
        Collider rocketCol = rocket.GetComponent<Collider>();

        Physics.IgnoreCollision(rocketCol, playerCollider);
        Physics.IgnoreCollision(rocketCol, head);
        Physics.IgnoreCollision(rocketCol, eye);

        NetworkServer.Spawn(rocket);

    }

    [Command]
    void CmdHitscan(Vector3 pos, Vector3 dir)
    {
        if(Physics.Raycast(pos,dir, out hit, range, headMask))
        {
            var hitObj = hit.transform.gameObject;
            var victimPlayer = hitObj.GetComponent<Player>();
            
            bool hitPlayer = false;
            
            if (victimPlayer != null)
            {
                
                hitPlayer = true;
                victimPlayer.TakeDamage(damage, playerScript);
            }

            CmdExplode(hitPlayer);
        }
    }

    [Command]
    void CmdExplode(bool didHitPlayer)
    {
        GameObject explode = Instantiate(explosion, hit.point, Quaternion.Euler(hit.normal)) as GameObject;

        Explosion explosionScript = explode.GetComponent<Explosion>();
        explosionScript.hitPlayer = didHitPlayer;
        explosionScript.pushObjects = false;

        NetworkServer.Spawn(explode);
        
    }

    [Command]
    void CmdSound(int ind)
    {
        RpcSound(ind);
    }

    [ClientRpc]
    void RpcSound(int ind)
    {
        audio.PlayOneShot(clips[ind], 1);
    }

}
