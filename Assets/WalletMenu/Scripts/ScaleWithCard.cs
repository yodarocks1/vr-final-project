using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ScaleWithCard : MonoBehaviour
{
    public ScalableField Field;
    public float Add;
    public float Multiply;
    public float Divide;
    
    void Start()
    {
        switch (Field)
        {
            case ScalableField.PositionX:
                Vector3 pos = transform.localPosition;
                transform.localPosition = new Vector3(
                    Modify(pos.x), pos.y, pos.z);
                break;
            case ScalableField.PositionY:
                pos = transform.localPosition;
                transform.localPosition = new Vector3(
                    pos.x, Modify(pos.y), pos.z);
                break;
            case ScalableField.PositionZ:
                pos = transform.localPosition;
                transform.localPosition = new Vector3(
                    pos.x, pos.y, Modify(pos.z));
                break;
            case ScalableField.ScaleX:
                Vector3 scale = transform.localScale;
                transform.localScale = new Vector3(
                    Modify(scale.x), scale.y, scale.z);
                break;
            case ScalableField.ScaleY:
                scale = transform.localScale;
                transform.localScale = new Vector3(
                    scale.x, Modify(scale.y), scale.z);
                break;
            case ScalableField.ScaleZ:
                scale = transform.localScale;
                transform.localScale = new Vector3(
                    scale.x, scale.y, Modify(scale.z));
                break;
            case ScalableField.RectWidth:
                if (transform.TryGetComponent(out RectTransform r))
                {
                    Vector2 size = r.sizeDelta;
                    r.sizeDelta = new Vector2(
                        Modify(size.x), size.y);
                }
                break;
            case ScalableField.RectHeight:
                if (transform.TryGetComponent(out r))
                {
                    Vector2 size = r.sizeDelta;
                    r.sizeDelta = new Vector2(
                        size.x, Modify(size.y));
                }
                break;
        }
    }

    private float Modify(float field)
    {
        WalletMenu wallet = GetComponentInParent<WalletMenu>();
        float cardHeight = (float)wallet.CardHeight;
        if (Multiply != 0) field *= cardHeight * Multiply;
        if (Add != 0) field += cardHeight * Add;
        if (Divide != 0) field /= cardHeight * Divide;
        return field;
    }

    public enum ScalableField
    {
        PositionX,
        PositionY,
        PositionZ,
        ScaleX,
        ScaleY,
        ScaleZ,
        RectWidth,
        RectHeight
    }
}
