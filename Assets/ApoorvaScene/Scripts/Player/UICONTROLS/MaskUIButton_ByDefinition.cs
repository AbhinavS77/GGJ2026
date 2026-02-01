using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public sealed class MaskUIButton_ByDefinition : MonoBehaviour
{
    [SerializeField] private MaskDefinition maskDefinition;
    [SerializeField] private bool autoBind = true;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (autoBind) button.onClick.AddListener(OnClicked);
    }

    public void OnClicked()
    {
        Debug.Log("[MaskUIButton] OnClicked"); 
        if (maskDefinition == null)
        {
            Debug.LogWarning("[MaskUIButton] No MaskDefinition assigned.");
            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogWarning("[MaskUIButton] No GameManager.Instance found.");
            return;
        }

        // ✅ Send the ID forward (MaskType)
        bool ok = GameManager.Instance.RequestEquipMask(maskDefinition.id);

        Debug.Log($"[MaskUIButton] Clicked {maskDefinition.displayName} -> RequestEquipMask({maskDefinition.id}) => {ok}");
    }
}
