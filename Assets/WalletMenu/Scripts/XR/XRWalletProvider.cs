using UnityEngine;

public class XRWalletProvider : MonoBehaviour
{
    public Transform Viewer;
    public WalletMenu Wallet;

    private void FixedUpdate()
    {
        Vector3 displacement = Wallet.transform.position - Viewer.position;
        Vector3 facingDirection = Wallet.transform.rotation * Vector3.forward;
        float percentFacingTowardsHead = Vector3.Project(facingDirection, displacement).magnitude;
        float angle = Vector3.Angle(displacement.normalized, facingDirection);

        Wallet.Visibility = angle > 90 ? 0 : Mathf.Clamp01(Mathf.Lerp(0, 1.2f, percentFacingTowardsHead));
    }
}
