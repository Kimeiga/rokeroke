using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {

    public int ammoBonus = 50;
    public float rotateSpeed = 10;

    public AudioSource audio;

	// Use this for initialization
	void Start () {
        audio = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {


        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
    //    {
    //        Shoot shootScript = other.gameObject.GetComponent<Shoot>();
    //        shootScript.Ammo += ammoBonus;

    //        CmdDestroy();
    //    }

    //}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            //Shoot shootScript = collision.gameObject.GetComponent<Shoot>();
            //shootScript.Ammo += ammoBonus;

            //CmdDestroy();

            Player player = collision.gameObject.GetComponent<Player>();
            player.CmdGetBullet(gameObject,ammoBonus);

            print("D");
        }
    }

    [Command]
    void CmdDestroy()
    {
        AudioSource.PlayClipAtPoint(audio.clip, transform.position);
        Destroy(gameObject);
    }
    //[Command]
    //void CmdSound(int ind)
    //{
    //    RpcSound(ind);
    //}

    //[ClientRpc]
    //void RpcSound(int ind)
    //{
    //    audio.PlayOneShot(clips[ind], 1);
    //}
}
