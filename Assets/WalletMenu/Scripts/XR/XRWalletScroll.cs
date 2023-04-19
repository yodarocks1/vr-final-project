using System;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRController))]
public class XRWalletScroll : MonoBehaviour
{
    public XRWalletProvider WalletProvider;
    public WalletMenu Wallet { get => WalletProvider.Wallet; }
    [Flags] public enum ScrollingMethod { None = 0, Joystick = 1, Grab = 2 }
    public ScrollingMethod ScrollMethod;
    public bool WhenSelecting;

    private float? grippedLocation = null;
    private float scrollVelocity = 0;
    public float JoystickSpeed = 1;
    public float JoystickStrength = 0.7f;
    public float GripStrength = 100;
    public float RayGripStrength = 2; // Reduces jitter from slight changes in pitch
    public float Friction = 0.5f;

    private XRNode controllerNode;
    private Collider col;
    private XRWalletSelector selector;
    void Start()
    {
        controllerNode = GetComponent<XRController>().controllerNode;
        col = GetComponent<Collider>();
        if (!col)
        {
            ScrollMethod &= ~ScrollingMethod.Grab; // No collider means no grabbing
            Debug.LogWarning("No collider on XRWalletScroll controller, yet Grab was enabled. Disabling Grab.");
        }
        selector = GetComponent<XRWalletSelector>();
        if (WhenSelecting && !selector)
        {
            WhenSelecting = false; // No selector means no knowing when we're selecting
            Debug.LogWarning("No XRWalletSelector on XRWalletScroll controller, yet WhenSelecting was enabled. Disabling WhenSelecting.");
        }
        else if (WhenSelecting && !selector.Wallet.Equals(Wallet))
        {
            WhenSelecting = false;
            Debug.LogWarning("XRWalletSelector and XRWalletScroll listen to different wallets, yet WhenSelecting was enabled. Disabling WhenSelecting.");
        }
    }

    
    private Vector2 primaryAxis;
    private Vector3 location;
    private bool gripButton;
    void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(controllerNode);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out primaryAxis);
        device.TryGetFeatureValue(CommonUsages.devicePosition, out location);
        device.TryGetFeatureValue(CommonUsages.gripButton, out gripButton);
    }

    void FixedUpdate()
    {
        if (Wallet.Visibility >= 0.5f)
        {
            if (Wallet.ScrollPosition == Wallet.MinScroll || Wallet.ScrollPosition == Wallet.MaxScroll)
            {
                scrollVelocity = 0;
                if (!WhenSelecting) grippedLocation = null;
            }
            else if (Friction > 0) scrollVelocity = Mathf.MoveTowards(scrollVelocity, 0, Friction * Time.fixedDeltaTime);
            else scrollVelocity = 0;

            if (WhenSelecting) selector.PlayAudio = !grippedLocation.HasValue && !gripButton;

            if (ScrollMethod.HasFlag(ScrollingMethod.Grab) && (!WhenSelecting || selector.HasSelection))
            {
                float strength = (WhenSelecting && selector.HasSelectionType == XRWalletSelector.SelectionMethod.Ray) ? RayGripStrength : GripStrength;
                if (!gripButton) grippedLocation = null;
                bool allowNewGrip = col.bounds.Intersects(Wallet.GetComponent<Collider>().bounds) || (WhenSelecting && selector.HasSelection);
                if (gripButton && (grippedLocation.HasValue || allowNewGrip))
                {
                    float currentScrollLocation = (float)(Wallet.ScrollPosition * Wallet.CardSpace);
                    Vector3 whichLocation = WhenSelecting && selector.HasSelection ? selector.SelectLocation.Value : location;
                    float controllerLocation = Wallet.transform.worldToLocalMatrix.MultiplyPoint(whichLocation).y + currentScrollLocation;
                    if (!grippedLocation.HasValue) grippedLocation = controllerLocation;
                    scrollVelocity = Mathf.MoveTowards(scrollVelocity, grippedLocation.Value - controllerLocation, strength * Time.fixedDeltaTime);
                }
            }
            if (ScrollMethod.HasFlag(ScrollingMethod.Joystick) && (!WhenSelecting || selector.HasSelection))
            {
                float desiredVelocity = Mathf.Pow(primaryAxis.y, 3);
                if (Mathf.Abs(desiredVelocity) > Mathf.Abs(scrollVelocity))
                {
                    scrollVelocity = Mathf.MoveTowards(scrollVelocity, desiredVelocity, JoystickStrength * Time.fixedDeltaTime);
                }
            }
            Wallet.Scroll(scrollVelocity, true);
        }
        else
        {
            scrollVelocity = 0;
        }
    }
}
