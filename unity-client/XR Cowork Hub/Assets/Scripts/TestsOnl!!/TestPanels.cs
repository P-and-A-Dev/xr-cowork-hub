using UnityEngine;

public class PanelTest : MonoBehaviour
{
    public PanelManager panelManager;

    void Start()
    {
        panelManager.CreatePanel("note", "firrts panel");
    }
}
