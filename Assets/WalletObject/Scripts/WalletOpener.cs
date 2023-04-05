using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class WalletOpener : MonoBehaviour
{
    private bool isOpen;
    private bool isHeld;
    //private Quaternion bottomCoverRotation;
    private Vector3 initialHandRotation;
    private Vector3 currentHandRotation;
    public GameObject frontCover;
    public GameObject backCover;

    private InputDevice targetDevice;
    public InputDeviceCharacteristics controllerCharacteristics;
    // Start is called before the first frame update
    void Start()
    {
        isOpen = false;
        isHeld = false;
        getDevices();
    }

    // Update is called once per frame
    void Update()
    {
        if (!targetDevice.isValid)
        {
            getDevices();
        }
        else
        {
            if (isHeld && isOpen)
            {
                //backCover.transform.rotation = bottomCoverRotation;

                //Quaternion rotationDistance = currentHandRotation * Quaternion.Inverse(initialHandRotation);
                //backCover.transform.rotation = rotationDistance * backCover.transform.rotation;
                //targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue);
                //if (triggerValue > 0.2)
                //{
                //    //bottomCoverRotation = backCover.transform.rotation;
                //}
                backCover.transform.rotation = frontCover.transform.rotation;
                Vector3 angle = currentHandRotation + initialHandRotation;
                Debug.Log(angle);

                backCover.transform.Rotate(new Vector3(160 + angle.y, 0, 0));
            }
            else
            {
                backCover.transform.rotation = frontCover.transform.rotation;
            }
        }
    }
    
    //void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Left Hand"))
    //    {
    //        if (!targetDevice.isValid)
    //        {
    //            getDevices();
    //        }
    //        else
    //        {
    //            targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue);
    //            if (triggerValue > 0.2)
    //            {
    //                initialHandRotation = other.transform.rotation.eulerAngles;
    //                currentHandRotation = other.transform.rotation.eulerAngles;
    //                isHeld = true;
    //            }
    //        }
    //        //Debug.Log("Hand and wallet began touching each other");
    //    }
    //}

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Left Hand"))
        {
            isHeld = true;
            if (!targetDevice.isValid)
            {
                getDevices();
            }
            else
            {
                targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue);
                if (!isOpen && triggerValue > 0.2)
                {
                    //Debug.Log("Opened Wallet");

                    initialHandRotation = other.transform.rotation.eulerAngles;
                    currentHandRotation = other.transform.rotation.eulerAngles;
                    isOpen = true;
                }
                else if (triggerValue > 0.2)
                {
                    //Debug.Log("Updated Wallet");

                    currentHandRotation = other.transform.rotation.eulerAngles;
                }
                else
                {
                    //Debug.Log("Closed Wallet");
                    isOpen = false;
                }
            }
            //currentHandRotation = other.transform.rotation;

            //Debug.Log("Hand and wallet are touching each other");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Left Hand"))
        {
            //Debug.Log("Hand and wallet stopped touching");
        }
        isHeld = false;
        isOpen = false;
    }

    void getDevices()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);

        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);

        if (devices.Count > 0)
        {
            foreach (var item in devices)
            {
                Debug.Log("Wallet Opener Script: " + item.name + ", " + item.characteristics);
            }
            targetDevice = devices[0];
        }
    }
}
