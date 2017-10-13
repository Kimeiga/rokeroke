using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

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
                    
                    int victimID = victimPlayer.myID;

                    owner.CmdDoleDamage(victimID, damage, owner.myID);

                    CmdTakeDamage(victimID);
                }
            }
            //
            CmdExplode(hitPlayer);

        }

	}
    
    [Command]
    void CmdTakeDamage(int victimID)
    {
        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject obj in Players)
         {
            if (obj.GetComponent<Player>().myID == victimID)
            {
             
                obj.GetComponent<Player>().TakeDamage(damage, owner);
            }
         }



        //var victimPlayer = GameManager.gameManager.playerDictionary.FirstOrDefault(x => x.Value == victimID).Key;

        //victimPlayer.TakeDamage(damage, owner);
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

