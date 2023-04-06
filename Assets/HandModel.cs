using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;


public class HandModel : MonoBehaviour
{
    public InputDeviceCharacteristics ControllerCharacteristics;
    private InputDevice targetDevice;

    public GameObject ControllerPrefab;
    private ControllerView controller;

    public GameObject HandModelPrefab;
    private ControllerView hand;

    void Start()
    {
        GetDevices();
    }

    private readonly InputData inputs = new();
    void Update()
    {
        if(!targetDevice.isValid)
        {
            GetDevices();
        }
        else
        {
            controller.Show = true;
            //if (targetDevice.TryGetFeatureValue(CommonUsages.userPresence, out bool userPresent))
            //{
            //    controller.Show = !userPresent;
            //    hand.Show = userPresent;
            //}
            inputs.Update(targetDevice);
            controller.UpdateAnimator(inputs);
            //hand.UpdateAnimator(inputs);
        }
    }

    void GetDevices()
    {
        List<InputDevice> devices = new();
        InputDevices.GetDevices(devices);
        InputDevices.GetDevicesWithCharacteristics(ControllerCharacteristics, devices);

        if(devices.Count > 0)
        {
            foreach (var item in devices)
            {
                Debug.Log(item.name + ", " + item.characteristics);
            }
            targetDevice = devices[0];
            GameObject controllerInstance = Instantiate(ControllerPrefab, transform);
            //GameObject handInstance = Instantiate(HandModelPrefab, transform);
            controller = new ControllerView(controllerInstance, controllerInstance.GetComponent<Animator>());
            //hand = new ControllerView(handInstance, handInstance.GetComponent<Animator>());
        }
    }

    private class InputData
    {
        public float Trigger;
        public float Grip;

        public Vector2 Joystick;
        public bool JoystickTouch;
        public bool JoystickClick;

        public bool PrimaryButton;
        public bool PrimaryButtonTouch;

        public bool SecondaryButton;
        public bool SecondaryButtonTouch;

        public bool MenuButton;

        public void Update(InputDevice device)
        {
            device.TryGetFeatureValue(CommonUsages.trigger, out Trigger);
            device.TryGetFeatureValue(CommonUsages.grip, out Grip);

            if (!device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Joystick))
                device.TryGetFeatureValue(CommonUsages.secondary2DAxis, out Joystick);
            if (!device.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out JoystickTouch))
                device.TryGetFeatureValue(CommonUsages.secondary2DAxisTouch, out JoystickTouch);
            if (!device.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out JoystickClick))
                device.TryGetFeatureValue(CommonUsages.secondary2DAxisClick, out JoystickClick);

            device.TryGetFeatureValue(CommonUsages.primaryButton, out PrimaryButton);
            device.TryGetFeatureValue(CommonUsages.primaryTouch, out PrimaryButtonTouch);

            device.TryGetFeatureValue(CommonUsages.secondaryButton, out SecondaryButton);
            device.TryGetFeatureValue(CommonUsages.secondaryTouch, out SecondaryButtonTouch);

            device.TryGetFeatureValue(CommonUsages.menuButton, out MenuButton);
        }
    }

    private class ControllerView
    {
        public GameObject Instance;
        public Animator Animator;
        private bool _show = true;
        public bool Show
        {
            get => _show;
            set
            {
                if (_show != value)
                {
                    _show = value;
                    Instance.SetActive(value);
                }
            }
        }

        public ControllerView(GameObject instance, Animator animator)
        {
            Instance = instance;
            Animator = animator;
        }

        public void UpdateAnimator(InputData inputs)
        {
            if (Show)
            {
                Animator.SetFloat("Trigger", inputs.Trigger);
                Animator.SetFloat("Grip", inputs.Grip);

                Animator.SetFloat("Joystick X", inputs.Joystick.x);
                Animator.SetFloat("Joystick Y", inputs.Joystick.y);
                Animator.SetBool("Joystick Touch", inputs.JoystickTouch);
                Animator.SetBool("Joystick Click", inputs.JoystickClick);

                Animator.SetBool("Button 1", inputs.PrimaryButton);
                Animator.SetBool("Button 1 Touch", inputs.PrimaryButtonTouch);

                Animator.SetBool("Button 2", inputs.SecondaryButton);
                Animator.SetBool("Button 2 Touch", inputs.SecondaryButtonTouch);

                Animator.SetBool("Button 3", inputs.MenuButton);
            }
        }
    }
}
