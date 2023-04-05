using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraUIController : MonoBehaviour
{
    public Slider XSlider;
    public Slider YSlider;
    public float CoordinateMultiplier;

    public Slider YawSlider;
    public Slider PitchSlider;
    public float RotationMultiplier;
    public Camera Camera;

    void Start()
    {
        XSlider.onValueChanged.AddListener(_ => UpdatePosition());
        YSlider.onValueChanged.AddListener(_ => UpdatePosition());

        YawSlider.onValueChanged.AddListener(_ => UpdateRotation());
        PitchSlider.onValueChanged.AddListener(_ => UpdateRotation());
    }

    private void UpdatePosition()
    {
        Camera.transform.position = new Vector3(
            XSlider.value * CoordinateMultiplier,
            YSlider.value * CoordinateMultiplier,
            Camera.transform.position.z
        );
    }
    private void UpdateRotation()
    {
        Camera.transform.eulerAngles = new Vector3(
            PitchSlider.value * RotationMultiplier,
            YawSlider.value * RotationMultiplier,
            Camera.transform.eulerAngles.z
        );
    }
}
