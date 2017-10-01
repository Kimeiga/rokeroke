﻿using UnityEngine; using System.Collections;

using UnityEngine.UI;

/// MouseLook rotates the transform based on the mouse delta. /// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character: /// - Create a capsule. /// - Add a rigid body to the capsule /// - Add the MouseLook script to the capsule. /// -> Set the mouse look to use LookX. (You want to only turn character but not tilt it) /// - Add FPSWalker script to the capsule

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform. /// - Add a MouseLook script to the camera. /// -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)

public class MouseRotate : MonoBehaviour {

    public bool canRotate;
    public bool onlyOffset;

public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
public RotationAxes axes = RotationAxes.MouseXAndY;
public float sensitivityX = 2F;
public float sensitivityY = 2F;

public float minimumX = -360F;
public float maximumX = 360F;

public float minimumY = -90F;
public float maximumY = 90F;

public float rotationX = 0F;
public float rotationY = 0F;

public Quaternion originalRotation;

    public float xOffset;
    public float yOffset;
    

    public float glideTurn;

    public float turnOffset;

void Update ()
{
        
        

        if (axes == RotationAxes.MouseXAndY)
        {
            // Read the mouse input axis
            if(canRotate){

				rotationX += Input.GetAxis("Mouse X") * sensitivityX;
				rotationY += Input.GetAxis("Mouse Y") * sensitivityY;

				rotationX = ClampAngle(rotationX, minimumX, maximumX);
				rotationY = ClampAngle(rotationY, minimumY, maximumY);   
            }
            

            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX + xOffset + turnOffset, Vector3.up);
            Quaternion yQuaternion = Quaternion.AngleAxis(rotationY + yOffset, -Vector3.right);

            transform.localRotation = originalRotation * xQuaternion * yQuaternion;
        }
        else if (axes == RotationAxes.MouseX)
        {
            if(canRotate){
				rotationX += Input.GetAxis("Mouse X") * sensitivityX;
				rotationX = ClampAngle(rotationX, minimumX, maximumX);    
            }


            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX + xOffset + turnOffset, Vector3.up);
            transform.localRotation = originalRotation * xQuaternion;

        }
        else
        {
            if(canRotate){

				rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
				rotationY = ClampAngle(rotationY, minimumY, maximumY);
            }

            Quaternion yQuaternion = Quaternion.AngleAxis(rotationY + yOffset, -Vector3.right);
            transform.localRotation = originalRotation * yQuaternion;
        }


        if (onlyOffset){

			Quaternion xQuaternion = Quaternion.AngleAxis(xOffset, Vector3.up);
			Quaternion yQuaternion = Quaternion.AngleAxis(yOffset, -Vector3.right);

			transform.localRotation = originalRotation * xQuaternion * yQuaternion;
        }
        

}

    public void ResetOffsets()
    {
        LeanTween.value(gameObject, yOffset, 0, 0.7f).setEase(LeanTweenType.easeOutQuart).setOnUpdate((float val) => { yOffset = val; });
        LeanTween.value(gameObject, xOffset, 0, 0.7f).setEase(LeanTweenType.easeOutQuart).setOnUpdate((float val) => { xOffset = val; });
    }

void Start ()
{
	// Make the rigid body not change rotation
	//if (GetComponent<Rigidbody>())
	//	GetComponent<Rigidbody>().freezeRotation = true;
	originalRotation = transform.localRotation;

}
    void ResetAll(){

        originalRotation = transform.localRotation;
        rotationX = 0F;
        rotationY = 0F;
    }   

public static float ClampAngle (float angle, float min, float max)
{
	if (angle < -360F){
			angle += 360F;
		}
	if (angle > 360F){
		angle -= 360F;
		}
	return Mathf.Clamp (angle, min, max);
}

    public void Reset()
    {
        rotationX = 0;
        rotationY = 0;
    }
}