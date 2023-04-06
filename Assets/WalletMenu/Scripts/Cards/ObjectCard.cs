using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class ObjectCard : Card
{
    public PrefabPositioning PrefabData;
    protected GameObject instance;

    protected GameObject boundingBoxInstance;

    // For execution order, see `Card.cs`
    protected override void Awake()
    {
        base.Awake();
        ShowingChanged += Setup;
    }

    // Runs only once when the object is first shown.
    private void Setup(object sender, EventArgs e)
    {
        if (boundingBoxInstance == null)
        {
            boundingBoxInstance = Instantiate(parent.BoundingBoxPrefab, transform);
            instance = Instantiate(PrefabData.Prefab, boundingBoxInstance.transform);
            PrefabData.Apply(instance.transform);
            if (instance.TryGetComponent(out Collider col))
            {
                col.enabled = false;
            }
        }
        ShowingChanged -= Setup;
    }

    protected override void Update()
    {
        base.Update();
        if (PrefabData.DoUpdate) PrefabData.Apply(instance.transform);
    }
}
