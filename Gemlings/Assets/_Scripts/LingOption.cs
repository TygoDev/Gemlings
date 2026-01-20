using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LingOption : MonoBehaviour
{
    [SerializeField] private int cost = 0;
    [SerializeField] private List<GameObject> gemlings = new List<GameObject>();
    [SerializeField] private int gemlingIndex = 0;
    [SerializeField] private bool unlocked = false;

    [SerializeField] private TMP_Text costText;

    private void Start()
    {
        if (PlayerStats.Instance.unlockedLingIndexes.Contains(gemlingIndex))
        {
            UnlockGemling();

            if(PlayerStats.Instance.selectedLingIndex == gemlingIndex)
            {
                SelectGemling();
            }
        }
    }

    public void SelectGemling()
    {
        if (!unlocked)
        {
            BuyGemling();
            return;
        }

        if (!PlayerStats.Instance.unlockedLingIndexes.Contains(gemlingIndex))
        {
            PlayerStats.Instance.unlockedLingIndexes.Add(gemlingIndex);
            PlayerStats.Instance.GetPlayerStats().unlockedGemIndexes.Add(gemlingIndex);
            PlayerStats.Instance.SaveGame();
        }
            

        foreach (GameObject gemling in gemlings)
        {
            gemling.SetActive(false);
        }

        gemlings[gemlingIndex].SetActive(true);
        PlayerStats.Instance.selectedLingIndex = gemlingIndex;
        PlayerStats.Instance.SaveGame();
    }

    public void BuyGemling()
    {
        if (PlayerStats.Instance.GetMoney() >= cost)
        {
            PlayerStats.Instance.UpdateMoney(-cost);
            UnlockGemling();
            SelectGemling();
        }
    }

    private void UnlockGemling()
    {
        unlocked = true;
        costText.text = "unlocked!";
    }
}
