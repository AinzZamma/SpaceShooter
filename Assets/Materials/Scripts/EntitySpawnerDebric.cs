using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    public class EntitySpawnerDebric : MonoBehaviour
    {
        [SerializeField] private Destructible[] m_DebrisPrefab;

        [SerializeField] private int m_NumDebris;

        [SerializeField] private CycleArea m_Area;

        [SerializeField] private float m_RandomSpeed;



        private void Start()
        {
            for(int i = 0; i < m_NumDebris; i++)
            { 
                SpawnDebric(); 
            }
        }

        private void SpawnDebric()
        {
            int index = Random.Range(0, m_DebrisPrefab.Length);
            GameObject debris = Instantiate(m_DebrisPrefab[index].gameObject);

            debris.transform.position = m_Area.GetRandomInsideZone();
            debris.GetComponent<Destructible>().EventOnDeath.AddListener(OnDebrisDead);

            Rigidbody2D rb = debris.GetComponent<Rigidbody2D>();

            if (rb != null && m_RandomSpeed > 0) 
            {
                rb.velocity = (Vector2)UnityEngine.Random.insideUnitSphere * m_RandomSpeed;
            }

        }

        private void OnDebrisDead()
        {
            SpawnDebric();
        }
    }
}