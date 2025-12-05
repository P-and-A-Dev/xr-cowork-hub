using UnityEngine;

public class XRAvatarSelector : MonoBehaviour
{
    [Header("Raycast Settings")]
    public LayerMask avatarLayer;    // Only hit avatars
    public float maxDistance = 10f;  // Maximum raycast distance

    void Update()
    {
        // Example: trigger action (replace with VR input if needed)
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            TrySelectAvatar();
        }
    }
    //Casts a ray and tries to select an avatar.
    void TrySelectAvatar()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, avatarLayer))
        {
            //try to get the AvatarSelectable
            AvatarSelectable selectable = hit.collider.GetComponentInParent<AvatarSelectable>();

            if (selectable != null)
            {
                selectable.OnSelected();
            }
        }
    }
}
