using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SpaceShooter
{
    /// <summary>
    /// ������������ ������ �� �����. �� ��� ����� ����� ��.
    /// </summary>
    public class Destructible : Entity
    {
        #region Properties
        /// <summary>
        /// ������ ���������� �����������.
        /// </summary>
        [SerializeField] private bool m_Indestructible;
        public bool IsIndestructible => m_Indestructible;

        /// <summary>
        /// ��������� ���������� ��.
        /// </summary>
        [SerializeField] private int m_HitPoints;

        /// <summary>
        /// ������� ��.
        /// </summary>
        private int m_CurrentHitPoints;
        public int HitPoints => m_CurrentHitPoints;
        #endregion

        #region Unity Events
        protected virtual void Start()
            {
            m_CurrentHitPoints = m_HitPoints;
            }
        #endregion

        #region Public API
        public void ApplyDamage(int damage)
        {
            if (m_Indestructible) return;
            m_CurrentHitPoints -= damage;
            if (m_CurrentHitPoints <= 0) OnDeath();
            


            
        }




        #endregion

        /// <summary>
        /// ����������������� ������� ����������� �������, ����� �� ���� ����.
        /// </summary>
        protected virtual void OnDeath()
        {
            Destroy(gameObject);

            m_EventOnDeath?.Invoke();

        }


        [SerializeField] private UnityEvent m_EventOnDeath;

        public UnityEvent EventOnDeath => m_EventOnDeath;

    }
}