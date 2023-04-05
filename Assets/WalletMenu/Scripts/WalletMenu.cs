using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class WalletMenu : MonoBehaviour
{
    public GameObject BoxPrefab;
    public GameObject TextPrefab;
    [Layer] public int CardLayer = 6;
    public readonly List<Card> Cards = new();

    // Use https://www.desmos.com/calculator/bnelriqxg8 to calculate different values, where preferred.
    public readonly double CardSpace = 0.6;
    public readonly double CardHeight = 0.5;
    public readonly double Fade = 0.85;

    public double MaxScroll { get => Cards.Count - Height - 1; } // Computed property

    // Properties that activate the ViewportChange event when modified
    private double _height = 5.0;
    public double Height
    {
        get => _height;
        set
        {
            if (_height != value)
            {
                _height = value;
                ScrollPosition = ScrollPosition;
                ViewportChange?.Invoke(this, EventArgs.Empty);
            }
        }
    }
    private double _scrollPosition = 0.0;
    public double ScrollPosition
    {
        get => _scrollPosition;
        set
        {
            if (_scrollPosition != value)
            {
                if (MaxScroll >= 0 && value > MaxScroll) _scrollPosition = MaxScroll;
                else if (value < 0) _scrollPosition = 0;
                else _scrollPosition = value;
                ViewportChange?.Invoke(this, EventArgs.Empty);
            }
        }
    }
    public event EventHandler ViewportChange;

    public void Scroll(Slider slider)
    {
        ScrollPosition = (slider.value / slider.maxValue) * MaxScroll;
    }

    public float GetVisibility(int index)
    {
        double scrolledPosition = index - ScrollPosition;
        double value;
        if (0 <= scrolledPosition && scrolledPosition < Height)
        {
            value = 1;
        }
        else if (scrolledPosition >= Height)
        {
            value = (-scrolledPosition + Height + Fade) / Fade;
        }
        else
        {
            value = (scrolledPosition + Fade) / Fade;
        }
        return (float)Math.Max(0, value);
    }
    public double GetOffset(int index)
    {
        double scrolledPosition = index - ScrollPosition;
        float visibility = GetVisibility(index);
        if (0 <= scrolledPosition && scrolledPosition < Height)
        {
            return scrolledPosition * CardSpace;
        }
        else if (scrolledPosition >= Height)
        {
            return Height * CardSpace - (visibility - 1) / 2 * CardHeight;
        }
        else
        {
            return (visibility - 1) / 2 * CardHeight;
        }
    }

    // For execution order, see `Card.cs`
    void Awake()
    {
        Cards.AddRange(GetComponentsInChildren<Card>(true));
        foreach (Card card in Cards)
        {
            card.OnParentAwake(this);
        }
    }
}
