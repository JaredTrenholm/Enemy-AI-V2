using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeColorChange : MonoBehaviour
{
    public GameObject enemy;
    public GameObject enemyEye;
    public GameObject traitorEye;
    void Update()
    {
        switch (enemy.GetComponent<EnemyAI>().type) {
            case GameCharacter.CharacterType.Team1:
                enemyEye.SetActive(true);
                traitorEye.SetActive(false);
                break;
            case GameCharacter.CharacterType.Traitor:
                enemyEye.SetActive(false);
                traitorEye.SetActive(true);
                break;
        }
    }
}
