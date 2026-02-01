using UnityEngine;

public sealed class UIOverlayController : MonoBehaviour
{
    [SerializeField] private GameObject canvasRoot; // your Canvas (or panel root)
    [SerializeField] private bool startHidden = true;

    public bool IsOpen => canvasRoot != null && canvasRoot.activeSelf;

    private void Awake()
    {
        if (canvasRoot == null)
            canvasRoot = gameObject;

        canvasRoot.SetActive(!startHidden);
    }

    public void Toggle()
    {
        if (canvasRoot == null) return;

        bool next = !canvasRoot.activeSelf;
        canvasRoot.SetActive(next);

        Debug.Log($"[UIOverlayController] Canvas active = {next}");
    }

    public void SetOpen(bool open)
    {
        if (canvasRoot == null) return;
        canvasRoot.SetActive(open);
        Debug.Log($"[UIOverlayController] Canvas active = {open}");
    }
}
