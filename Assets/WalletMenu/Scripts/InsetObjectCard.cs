using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class InsetObjectCard : InsetCard
{
    public GameObject Prefab;
    public Vector3 PrefabPosition;
    public Vector3 PrefabRotation;
    public Vector3 PrefabScale;
    public bool DoUpdate = false;
    private GameObject instance;

    // For execution order, see `Card.cs`
    protected override void Awake()
    {
        base.Awake();
        ShowingChanged += Setup;
    }

    // Runs only once when the object is first shown.
    private void Setup(object sender, EventArgs e)
    {
        if (instance == null)
        {
            instance = Instantiate(Prefab, boxInstance.transform);
            instance.transform.localPosition = PrefabPosition;
            instance.transform.localRotation = Quaternion.Euler(PrefabRotation);
            instance.transform.localScale = PrefabScale;
            SetCardLayerAllChildren(instance.transform);
        }
        ShowingChanged -= Setup;
    }

    protected override void Update()
    {
        base.Update();
        if (DoUpdate)
        {
            instance.transform.localPosition = PrefabPosition;
            instance.transform.localRotation = Quaternion.Euler(PrefabRotation);
            instance.transform.localScale = PrefabScale;
        }
    }
}
