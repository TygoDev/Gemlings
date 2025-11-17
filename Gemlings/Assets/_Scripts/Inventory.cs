using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    [Header("References")]
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private GameObject itemContainerPrefab;
    [SerializeField] private Transform itemsParent;
    [SerializeField] private List<GemSO> inventory;

    private PlayerStats playerStats => PlayerStats.Instance;

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple Inventory instances found! Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        PopulateInventory();
        UpdateMoneyUI();
    }

    // --------------------------
    // UI / Setup
    // --------------------------
    private void PopulateInventory()
    {
        foreach (Transform child in itemsParent)
            Destroy(child.gameObject);

        foreach (GemSO gem in inventory)
        {
            var item = Instantiate(itemContainerPrefab, itemsParent)
                .GetComponent<ItemContainer>();
            item.gem = gem;
        }
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = $"${playerStats.GetMoney()}";
    }

    // --------------------------
    // Inventory Management
    // --------------------------
    public void AddGem(GemSO newGem)
    {
        if (newGem == null) return;

        inventory.Add(newGem);
        PopulateInventory();

        PlayerStats.Instance.SaveGame();
    }

    public void SellGem(GemSO gemToSell)
    {
        if (gemToSell == null)
        {
            return;
        }

        if (!inventory.Contains(gemToSell))
        {
            return;
        }

        // Add its true value to money
        playerStats.UpdateMoney(gemToSell.trueValue);
        UpdateMoneyUI();

        // Remove gem and refresh inventory
        inventory.Remove(gemToSell);
        PopulateInventory();

        PlayerStats.Instance.SaveGame();
    }

    // Optional helper for selling all gems at once
    public void SellAllGems()
    {
        int totalValue = 0;
        foreach (var gem in inventory)
            totalValue += gem.trueValue;

        playerStats.UpdateMoney(totalValue);
        inventory.Clear();

        UpdateMoneyUI();
        PopulateInventory();

        PlayerStats.Instance.SaveGame();
    }

    public void ClearInventoryInternal()
    {
        inventory.Clear();
    }

    public void AddGemInternal(GemSO gem)
    {
        inventory.Add(gem);
    }


    public IReadOnlyList<GemSO> GetAllGems() => inventory;
}
