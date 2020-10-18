using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;


public class cubeTest : MonoBehaviour
{
    // Start is called before the first frame update
     GameObject leftController ;
     GameObject rightController;
    GameObject reticle;
    GameObject camera;
    

    Vector3 leftPos;
    Vector3 rightPos;
    Vector3 leftMag;
    Vector3 rightMag;
    Vector3 cameraVector;

    Vector2 primaryStick;
    Vector2 secondaryStick;

    Vector3 gameObjectPos;

    Boolean cubeStatusOne = false;


    float distance;


    private void Start()
    {
        leftController = GameObject.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/LeftHandAnchor/LeftControllerAnchor");
        rightController = GameObject.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/RightControllerAnchor");
        camera = GameObject.Find("OVRPlayerController/OVRCameraRig");
        reticle = GameObject.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/RightControllerAnchor/Reticle");
    }

    // Update is called once per frame
    void Update()
    {
        if (cubeStatusOne)
        {
            scaleObject();
            rotateObject();
            moveObject();
        }

        
    }

    public void changeColor()
    {
        //objectOne.GetComponent<MeshRenderer>().material.color = Color.red;
    }

    public void scaleObject()
    {
     
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) > 0f && OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) > 0f) 
        {
            leftPos = leftController.transform.position;
            rightPos = rightController.transform.position;
            cameraVector = camera.transform.position;

            leftMag = leftPos - camera.transform.position;
            rightMag = rightPos - camera.transform.position;

            distance = (rightMag.x - leftMag.x) * 10;
            this.gameObject.transform.localScale = new Vector3(distance, distance, distance);
        }
     }

    public void rotateObject()
    {
        secondaryStick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        // this.gameObject.transform.localPosition.x = secondaryStick.x;
        
        this.gameObject.transform.Rotate(secondaryStick.y * 2, -secondaryStick.x * 2, 0);
        Debug.Log("SecondaryStick" + secondaryStick);
    }

    public void moveObject()
    {
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0f)
        {
            Debug.LogError("test");
            this.gameObject.transform.position = reticle.transform.position;
        }
    }

    public void cubeStatusOn()
    {
        cubeStatusOne = true;
    }

    public void cubeStatusOff()
    {
        cubeStatusOne = false;
    }
}
