using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRController))]
public class XRWalletSelector : MonoBehaviour
{
    private LineRenderer rayRenderer;
    public Transform visibleOrigin;
    public XRWalletProvider WalletProvider;
    public WalletMenu Wallet { get => WalletProvider.Wallet; }
    [Flags] public enum SelectionMethod { None = 0, Ray = 1, ClosestInBounds = 2 }
    public SelectionMethod SelectMethod;
    [HideInInspector] public Vector3? SelectLocation = null;
    [HideInInspector] public SelectionMethod HasSelectionType = SelectionMethod.None;
    public bool HasSelection { get => SelectLocation.HasValue; }

    void Start()
    {
        rayRenderer = GetComponent<LineRenderer>();
        rayRenderer.endColor = Color.green;
        rayRenderer.startColor = Color.white;
        rayRenderer.enabled = false;
    }

    private Vector3 devicePosition;
    private Vector3 deviceForward;
    void Update()
    {
        devicePosition = visibleOrigin.localToWorldMatrix.MultiplyPoint(Vector3.zero);
        deviceForward = visibleOrigin.localToWorldMatrix.MultiplyVector(Vector3.forward);
    }
    void FixedUpdate()
    {
        Vector3 pointA = Vector3.negativeInfinity;
        Vector3 pointB = Vector3.negativeInfinity;
        if (SelectMethod.HasFlag(SelectionMethod.Ray) && TryRaycast(true, out Card hit, out pointA))
        {
            Wallet.AutoSelect = false;
            Wallet.Selected = hit;
            HasSelectionType = SelectionMethod.Ray;
        }
        else if (SelectMethod.HasFlag(SelectionMethod.ClosestInBounds) && TryCollide(out Card nearest, out pointB))
        {
            Wallet.AutoSelect = false;
            Wallet.Selected = nearest;
            HasSelectionType = SelectionMethod.ClosestInBounds;
        }
        else
        {
            Wallet.AutoSelect = true;
            HasSelectionType = SelectionMethod.None;
            SelectLocation = null;
        }
        if (HasSelectionType != SelectionMethod.None)
        {
            SelectLocation = pointB.Equals(Vector3.negativeInfinity) ? pointA : pointB;
        }
    }
    private void ShowRay(Vector3 origin, Vector3 hit, Color color)
    {
        rayRenderer.enabled = true;
        Vector3 originLocal = transform.worldToLocalMatrix.MultiplyPoint(origin);
        Vector3 hitLocal = transform.worldToLocalMatrix.MultiplyPoint(hit);
        rayRenderer.SetPositions(new Vector3[] { originLocal, hitLocal });
        rayRenderer.endColor = color;
    }
    private void HideRay()
    {
        rayRenderer.enabled = false;
    }
    private bool TryRaycast(bool render, out Card hit, out Vector3 point)
    {
        if (Physics.Raycast(devicePosition, deviceForward, out RaycastHit hitInfo))
        {
            if (hitInfo.collider.Equals(Wallet.GetComponent<Collider>()))
            {
                hit = GetNearest(hitInfo.point, true);
                if (render) ShowRay(devicePosition, hitInfo.point, Color.green);
                point = hitInfo.point;
            }
            else
            {
                hit = null;
                if (render) ShowRay(devicePosition, hitInfo.point, Color.yellow);
                point = Vector3.negativeInfinity;
            }
        }
        else
        {
            hit = null;
            point = Vector3.negativeInfinity;
            if (render) HideRay();
        }
        return hit != null;
    }
    private bool TryCollide(out Card nearest, out Vector3 point)
    {
        if (TryGetComponent(out Collider col) && col.bounds.Intersects(Wallet.GetComponent<Collider>().bounds))
        {
            point = col.ClosestPoint(devicePosition);
            nearest = GetNearest(point, false);
        }
        else
        {
            point = Vector3.negativeInfinity;
            nearest = null;
        }
        return nearest != null;
    }
    private Card GetNearest(Vector3 location, bool returnNullSpace)
    {
        float currentScrollLocation = (float)(Wallet.ScrollPosition * Wallet.CardSpace);
        float position = (Wallet.transform.worldToLocalMatrix.MultiplyPoint(location).y + currentScrollLocation) / (float)Wallet.CardSpace;
        if (position >= -0.5 && position <= Wallet.Cards.Count - 0.5)
        {
            return Wallet.Cards[Mathf.RoundToInt(position)];
        }
        else if (!returnNullSpace)
        {
            return Wallet.Cards[Mathf.RoundToInt(Mathf.Clamp(position, 0, Wallet.Cards.Count - 1))];
        }
        return null;
    }
}
