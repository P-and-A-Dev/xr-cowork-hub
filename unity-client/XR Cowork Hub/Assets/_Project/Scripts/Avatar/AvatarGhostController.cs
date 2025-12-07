using UnityEngine;
// Controls the ghost/opacity effect of an avatar based on voice group membership
public class AvatarGhostController : MonoBehaviour
{
    [Header("Visual components")]
    public Renderer avatarRenderer;

    [Header("Opacity settings")]
    [Range(0f, 1f)] public float normalOpacity = 1f;
    [Range(0f, 1f)] public float ghostOpacity = 0.25f;

    private Material _material;

    private void Awake()
    {
        if (avatarRenderer != null)
            _material = avatarRenderer.material;
    }
    // Updates the avatar visual depending on whether the user is in the same voice group.
    public void SetGhost(bool isGhost)
    {
        if (_material == null) return;

        float targetOpacity = isGhost ? ghostOpacity : normalOpacity;

        Color c = _material.color;
        c.a = targetOpacity;
        _material.color = c;
    }
}
