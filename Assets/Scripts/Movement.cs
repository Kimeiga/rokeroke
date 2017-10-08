using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class Movement : NetworkBehaviour
{

    public float speed = 10.0f;
    public float gravity = 10.0f;
    public float maxVelocityChange = 10.0f;
    public bool canJump = true;
    public float jumpHeight = 2.0f;
    public bool grounded = false;
    private Rigidbody rigidbody;

    public AudioSource audio;

    private Vector3 targetVelocityOld;

    public bool exploding;

    public float footstepInterval = 1;
    private float nextStep = 0;

    private float height;
    private CapsuleCollider col;

    public AudioClip[] clips;

    private bool jumpInvoke = false;

    public Vector3 velocityChange;


    private bool slideOneshot = false;
    public float turnSpeed = 1.7f;


    private bool groundedPrev;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.freezeRotation = true;

        col = GetComponent<CapsuleCollider>();
        height = col.height;
        
    }
    

    private void Update()
    {
        if (Input.GetButton("Jump"))
        {
            jumpInvoke = true;
        }


        if (Input.GetButtonUp("Slide") || !grounded && slideOneshot == true)
        {
            slideOneshot = false;
            
        }

        Debug.DrawLine(transform.position, transform.position - (transform.up * height * 0.6f));
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        
        if (Physics.Raycast(transform.position, -transform.up, (height * 0.6f)))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }

        if (Input.GetButton("Slide") && grounded && slideOneshot == false)
        {
            slideOneshot = true;
        }

        // Calculate how fast we should be moving
        Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        targetVelocity = transform.TransformDirection(targetVelocity);
        targetVelocity *= speed;

        // Apply a force that attempts to reach our target velocity
        Vector3 velocity = rigidbody.velocity;
        Vector3 velocityChange = (targetVelocity - velocity);
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;

        if(!exploding && !Input.GetButton("Slide"))
        {
            rigidbody.AddForce(velocityChange * 30, ForceMode.Force);

            if (grounded && targetVelocity.magnitude > 0.01f && Time.time > nextStep)
            {
                nextStep = Time.time + footstepInterval;
                CmdSound(1);
            }
        }
        else if (!Input.GetButton("Slide"))
        {
            
                //aircontrol
                rigidbody.AddForce(targetVelocity * 2.5f, ForceMode.Force);

        }
        else
        {
            

            Vector3 velocityOffset = rigidbody.velocity;

            velocityOffset = Quaternion.AngleAxis(Input.GetAxis("Horizontal") * turnSpeed, Vector3.up) * velocityOffset;

            rigidbody.velocity = velocityOffset;

        }
        

        
        
        // Jump
        if (canJump && grounded && (Input.GetButton("Jump") || jumpInvoke))
        {
            CmdSound(0);


            rigidbody.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
        }


        // We apply gravity manually for more tuning control
        rigidbody.AddForce(new Vector3(0, -gravity * rigidbody.mass, 0));


        jumpInvoke = false;

        if (!grounded && groundedPrev)
        {
            StartCoroutine(HitGroundSound());

        }
        

        groundedPrev = grounded;
    }
    

    float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }

    public IEnumerator StopExploding()
    {
        yield return new WaitForSeconds(0.3f);
        
        yield return new WaitUntil(() => grounded == true);
       

        exploding = false;
    }

    public IEnumerator HitGroundSound()
    {
        yield return new WaitForSeconds(0.2f);

        yield return new WaitUntil(() => (grounded));

        CmdSound(2);
    }

    [Command]
    void CmdSound(int ind)
    {
        RpcSound(ind);
    }

    [ClientRpc]
    void RpcSound(int ind)
    {
        audio.PlayOneShot(clips[ind],1);
    }
    
}