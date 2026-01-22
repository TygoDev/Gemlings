using UnityEngine;
using TMPro;

public class LingOption : MonoBehaviour
{
    [SerializeField] private int lingIndex;
    public int LingIndex => lingIndex;  
    public bool unlocked = false;
    public TMP_Text costText = null;
    public int cost = 0;
}
