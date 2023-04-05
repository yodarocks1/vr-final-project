using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CardCameraController : MonoBehaviour
{
    public static Material CardMaterialOut { get; private set; }

    void Awake()
    {
        CardMaterialOut = new(Shader.Find("Unlit/Card Shader"));
    }

    public LayerMask CardInternalsLayer;
    private Camera mainCamera;
    private Camera cardCamera;
    private RenderTexture renderTexture;

    void Start()
    {
        // Hide card internals from the main camera
        mainCamera = GetComponent<Camera>();
        mainCamera.cullingMask &= ~CardInternalsLayer;

        // Create the card renderer camera
        GameObject cardCameraObject = Instantiate(gameObject, transform);
        Destroy(cardCameraObject.GetComponent<CardCameraController>());
        Destroy(cardCameraObject.GetComponent<AudioListener>());
        cardCameraObject.transform.localPosition = Vector3.zero;
        cardCameraObject.name = "Card Renderer";
        cardCamera = cardCameraObject.GetComponent<Camera>();
        cardCamera.cullingMask = CardInternalsLayer;
        cardCamera.clearFlags = CameraClearFlags.SolidColor;
        cardCamera.backgroundColor = Color.gray;

        // Create the render texture
        renderTexture = new RenderTexture(mainCamera.pixelWidth, mainCamera.pixelHeight, 0);
        cardCamera.targetTexture = renderTexture;

        // Set the material's texture
        CardMaterialOut.mainTexture = renderTexture;
    }

    void Update()
    {
        // If the camera's resolution changes, make a new render texture that matches
        if (mainCamera.pixelWidth != renderTexture.width || mainCamera.pixelHeight != renderTexture.height)
        {
            RenderTexture newRenderTexture = new(mainCamera.pixelWidth, mainCamera.pixelHeight, 0);
            cardCamera.targetTexture = newRenderTexture;
            CardMaterialOut.mainTexture = newRenderTexture;
            Destroy(renderTexture); // Take out the garbage
            renderTexture = newRenderTexture;
        }
    }
}
