using cakeslice;
using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class WalletMenu : MonoBehaviour
{
    [Header("Parameters")]
    public int SelectedColor = 1;
    public bool AlwaysVisible = false;
    public int CardsSelectedAtPosition = 1;
    public bool AutoSelect = true;
    public AudioClip SelectionSound;

    [Header("Internal")]
    public GameObject BoxPrefab;
    public GameObject TextPrefab;
    public GameObject HoloCardPrefab;
    public GameObject BoundingBoxPrefab;
    public Shader CardViewerShader;
    public Shader CardDisplayShader;
    [HideInInspector] public List<Card> Cards = new();

    // Use https://www.desmos.com/calculator/bnelriqxg8 to calculate different values, where preferred.
    public readonly double DefaultCardSpace = 0.6;
    public readonly double CardHeight = 0.5;
    public readonly double Fade = 0.85;

    [HideInInspector] public double CardSpace = 0.6;

    private BoxCollider _gripCollider;

    public double MaxScroll { get => Cards.Count - 1 - CardsSelectedAtPosition; } // Computed property
    public double MinScroll { get => -CardsSelectedAtPosition; }

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
                if (MaxScroll >= MinScroll && value > MaxScroll) _scrollPosition = MaxScroll;
                else if (value < MinScroll) _scrollPosition = MinScroll;
                else _scrollPosition = value;
                ViewportChange?.Invoke(this, EventArgs.Empty);
            }
        }
    }
    public event EventHandler ViewportChange;

    private float _visibility = 0;
    public float Visibility
    {
        get => _visibility;
        set
        {
            if (_visibility != value)
            {
                _visibility = value;
                VisibilityChange?.Invoke(this, EventArgs.Empty);
            }
        }
    }
    public event EventHandler VisibilityChange;

    public void Scroll(float percentage)
    {
        ScrollPosition = Mathf.Clamp01(percentage) * MaxScroll;
    }
    public void Scroll(float amount, bool additive)
    {
        if (additive)
        {
            ScrollPosition += amount / CardSpace;
        }
        else
        {
            ScrollPosition = amount / CardSpace;
        }
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

    [SerializeField] private Card _selected;
    public Card Selected
    {
        get => _selected;
        set
        {
            if (_selected != value)
            {
                Card old = _selected;
                _selected = value;
                SelectedChanged?.Invoke(this, new CardChangeEventArgs() { newCard = value, oldCard = old });
            }
        }
    }
    public event EventHandler<CardChangeEventArgs> SelectedChanged;

    // For execution order, see `Card.cs`
    void Awake()
    {
        CardSpace = DefaultCardSpace;
        Vector3 defaultScale = transform.localScale;
        VisibilityChange += (sender, e) =>
        {
            if (AlwaysVisible && Visibility != 1)
            {
                Visibility = 1;
            }
            else
            {
                CardSpace = Mathf.Lerp((float)CardHeight, (float)DefaultCardSpace, (Visibility - 0.8f) / 0.2f);
                if (Visibility < 0.8)
                    transform.localScale = new Vector3(defaultScale.x, defaultScale.y * (Visibility / 0.8f), defaultScale.z * Visibility);
                else
                    transform.localScale = Vector3.Project(defaultScale * Visibility, Vector3.forward) + Vector3.ProjectOnPlane(defaultScale, Vector3.forward);
                ViewportChange?.Invoke(this, EventArgs.Empty);
            }
        };
        ViewportChange += (sender, e) =>
        {
            if (AutoSelect)
            {
                if (Selected != null) Selected.Selected = false;
                if (Visibility >= 0.5)
                {
                    Selected = Cards[(int)Math.Round(ScrollPosition + CardsSelectedAtPosition)];
                    Selected.Selected = true;
                }
                else
                {
                    Selected = null;
                }
            }
        };
        SelectedChanged += (sender, e) =>
        {
            if (e.newCard != null)
            {
                Outline outline = e.newCard.gameObject.AddComponent<Outline>();
                outline.color = SelectedColor;
                if (AutoSelect) PlaySelectedAudio();
            }
            if (e.oldCard != null)
                Destroy(e.oldCard.GetComponent<Outline>());
        };
        _gripCollider = gameObject.AddComponent<BoxCollider>();
        _gripCollider.isTrigger = true;
        _gripCollider.size = new Vector3(1, (float)((Height + Fade) * CardSpace), 0.2f);
        _gripCollider.center = Vector3.up * (float)(Height * CardSpace / 2);
        Cards.AddRange(GetComponentsInChildren<Card>(true));
        foreach (Card card in Cards)
        {
            card.OnParentAwake(this);
        }
    }

    public void PlaySelectedAudio()
    {
        AudioSource.PlayClipAtPoint(SelectionSound,
            transform.localToWorldMatrix.MultiplyPoint(new Vector3(0, (float)GetOffset(Cards.IndexOf(Selected)), 0)));
    }

    public class CardChangeEventArgs : EventArgs
    {
        public Card newCard;
        public Card oldCard;
    }
}
