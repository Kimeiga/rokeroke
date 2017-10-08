using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Rocket : NetworkBehaviour {

    private Rigidbody rigid;
    public float speed = 10;

    public GameObject explosion;
    private RaycastHit hit;
    public LayerMask rocketMask;
    public float range = 2;

    public float lifetime = 10;

    public float damage = 10;

    public Player owner;

    public bool dummy = false;

    // Use this for initialization
    void Start () {
        rigid = GetComponent<Rigidbody>();
        rigid.velocity = transform.forward * speed;

        StartCoroutine(WaitAndDestroy());
    }
	
	// Update is called once per frame
	void Update () {

        if(Physics.Raycast(transform.position,transform.forward, out hit, range, rocketMask))
        {

            bool hitPlayer = false;

            if (!dummy)
            {

                var hitObj = hit.transform.gameObject;
                var victimPlayer = hitObj.GetComponent<Player>();

                if (victimPlayer != null)
                {
                    if (victimPlayer == owner)
                    {
                        return;
                    }



                    hitPlayer = true;
                    victimPlayer.TakeDamage(damage, owner);
                }
            }
            
            CmdExplode(hitPlayer);

        }

	}
    
    

    IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);

    }
    
    void CmdExplode(bool didHitPlayer)
    {
        GameObject explode = Instantiate(explosion, hit.point, Quaternion.Euler(hit.normal)) as GameObject;

        Explosion explosionScript = explode.GetComponent<Explosion>();
        explosionScript.hitPlayer = didHitPlayer;

        //NetworkServer.Spawn(explode);

        Destroy(gameObject);
    }
}

