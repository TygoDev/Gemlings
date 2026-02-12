using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectionLog : MonoBehaviour
{
    [SerializeField] private GameObject collectionLogItemPrefab = null;
    [SerializeField] private GameObject collectionLogContent = null;
    private List<CollectionLogItem> collectionLogItems = new List<CollectionLogItem>();

    private void Start()
    {
        PopulateList();
    }

    public void PopulateList()
    {
        // Clear old UI (important if repopulating)
        foreach (Transform child in collectionLogContent.transform)
            Destroy(child.gameObject);

        collectionLogItems.Clear();

        var sortedGems = GameManager.Instance
            .GetGemSOs()
            .OrderBy(gem => gem.rarityLevel);

        var unlockedCollection = Inventory.Instance.GetCollection();

        foreach (var gem in sortedGems)
        {
            GameObject newLogItem =
                Instantiate(collectionLogItemPrefab, collectionLogContent.transform);

            CollectionLogItem logItem =
                newLogItem.GetComponent<CollectionLogItem>();

            // Check if player has unlocked this gem type
            GemSO unlockedVersion = unlockedCollection
                .FirstOrDefault(g => g.id == gem.id);

            if (unlockedVersion != null)
            {
                // Player has this gem unlocked
                logItem.gem = unlockedVersion; // set BEST unlocked variant
                logItem.Unlock(true);
            }
            else
            {
                // Still locked
                logItem.gem = gem; // default base gem (for silhouette etc.)
                logItem.Unlock(false);
            }

            collectionLogItems.Add(logItem);
        }
    }


}
