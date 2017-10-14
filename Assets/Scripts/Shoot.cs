using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Shoot : NetworkBehaviour {

    public bool canFire = true;
    public GameObject rocketPrefab;
    public Transform fireTrans;
    public int maxAmmo = 10;
    //private int ammo;
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

    [SyncVar(hook="OnChangeAmmo")]
    public int Ammo;

    private RaycastHit hit;
    public float range = 5000;
    public LayerMask headMask;

    public float damage = 10;

    public GameObject explosion;

    public AudioClip[] clips;

    public string theNetworkID;

    // Use this for initialization
    void Start() {

        playerScript = GetComponent<Player>();

        if (!isLocalPlayer)
        {
            return;
        }

        Ammo = maxAmmo;
        nextFire = 0;
        kickbackAcc = 0;

        OnChangeAmmo(maxAmmo);

        theNetworkID = Network.player.ToString();
    }

    // Update is called once per frame
    void Update() {

        if (!isLocalPlayer)
        {
            return;
        }

        if (Input.GetButton("Fire1") && canFire && Ammo > 0 && Time.time > nextFire)
        {

            LeanTween.cancel(gameObject);

            //CmdFire(firePosition.position,firePosition.rotation);
            FireRocket();

            //audio.Play();
            CmdSound(0);

            fireOneshot = true;

            kickbackAcc -= kickback;
            //kickbackAcc = Mathf.Clamp(kickbackAcc, -3, 0);

            Ammo--;

            PlayerCanvas.canvas.SetAmmo(Ammo);

            nextFire = Time.time + fireDelay;
        }


        if (Input.GetButton("Fire2") && canFire && Ammo > 9 && Time.time > nextFire)
        {

            CmdHitscan(fireTrans.position, fireTrans.forward);

            LeanTween.cancel(gameObject);

            fireOneshot = true;

            //audio.Play();
            CmdSound(1);


            kickbackAcc -= kickback * 2;

            Ammo -= 10;

            PlayerCanvas.canvas.SetAmmo(Ammo);

            nextFire = Time.time + fireDelay * 2;
        }




        if (fireOneshot)
        {

            LeanTween.value(gameObject, kickbackAcc, 0, kickTween).setEase(LeanTweenType.easeOutQuint).setOnUpdate((float val) => { kickbackAcc = val; });

            fireOneshot = false;
        }

        swayScript.zOffset = kickbackAcc;

    }

    void FireRocket()
    {
        GameObject rocket = GameObject.Instantiate(rocketPrefab,
                               fireTrans.position,
                               fireTrans.rotation) as GameObject;

        //set me as the owner of the rocket
        Rocket rocketScript = rocket.GetComponent<Rocket>();
        rocketScript.owner = playerScript;

        //make rocket not collide with me
        Collider rocketCol = rocket.GetComponent<Collider>();

        Physics.IgnoreCollision(rocketCol, playerCollider);
        Physics.IgnoreCollision(rocketCol, head);
        Physics.IgnoreCollision(rocketCol, eye);

        //tell the server to tell the rest of the players to make a rocket on their machines
        //you'll need to pass the pos and rot to spawn the rocket in
        CmdSpawnDummyRocket(fireTrans.position, fireTrans.rotation, theNetworkID);
    }

    [Command]
    void CmdSpawnDummyRocket(Vector3 pos, Quaternion rot, string shooterInd)
    {
        //send message to players to make a rocket and shit
        RpcSpawnDummyRocket(pos, rot, shooterInd);
    }

    [ClientRpc]
    void RpcSpawnDummyRocket(Vector3 pos, Quaternion rot, string shooterInd)
    {
        //this dummy rocket will have no hit detection
        //it is to show the other players where the owner's rocket is
        //as such, it shouldn't be called on the owner

        

        if (theNetworkID == shooterInd) {
            return;
        }
        

        GameObject rocket = GameObject.Instantiate(rocketPrefab,
                               pos,
                               rot) as GameObject;

        //set the rocket to dummy mode
        Rocket rocketScript = rocket.GetComponent<Rocket>();
        rocketScript.dummy = true;
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

    void OnChangeAmmo(int newAmmo)
    {
        if (isLocalPlayer)
        {

            PlayerCanvas.canvas.SetAmmo(newAmmo);
            Ammo = newAmmo;

        }
    }

}
