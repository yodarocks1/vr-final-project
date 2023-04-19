using UnityEngine;
using UnityEngine.XR;

public class XRWalletGrabber : MonoBehaviour
{
    public XRWalletProvider WalletProvider;
    public WalletMenu Wallet { get => WalletProvider.Wallet; }
    public XRNode GrabbingNode;
    public bool UseGrabbingNode;
    public CommonBoolUsages GrabInput = CommonBoolUsages.GRIP;
    public bool GrabToggle = false;
    public Transform Parent;

    public bool Grabbing;
    private GameObject grabObject;

    private bool grabbingLast;
    void Update()
    {
        if (UseGrabbingNode && GrabInput != CommonBoolUsages.MANUAL)
        {
            InputDevice device = InputDevices.GetDeviceAtXRNode(GrabbingNode);
            device.TryGetFeatureValue(FeatureUsage(GrabInput), out bool g);
            if (grabbingLast != g)
            {
                grabbingLast = g;
                if (GrabToggle)
                {
                    if (g) Grabbing = !Grabbing;
                }
                else
                {
                    Grabbing = g;
                }
            }
        }
    }
    private void FixedUpdate()
    {
        if (Grabbing && !grabObject)
        {
            grabObject = Wallet.Selected.Grab(Parent);
            grabObject.SetActive(true);
        }
        else if (!Grabbing && grabObject)
        {
            grabObject.SetActive(false);
            grabObject = null;
        }
    }

    public enum CommonBoolUsages
    {
        MANUAL,
        GRIP,
        TRIGGER,
        PRIMARY_BUTTON,
        SECONDARY_BUTTON,
        JOYSTICK_CLICK
    }
    private static InputFeatureUsage<bool> FeatureUsage(CommonBoolUsages commonBoolUsages)
    {
        return commonBoolUsages switch
        {
            CommonBoolUsages.GRIP => CommonUsages.gripButton,
            CommonBoolUsages.TRIGGER => CommonUsages.triggerButton,
            CommonBoolUsages.PRIMARY_BUTTON => CommonUsages.primaryButton,
            CommonBoolUsages.SECONDARY_BUTTON => CommonUsages.secondaryButton,
            CommonBoolUsages.JOYSTICK_CLICK => CommonUsages.primary2DAxisClick,
            _ => default,
        };
    }
}
