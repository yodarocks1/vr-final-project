using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
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
            GetComponent<Renderer>().material.shader = parent.CardViewerShader;
            boxInstance = Instantiate(parent.BoxPrefab, transform);
            SetCardShaderAllChildren(boxInstance.transform);
        }
        ShowingChanged -= Setup;
    }

    public static void SetShaderAllChildren(Transform transform, Shader shader)
    {
        if (transform.TryGetComponent(out Renderer renderer))
        {
            if (renderer.material.shader.FindPropertyIndex("_Stencil") == -1)
            {
                renderer.material.shader = shader;
            }
            int idx = renderer.material.shader.FindPropertyIndex("_Stencil");
            switch (renderer.material.shader.GetPropertyType(idx))
            {
                case UnityEngine.Rendering.ShaderPropertyType.Float:
                    renderer.material.SetFloat("_Stencil", 22);
                    renderer.material.SetFloat("_StencilComp", 3);
                    renderer.material.SetFloat("_StencilOp", 0);
                    break;
                case UnityEngine.Rendering.ShaderPropertyType.Int:
                    renderer.material.SetInt("_Stencil", 22);
                    renderer.material.SetInt("_StencilComp", 3);
                    renderer.material.SetInt("_StencilOp", 0);
                    break;
            }
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            SetShaderAllChildren(transform.GetChild(i), shader);
        }
    }
    public void SetCardShaderAllChildren(Transform transform)
    {
        SetShaderAllChildren(transform, parent.CardDisplayShader);
    }
}
