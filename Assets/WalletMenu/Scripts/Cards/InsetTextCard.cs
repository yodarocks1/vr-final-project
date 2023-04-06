using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class InsetTextCard : InsetCard
{
    public string Text;
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
            instance = Instantiate(parent.TextPrefab, boxInstance.transform);
            TextMeshPro textMesh = instance.GetComponent<TextMeshPro>();
            textMesh.text = Text;
            SetCardShaderAllChildren(instance.transform);
        }
        ShowingChanged -= Setup;
    }
}
