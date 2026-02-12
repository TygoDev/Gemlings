using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    [Header("References")]
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private GameObject itemContainerPrefab;
    [SerializeField] private Transform itemsParent;
    [SerializeField] private CollectionLog collectionLog;

    [Header("Runtime Inventory (Sellable)")]
    [SerializeField] private List<GemSO> inventory = new();

    [Header("Collection Log (Unique Best Gems)")]
    [SerializeField] private List<GemSO> collection = new();

    private PlayerStats playerStats => PlayerStats.Instance;

    private void OnEnable()
    {
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.OnMoneyChanged += UpdateMoneyUI;
    }

    private void OnDisable()
    {
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.OnMoneyChanged -= UpdateMoneyUI;
    }

    private void Awake()
    {
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

    // ==========================
    // UI
    // ==========================

    private void PopulateInventory()
    {
        foreach (Transform child in itemsParent)
            Destroy(child.gameObject);

        var sortedInventory = inventory
            .OrderBy(gem => (int)gem.rarityLevel)
            .ToList();

        foreach (GemSO gem in sortedInventory)
        {
            var item = Instantiate(itemContainerPrefab, itemsParent)
                .GetComponent<ItemContainer>();

            item.gameObject.name = $"{gem.rarityLevel} - {gem.name}";
            item.gem = gem;
        }
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = $"{playerStats.GetMoney()}";
    }

    // ==========================
    // Inventory Management
    // ==========================

    public void AddGem(GemSO newGem)
    {
        if (newGem == null) return;

        // Add to normal inventory
        inventory.Add(newGem);

        // Update collection log
        UpdateCollection(newGem);

        PopulateInventory();
        PlayerStats.Instance.SaveGame();
    }

    private void UpdateCollection(GemSO newGem)
    {
        // Find gem in collection with same base identity
        // This assumes gems share a base name or ID
        GemSO existingGem = collection
            .FirstOrDefault(g => g.id == newGem.id);

        if (existingGem == null)
        {
            // New gem type discovered
            collection.Add(newGem);
        }
        else
        {
            // Replace only if new gem is higher rarity/value
            if (newGem.trueValue > existingGem.trueValue)
            {
                int index = collection.IndexOf(existingGem);
                collection[index] = newGem;
            }
        }

        collectionLog.PopulateList();
    }

    public void SellGem(GemSO gemToSell)
    {
        if (gemToSell == null) return;
        if (!inventory.Contains(gemToSell)) return;

        playerStats.UpdateMoney(gemToSell.trueValue);

        inventory.Remove(gemToSell);

        UpdateMoneyUI();
        PopulateInventory();

        PlayerStats.Instance.SaveGame();
    }

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

    // ==========================
    // Internal Save Helpers
    // ==========================

    public void ClearCollectionInternal()
    {
        collection.Clear();
    }

    public void AddCollectionInternal(GemSO gem)
    {
        collection.Add(gem);
    }


    public void ClearInventoryInternal()
    {
        inventory.Clear();
    }

    public void AddGemInternal(GemSO gem)
    {
        inventory.Add(gem);
        UpdateCollection(gem);
    }

    public IReadOnlyList<GemSO> GetAllGems() => inventory;
    public IReadOnlyList<GemSO> GetCollection() => collection;
}
