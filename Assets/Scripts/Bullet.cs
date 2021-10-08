using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private GameObject attacker;

    
    void Update()
    {
        transform.Translate((Vector3.forward*20)*Time.deltaTime, Space.Self);  
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "GameCharacter" && other.gameObject != attacker)
        {
            other.GetComponent<GameCharacter>().TakeDamage(attacker);
            Destroy(this.gameObject);
        }
    }
    public void SetAttacker(GameObject attackerRef)
    {
        attacker = attackerRef;
        transform.LookAt(attacker.GetComponent<EnemyAI>().target.transform);
    }
}
