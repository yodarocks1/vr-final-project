using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class InsetObjectCard : InsetCard
{
    public PrefabPositioning PrefabData;
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
            instance = Instantiate(PrefabData.Prefab, boxInstance.transform);
            instance.SetActive(true);
            PrefabData.Apply(instance.transform);
            SetCardShaderAllChildren(instance.transform);
        }
        ShowingChanged -= Setup;
    }

    protected override void Update()
    {
        base.Update();
        if (PrefabData.DoUpdate) PrefabData.Apply(instance.transform);
    }
}