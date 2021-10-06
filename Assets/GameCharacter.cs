using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacter : MonoBehaviour
{
    public CharacterType type;
    public int health = 50;
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
            Death();
        if(type != CharacterType.Player)
        {
            this.gameObject.GetComponent<EnemyAI>().target = attacker;
            if(this.gameObject.GetComponent<EnemyAI>().type == attacker.gameObject.GetComponent<EnemyAI>().type)
                this.gameObject.GetComponent<EnemyAI>().type = CharacterType.Traitor;
        }
    }
    public void Death()
    {
        this.gameObject.SetActive(false);
    }
}
