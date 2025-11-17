using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [SerializeField] private PlayerStatsSO playerStatsSO;

    private string savePath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        savePath = Path.Combine(Application.persistentDataPath, "playerSave.json");
    }

    private void Start()
    {
        LoadGame();
    }

    public void UpdateMoney(int amount)
    {
        playerStatsSO.money += amount;
        SaveGame();
    }

    public int GetMoney() => playerStatsSO.money;
    public PlayerStatsSO GetPlayerStats() => playerStatsSO;
    // ============================================================  
    // SAVE & LOAD
    // ============================================================  

    public void SaveGame()
    {
        PlayerSaveData data = new PlayerSaveData();
        data.activeDamagePerSecond = playerStatsSO.activeDamagePerSecond;
        data.autoDamagePerSecond = playerStatsSO.autoDamagePerSecond;
        data.money = playerStatsSO.money;

        // Save inventory gems
        List<GemSO> inv = Inventory.Instance.GetAllGems().ToList();
        foreach (GemSO gem in inv)
        {
            GemSaveData gemData = new GemSaveData
            {
                baseGemID = gem.id,
                adjective = (int)gem.adjective,
                trueValue = gem.trueValue,
                weight = gem.weight,
                durability = gem.durability
            };

            data.inventoryGems.Add(gemData);
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
    }

    public void LoadGame()
    {
        if (!File.Exists(savePath))
            return; // First install -> use defaults

        string json = File.ReadAllText(savePath);
        PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);

        // Load money
        playerStatsSO.activeDamagePerSecond = data.activeDamagePerSecond;
        playerStatsSO.autoDamagePerSecond = data.autoDamagePerSecond;
        playerStatsSO.money = data.money;

        // Rebuild inventory
        Inventory.Instance.ClearInventoryInternal();

        foreach (var gemData in data.inventoryGems)
        {
            GemSO baseGem = GameManager.Instance.GetGemByID(gemData.baseGemID);
            if (baseGem == null)
            {
                Debug.LogError($"Gem ID {gemData.baseGemID} not found in gem pool!");
                continue;
            }

            // Create runtime copy
            GemSO gemCopy = Instantiate(baseGem);

            // Apply saved instance values
            gemCopy.adjective = (Adjective)gemData.adjective;
            gemCopy.trueValue = gemData.trueValue;
            gemCopy.weight = gemData.weight;
            gemCopy.durability = gemData.durability;

            Inventory.Instance.AddGemInternal(gemCopy); // silent add
        }
    }
}
