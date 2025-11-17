using UnityEngine;

public class ToggleMenu : MonoBehaviour
{
    [SerializeField] private GameObject menu;

    public void Toggle()
    {
        menu.SetActive(!menu.activeSelf);
    }
}
