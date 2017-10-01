using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour {

	// Use this for initialization
	void Start () {

        StartCoroutine(WaitAndDestroy(1));

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator WaitAndDestroy(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);

    }
}
