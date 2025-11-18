using UnityEngine;

public enum StatType
{
    ActiveDamage,
    AutoDamage
}

public class ShopItem : MonoBehaviour
{
    [SerializeField] private ShopItemData data;
    [SerializeField] private int level = 1;
    [SerializeField] private ShopItemUI shopItemUI;

    private int cost;
    private int value;

    private PlayerStats playerStats;

    private void Start()
    {
        playerStats = PlayerStats.Instance;

        switch (data.statType)
        {
            case StatType.ActiveDamage:
                level = playerStats.activeDamageLevel;
                break;
            case StatType.AutoDamage:
                level = playerStats.autoDamageLevel;
                break;
        }

        ReCalculateCost();
        ReCalculateValue();
        shopItemUI.UpdateUI(cost, value, level);
    }

    public void PurchaseAttempt()
    {
        if (!CanAfford()) return;

        playerStats.UpdateMoney(-cost);

        level++;

        switch (data.statType)
        {
            case StatType.ActiveDamage:
                playerStats.UpdateActiveDamage(value, level);
                break;
            case StatType.AutoDamage:
                playerStats.UpdateAutoDamage(value, level);
                break;
        }

        ReCalculateCost();
        ReCalculateValue();

        if (AtMaxLevel())
            shopItemUI.MaxLevelReached();
        else
            shopItemUI.UpdateUI(cost, value, level);


    }

    private void ReCalculateCost()
    {
        cost = Mathf.FloorToInt(data.baseCost * Mathf.Pow(data.costMultiplier, level - 1));
    }

    private void ReCalculateValue()
    {
        value = data.baseValue + (data.valuePerLevel * level);
    }

    private bool AtMaxLevel() => level >= data.maxLevel;

    private bool CanAfford() => playerStats.GetMoney() >= cost;
}
