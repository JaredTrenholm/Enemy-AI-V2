using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacter : MonoBehaviour
{
    public CharacterType type;
    private int health = 50;
    public enum CharacterType
    {
        Enemy,
        NonAligned,
        Traitor,
        Player
    }

    public void TakeDamage(GameObject attacker)
    {
        health -= 10;
        if (health <= 0)
            this.gameObject.SetActive(false);
        if(type != CharacterType.Player)
        {
            this.gameObject.GetComponent<EnemyAI>().target = attacker;
            this.gameObject.GetComponent<EnemyAI>().type = CharacterType.Traitor;
        }
    }
}
