using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text cost;
    [SerializeField] private TMP_Text value;
    [SerializeField] private TMP_Text level;
    [SerializeField] private Button button;

    public void UpdateUI(int pCost, int pValue, int pLevel)
    {
        cost.text = $"$ {pCost}";
        value.text = $"+{pValue}";
        level.text = $"Level: {pLevel}";
    }

    public void MaxLevelReached()
    {
        button.interactable = false;
        level.text = "MAX";
        cost.gameObject.SetActive(false);
    }
}
