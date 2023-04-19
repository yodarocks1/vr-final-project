using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class RGBControls : MonoBehaviour
{
    public Transform Red;
    private float defaultRedHeight;
    public Transform Green;
    private float defaultGreenHeight;
    public Transform Blue;
    private float defaultBlueHeight;

    public Transform SelectionIndicator;
    private int selectedColor = 0;
    public float ColorChangeSpeed = 1;

    public Material OutputMaterial;

    public XRNode Controller;

    private Vector3 _color;
    public Vector3 ColorOut
    {
        get => _color;
        set
        {
            _color = value;
            Red.localScale = new Vector3(Red.localScale.x, defaultRedHeight * _color.x, Red.localScale.z);
            Green.localScale = new Vector3(Green.localScale.x, defaultGreenHeight * _color.y, Green.localScale.z);
            Blue.localScale = new Vector3(Blue.localScale.x, defaultBlueHeight * _color.z, Blue.localScale.z);
            OutputMaterial.color = new Color(_color.x, _color.y, _color.z);
        }
    }

    void Start()
    {
        defaultRedHeight = Red.localScale.y;
        defaultGreenHeight = Green.localScale.y;
        defaultBlueHeight = Blue.localScale.y;
    }

    public float sinceLastChange = 0;
    void FixedUpdate()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(Controller);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 direction);

        int pushingDirection = 0;
        if (direction.x > -0.15 && direction.x < 0.15) pushingDirection = 0;
        else if (direction.x >= 0.15 && pushingDirection != 1)
        {
            pushingDirection = 1;
        }
        else if (direction.x <= -0.15 && pushingDirection != -1)
        {
            pushingDirection = -1;
        }

        if (sinceLastChange >= 0.5 && pushingDirection == 1 && selectedColor < 2)
        {
            selectedColor += 1;
            sinceLastChange = 0;
        }
        else if (sinceLastChange >= 0.5 && pushingDirection == -1 && selectedColor > 0)
        {
            selectedColor -= 1;
            sinceLastChange = 0;
        }

        if (sinceLastChange <= 1) sinceLastChange += Time.fixedDeltaTime;

        UpdateSelectionIndicator(selectedColor);
        ModifyColor(selectedColor, direction.y * Time.fixedDeltaTime * ColorChangeSpeed);
    }

    private void UpdateSelectionIndicator(int color)
    {
        float x = color switch
        {
            0 => Red.localPosition.x,
            1 => Green.localPosition.x,
            2 => Blue.localPosition.x,
            _ => 0,
        };
        SelectionIndicator.localPosition = new Vector3(
            x,
            SelectionIndicator.localPosition.y,
            SelectionIndicator.localPosition.z
            );
    }
    private void ModifyColor(int color, float amount)
    {
        ColorOut += amount * color switch
        {
            0 => Vector3.right,
            1 => Vector3.up,
            2 => Vector3.forward,
            _ => Vector3.zero
        };
        ColorOut = new Vector3(
            Mathf.Clamp01(ColorOut.x),
            Mathf.Clamp01(ColorOut.y),
            Mathf.Clamp01(ColorOut.z)
            );
    }
}
