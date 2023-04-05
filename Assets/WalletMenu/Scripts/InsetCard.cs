using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public abstract class InsetCard : Card
{
    protected GameObject boxInstance;

    // For execution order, see `Card.cs`
    protected override void Awake()
    {
        base.Awake();
        ShowingChanged += Setup;
    }

    // Runs only once when the object is first shown.
    private void Setup(object sender, EventArgs e)
    {
        if (boxInstance == null)
        {
            GetComponent<Renderer>().material = CardCameraController.CardMaterialOut;
            boxInstance = Instantiate(parent.BoxPrefab, transform);
            SetCardLayerAllChildren(boxInstance.transform);
        }
        ShowingChanged -= Setup;
    }

    public static void SetLayerAllChildren(Transform transform, int layer)
    {
        transform.gameObject.layer = layer;
        for (int i = 0; i < transform.childCount; i++)
        {
            SetLayerAllChildren(transform.GetChild(i), layer);
        }
    }
    public void SetCardLayerAllChildren(Transform transform)
    {
        SetLayerAllChildren(transform, parent.CardLayer);
    }
}
