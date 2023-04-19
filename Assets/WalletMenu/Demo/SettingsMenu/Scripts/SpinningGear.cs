using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningGear : MonoBehaviour
{
    private HoloCard parent;
    private WalletMenu wallet;
    private Vector3 defaultRotation;
    public Vector3 MaximumRotation;

    void Start()
    {
        parent = GetComponentInParent<HoloCard>();
        wallet = parent.GetComponentInParent<WalletMenu>();
        defaultRotation = transform.localEulerAngles;
        parent.OffsetChanged += (sender, e) =>
        {
            transform.localEulerAngles = Vector3.Lerp(defaultRotation, MaximumRotation, (float)(parent.Offset / (wallet.Height * wallet.CardSpace)));
        };
    }
}
