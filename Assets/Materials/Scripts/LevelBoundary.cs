using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    public class LevelBoundary : MonoBehaviour
    {
        #region Singleton
        public static LevelBoundary Instance;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError(" на сцене  LevelBoundary уже существует ");
                Destroy(gameObject);
                return;
            }
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }

        #endregion

        [SerializeField] private float m_Radius;
        public float Radius => m_Radius;

        public enum Mode
        {
            Limit,
            Teleport

        }

        [SerializeField] private Mode m_LimitMode;
        public Mode LimitMode => m_LimitMode;

        private void OnDrawGizmosSelected()
        {
            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.DrawWireDisc(transform.position, transform.forward, m_Radius);
        }

    }
}