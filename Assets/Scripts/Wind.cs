using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Wind : NetworkBehaviour {

    public AudioSource audio;
    private Rigidbody rigid;
    public AnimationCurve curve;

    private Player player;
    

	// Use this for initialization
	void Start ()
    {

        rigid = GetComponent<Rigidbody>();
        player = GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () {

        
            float volume = Mathf.Clamp(rigid.velocity.magnitude / 100, 0, 1f);
            volume *= curve.Evaluate(volume);
            audio.volume = volume;


        if (player.isLocalPlayer)
        {
            PlayerCanvas.canvas.SetSpeed(rigid.velocity.magnitude);
        }
    }
}
