using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }
    public event Action OnMoneyChanged;

    [SerializeField] private PlayerStatsSO playerStatsSO;

    [Header("Shop Levels")]
    public int activeDamageLevel = 1;
    public int autoDamageLevel = 1;

    [Header("Unlocked Lings")]
    public List<int> unlockedLingIndexes = new();
    public int selectedLingIndex = 0;

    private string savePath;
    private PlayerSaveData cachedData;

    private const string PLAYER_PREFS_SAVE_KEY = "PLAYER_SAVE_JSON";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        savePath = Path.Combine(Application.persistentDataPath, "playerSave.json");
        LoadCoreData();
    }

    private void Start()
    {
        if (cachedData == null)
            return;

        Inventory.Instance.ClearInventoryInternal();

        foreach (var gemData in cachedData.inventoryGems)
        {
            GemSO baseGem = GameManager.Instance.GetGemByID(gemData.baseGemID);
            if (baseGem == null)
                continue;

            GemSO gemCopy = Instantiate(baseGem);

            gemCopy.adjective = (Adjective)gemData.adjective;
            gemCopy.trueValue = gemData.trueValue;
            gemCopy.weight = gemData.weight;
            gemCopy.durability = gemData.durability;

            Inventory.Instance.AddGemInternal(gemCopy);
        }
    }

    // ============================================================
    // PUBLIC UPDATE METHODS (SAVE IMMEDIATELY)
    // ============================================================

    public void UpdateMoney(int amount)
    {
        playerStatsSO.money += amount;
        OnMoneyChanged?.Invoke();
        SaveGame();
    }

    public void UpdateActiveDamage(int amount, int level)
    {
        activeDamageLevel = level;
        playerStatsSO.activeDamage = amount;
        SaveGame();
    }

    public void UpdateAutoDamage(int amount, int level)
    {
        autoDamageLevel = level;
        playerStatsSO.autoDamagePerSecond = amount;
        SaveGame();
    }

    public int GetMoney() => playerStatsSO.money;
    public PlayerStatsSO GetPlayerStats() => playerStatsSO;

    // ============================================================
    // SAVE & LOAD
    // ============================================================

    public void SaveGame()
    {
        Debug.Log("SAVING GAME: " + savePath);

        PlayerSaveData data = new PlayerSaveData
        {
            activeDamage = playerStatsSO.activeDamage,
            autoDamagePerSecond = playerStatsSO.autoDamagePerSecond,
            money = playerStatsSO.money,

            activeDamageLevel = activeDamageLevel,
            autoDamageLevel = autoDamageLevel,

            unlockedGemIndexes = new List<int>(unlockedLingIndexes),
            selectedLingIndex = selectedLingIndex
        };

        foreach (GemSO gem in Inventory.Instance.GetAllGems())
        {
            data.inventoryGems.Add(new GemSaveData
            {
                baseGemID = gem.id,
                adjective = (int)gem.adjective,
                trueValue = gem.trueValue,
                weight = gem.weight,
                durability = gem.durability
            });
        }

        string json = JsonUtility.ToJson(data, true);

        // File save (desktop + WebGL when allowed)
        try
        {
            File.WriteAllText(savePath, json);
        }
        catch (Exception e)
        {
            Debug.LogWarning("File save failed: " + e.Message);
        }

        // PlayerPrefs mirror (GUARANTEED on itch.io)
        PlayerPrefs.SetString(PLAYER_PREFS_SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    private void LoadCoreData()
    {
        string json = null;

        // 1️⃣ Try file system
        try
        {
            if (File.Exists(savePath))
                json = File.ReadAllText(savePath);
        }
        catch { }

        // 2️⃣ Fallback to PlayerPrefs
        if (string.IsNullOrEmpty(json) && PlayerPrefs.HasKey(PLAYER_PREFS_SAVE_KEY))
        {
            json = PlayerPrefs.GetString(PLAYER_PREFS_SAVE_KEY);
        }

        if (string.IsNullOrEmpty(json))
            return;

        cachedData = JsonUtility.FromJson<PlayerSaveData>(json);
        if (cachedData == null)
            return;

        playerStatsSO.activeDamage = cachedData.activeDamage;
        playerStatsSO.autoDamagePerSecond = cachedData.autoDamagePerSecond;
        playerStatsSO.money = cachedData.money;

        activeDamageLevel = cachedData.activeDamageLevel;
        autoDamageLevel = cachedData.autoDamageLevel;

        unlockedLingIndexes = new List<int>(cachedData.unlockedGemIndexes);
        selectedLingIndex = cachedData.selectedLingIndex;
    }

    // ============================================================
    // SAFETY NETS
    // ============================================================

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            SaveGame();
    }

    private void OnDisable()
    {
        SaveGame();
    }

    // ============================================================
    // DEBUG
    // ============================================================

    [ContextMenu("Delete Save File")]
    public void DeleteSaveFile()
    {
        playerStatsSO.activeDamage = 20;
        playerStatsSO.autoDamagePerSecond = 10;
        playerStatsSO.money = 0;

        unlockedLingIndexes.Clear();
        selectedLingIndex = 0;

        if (File.Exists(savePath))
            File.Delete(savePath);

        PlayerPrefs.DeleteKey(PLAYER_PREFS_SAVE_KEY);
        PlayerPrefs.Save();

        Debug.Log("Save data deleted.");
    }
}
