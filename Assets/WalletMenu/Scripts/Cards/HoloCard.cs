using System;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class HoloCard : Card
{
    private static readonly Vector3 ROTATE_OFFSET = new(-90, 0, 0);

    public PrefabPositioning PrefabData;
    protected GameObject instance;

    protected GameObject cardInstance;
    public Texture Texture;
    public Material Material;

    // For execution order, see `Card.cs`
    protected override void Awake()
    {
        base.Awake();
        if (Material == null) Material = new Material(Shader.Find("Standard"));
        ShowingChanged += Setup;
    }

    // Runs only once when the object is first shown.
    private void Setup(object sender, EventArgs e)
    {
        if (cardInstance == null)
        {
            cardInstance = Instantiate(parent.HoloCardPrefab, transform);
            foreach (Renderer renderer in cardInstance.GetComponentsInChildren<Renderer>())
            {
                renderer.material = new Material(Material);
                if (Texture != null) renderer.material.mainTexture = Texture;
            }
            instance = Instantiate(PrefabData.Prefab, cardInstance.transform);
            PrefabData.Apply(instance.transform, ROTATE_OFFSET);
        }
        ShowingChanged -= Setup;
    }

    protected override void Update()
    {
        base.Update();
        if (PrefabData.DoUpdate) PrefabData.Apply(instance.transform, ROTATE_OFFSET);
    }
}