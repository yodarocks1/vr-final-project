using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoloAppearScript : MonoBehaviour
{
    private HoloCard parent;
    private WalletMenu wallet;
    private Vector3 defaultScale;

    void Start()
    {
        parent = GetComponentInParent<HoloCard>();
        wallet = parent.GetComponentInParent<WalletMenu>();
        defaultScale = transform.localScale;
        parent.OffsetChanged += (sender, e) =>
        {
            transform.localScale = defaultScale * CalculateScale((float)(parent.Offset / (wallet.Height * wallet.CardSpace)));
        };
    }

    private float CalculateScale(float offset)
    {
        offset = Mathf.Clamp01(offset);
        if (offset >= 0.6f)
        {
            return (offset - 0.6f) / 0.4f;
        }
        else if (offset <= 0.4f)
        {
            return (0.4f - offset) / 0.4f;
        }
        else
        {
            return 0;
        }
    }
}
