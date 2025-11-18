using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ToggleMenu : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private List<GameObject> otherMenus = new List<GameObject>();

    public void Toggle()
    {
        menu.SetActive(!menu.activeSelf);

        if (menu.activeSelf)
        {
            foreach (var otherMenu in otherMenus)
            {
                otherMenu.SetActive(false);
            }
        }
    }
}
