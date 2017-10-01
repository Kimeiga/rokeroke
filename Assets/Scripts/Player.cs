using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

    public MouseRotate bodyRotate;
    public MouseRotate fireRotate;
    public MouseRotate lookRotate;

    public Movement movementScript;
    public Shoot shoot;
    public GameObject camera;

    public Sway sway;

    public const float maxHealth = 100;

    [SyncVar(hook="OnChangeHealth")]
    public float health;

    private NetworkStartPosition[] spawnPoints;

    public GameObject explosion;

    public string playerName = "MIO";

    [SyncVar(hook ="OnChangeKills")]
    public int kills = 1;
    private const int startingKills = 0;

    void Start () {
        
        if (isLocalPlayer)
        {
            PlayerCanvas.canvas.EnableCanvas();
            Camera.main.gameObject.SetActive(false);
            

            spawnPoints = FindObjectsOfType<NetworkStartPosition>();
            
            bodyRotate.enabled = true;
            fireRotate.enabled = true;
            lookRotate.enabled = true;
            movementScript.enabled = true;
            shoot.enabled = true;
            camera.SetActive(true);
            sway.enabled = true;

        }

        kills = startingKills;
        health = maxHealth;

        OnChangeKills(startingKills);

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TakeDamage(float amount, Player attacker)
    {
        

        if (!isServer)
        {
            return;
        }
        

        health -= amount;
        if (health <= 0)
        {
            CmdExplode();
            // called on the Server, invoked on the Clients
            RpcRespawn();

            attacker.kills++;

            health = maxHealth;
            shoot.Ammo = shoot.maxAmmo;

        }

    }

    [Command]
    void CmdExplode()
    {

        GameObject explode = Instantiate(explosion, transform.position, transform.rotation) as GameObject;

        NetworkServer.Spawn(explode);
    }
    


    [ClientRpc]
    void RpcRespawn()
    {
        if (isLocalPlayer)
        {

            // Set the spawn point to origin as a default value
            Vector3 spawnPoint = Vector3.zero;

            // If there is a spawn point array and the array is not empty, pick one at random
            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
            }

            // Set the player’s position to the chosen spawn point
            transform.position = spawnPoint;
        }
    }

    void OnChangeHealth(float newHealth)
    {
        if (isLocalPlayer)
        {
            health = newHealth;
            PlayerCanvas.canvas.SetHealth(newHealth);
        }
    }

    void OnChangeKills(int newKills)
    {
        if (isLocalPlayer)
        {
            kills = newKills;
            PlayerCanvas.canvas.SetKills(kills);
        }
    }
}
