using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card : MonoBehaviour
{
    private float _visibility = 0f;
    public float Visibility
    {
        get => _visibility;
        set
        {
            if (_visibility != value)
            {
                _visibility = value;
                if (_visibility > 0) Showing = true;
                else Showing = false;
                VisibilityChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
    public event EventHandler VisibilityChanged;

    private double _offset;
    public double Offset
    {
        get => _offset;
        set
        {
            if (_offset != value)
            {
                _offset = value;
                OffsetChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
    public event EventHandler OffsetChanged;

    [SerializeField] private bool _showing = false;
    public bool Showing
    {
        get => _showing;
        set
        {
            if (_showing != value)
            {
                _showing = value;
                ShowingChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
    public event EventHandler ShowingChanged;

    private int Position;
    protected WalletMenu parent;

    private Vector3 defaultScale;
    private Vector3 defaultPosition;

    /* If execution order is correct, we should see:
     *  1. WalletMenu.Awake()
     *  2. Card.OnParentAwake(wallet : WalletMenu)
     *  3. Card.Awake()
     *  4. InsetCard.Awake()
     *  5. InsetTextCard.Awake()
     *     InsetObjectCard.Awake()
     *  6. Card.Start()
     *  
     * This means our execution order should look like:
     *     WalletMenu < Card < InsetCard < (InsetTextCard & InsetObjectCard)
     */

    public void OnParentAwake(WalletMenu parent)
    {
        this.parent = parent;
        parent.ViewportChange += (sender, e) => UpdatePositioning();
    }

    protected virtual void Awake()
    {
        defaultScale = transform.localScale;
        defaultPosition = transform.localPosition;
        if (Math.Abs(defaultPosition.y) > parent.CardSpace)
        {
            defaultPosition.y = (float)parent.CardSpace * Math.Sign(defaultPosition.y);
        }

        VisibilityChanged += (sender, e) => transform.localScale = defaultScale * Visibility;
        OffsetChanged += (sender, e) => transform.localPosition = defaultPosition + new Vector3(0, (float)Offset, 0);
        ShowingChanged += (sender, e) =>
        {
            if (Showing) gameObject.SetActive(true);
            else gameObject.SetActive(false);
        };
    }
    protected virtual void Start()
    {
        UpdatePositioning();
    }
    protected virtual void Update() { }

    private void UpdatePositioning()
    {
        Position = parent.Cards.IndexOf(this);
        Visibility = parent.GetVisibility(Position);
        Offset = parent.GetOffset(Position);
    }
}