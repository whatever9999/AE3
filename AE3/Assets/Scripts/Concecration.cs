using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Concecration : MonoBehaviour
{
    public int damageMultiplierPercentage = 380;
    public int lifetimeInSeconds = 12;
    public int effectInterval = 2;

    private CharacterState creatorCS;

    private float currentEffectInterval = 0;
    private List<CharacterState> enemiesInConcecration;

    private void Start()
    {
        enemiesInConcecration = new List<CharacterState>();
        creatorCS = GameObject.Find("Isilion").GetComponent<CharacterState>();

        StartCoroutine(Remove());
    }

    private void Update()
    {
        for (int i = 0; i < enemiesInConcecration.Count; i++)
        {
            if (enemiesInConcecration[i].tag.Equals("Dead"))
            {
                enemiesInConcecration.Remove(enemiesInConcecration[i]);
            }
        }

        currentEffectInterval += Time.deltaTime;

        if (currentEffectInterval > effectInterval)
        {
            //DamageEnemies
            foreach(CharacterState cs in enemiesInConcecration)
            {
                int damageToDeal = (int)((Random.Range(creatorCS.startAttackDamage[0], creatorCS.startAttackDamage[1]) / 100) * damageMultiplierPercentage);
                cs.DealDamage(damageToDeal, UIManager.ResultType.MAGICALDAMAGE);
            }

            currentEffectInterval = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag.Equals("Enemy"))
        {
            bool canAdd = true;
            foreach(CharacterState cs in enemiesInConcecration)
            {
                if (cs == collision.GetComponent<CharacterState>())
                {
                    canAdd = false;
                }
            }

            if(canAdd)
            {
                enemiesInConcecration.Add(collision.GetComponent<CharacterState>());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Enemy"))
        {
            enemiesInConcecration.Remove(collision.GetComponent<CharacterState>());
        }
    }

    private IEnumerator Remove()
    {
        //Destroy the concecration after its lifetime
        yield return new WaitForSeconds(lifetimeInSeconds);
        Destroy(gameObject);
    }
}
