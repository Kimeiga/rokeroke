using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Explosion : NetworkBehaviour
{

    public Light light;
    private float startIntensity;

    public float explosiveForce = 30f;
    public float explosiveRadius = 10f;

    private Collider[] hitColliders;

    public float lifetime = 1;

    public AudioSource audioSource;
    public AudioClip hitSound;

    public bool hitPlayer = false;
    public bool pushObjects = true;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(WaitAndDestroy());

        if (hitPlayer)
        {
            CmdHitSound();
        }

        if (pushObjects)
        {
            hitColliders = Physics.OverlapSphere(transform.position, explosiveRadius);
            foreach (Collider hit in hitColliders)
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();

                if (rb != null && rb.gameObject.tag != "Rocket")
                {
                    //    rb.AddExplosionForce(explosiveForce, transform.position, explosiveRadius, upwardsForce, ForceMode.Impulse);
                    if (rb.GetComponent<Movement>())
                    {
                        Movement movementScript = rb.GetComponent<Movement>();
                        movementScript.exploding = true;
                        movementScript.StartCoroutine("StopExploding");
                    }

                    float distance = Vector3.Distance(hit.transform.position, transform.position);

                    float distanceMod = (explosiveRadius - distance) / explosiveRadius;


                    distanceMod = Mathf.Clamp(distanceMod, 0, 1);

                    rb.AddForce((hit.transform.position - transform.position).normalized * explosiveForce * distanceMod, ForceMode.Impulse);
                }
            }
        }
        

        startIntensity = light.intensity;

        LeanTween.value(gameObject, startIntensity, 0, 1f).setEase(LeanTweenType.easeOutQuart).setOnUpdate((float val) => { light.intensity = val; });

    }

    // Update is called once per frame
    void Update()
    {



    }

    IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);

    }

    [Command]
    void CmdHitSound()
    {
        RpcHitSound();
    }

    [ClientRpc]
    void RpcHitSound()
    {
        audioSource.PlayOneShot(hitSound);
    }
}
