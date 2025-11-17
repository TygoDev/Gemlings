using UnityEngine;

public class ToggleInventory : MonoBehaviour
{
    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject sellAllButton;

    public void Toggle()
    {
        inventory.SetActive(!inventory.activeSelf);
        sellAllButton.SetActive(!sellAllButton.activeSelf);
    }
}
