using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {

    public static GameManager gameManager;
    
    public Dictionary<Player, string> playerDictionary;

    //Ensure there is only one gameManager
    void Awake()
    {
        if (gameManager == null)
            gameManager = this;
        else if (gameManager != this)
            Destroy(gameObject);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
