using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CollectionLogItem : MonoBehaviour
{
    public GemSO gem;

    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemWeightText;
    [SerializeField] private TMP_Text itemValueText;
    [SerializeField] private Image itemSpriteImage;
    [SerializeField] private bool unlocked;

    private void Start()
    {
        SetItem();
    }

    public void SetItem()
    {
        if (gem == null)
        {
            Debug.LogWarning("GemSO is not assigned in CollectionLogItem.");
            return;
        }

        if (gem.adjective != Adjective.None) itemNameText.text = gem.adjective + " " + gem.itemName;
        else itemNameText.text = gem.itemName;

        itemSpriteImage.sprite = gem.icon;

        if (unlocked)
        {
            itemWeightText.text = gem.weight.ToString("0.00") + " kg";
            itemValueText.text = "$ " + gem.trueValue.ToString();
            GetComponent<Button>().interactable = true;
        }
        else
        {
            itemWeightText.text = "??? kg";
            itemValueText.text = "$ ???";
            GetComponent<Button>().interactable = false;
        }
            SetRarityColour();
    }

    private void SetRarityColour()
    {
        Image background = GetComponent<Image>();

        if (gem.rarityLevel == Rarity.Abundant)
            background.color = Color.gray;
        else if (gem.rarityLevel == Rarity.Uncommon)
            background.color = new Color(0f, 0.5f, 0f); // Dark Green
        else if (gem.rarityLevel == Rarity.Valuable)
            background.color = Color.blue;
        else if (gem.rarityLevel == Rarity.Precious)
            background.color = new Color(0.58f, 0f, 0.83f); // Purple
        else if (gem.rarityLevel == Rarity.Mythic)
            background.color = Color.orange;
    }

    public void Unlock(bool value)
    {
        unlocked = value;
        SetItem();
    }
}
