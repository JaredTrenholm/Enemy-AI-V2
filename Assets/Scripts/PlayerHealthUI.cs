using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public Text healthUI;
    public GameCharacter playerStats;
    void Update()
    {
        healthUI.text = "Health : " + playerStats.health; 
    }
}
