using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using static ValueManager;

public class SliderCardControl : MonoBehaviour
{
    public Transform SliderThumb;
    public float SliderSpace;
    private float thumbLeft;

    public Transform SizeIndicator;
    public Vector3 MaxIndicatorScale;
    private Vector3 defaultIndicatorScale;

    public TextMeshPro ValueText;
    public XRNode Controller;
    public bool UseController;
    public float SliderSpeed = 1;

    public ValueManager ValueManager;
    public string ValueKey;
    private ValueWrapper<float> _value;
    public float Value
    {
        get => _value.Value;
        set
        {
            if (Value != value)
            {
                _value.Value  = value;
                OnValueChange.Invoke(value);
            }
        }
    }
    public float MinValue;
    public float MaxValue;
    public string Units;

    public UnityEvent<float> OnValueChange;

    void Start()
    {
        thumbLeft = SliderThumb.localPosition.x;
        defaultIndicatorScale = SizeIndicator.localScale;
        _value = (ValueWrapper<float>) ValueManager.Values[ValueKey];
    }

    void Update()
    {
        float valuePercent = (Value - MinValue) / (MaxValue - MinValue);
        SliderThumb.localPosition = new Vector3(
            thumbLeft + valuePercent * SliderSpace,
            SliderThumb.localPosition.y,
            SliderThumb.localPosition.z
            );
        SizeIndicator.localScale = Vector3.Lerp(defaultIndicatorScale, MaxIndicatorScale, valuePercent);

        if (UseController)
        {
            InputDevice device = InputDevices.GetDeviceAtXRNode(Controller);
            device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 inputAxis);
            Value += inputAxis.x * SliderSpeed * Time.deltaTime;
            device.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool inputClick);
            if (inputClick) Value = Mathf.Round(Value * 100) / 100;
        }

        if (Value > MaxValue) Value = MaxValue;
        else if (Value < MinValue) Value = MinValue;

        ValueText.text = Value.ToString("F2") + Units;
    }
}
