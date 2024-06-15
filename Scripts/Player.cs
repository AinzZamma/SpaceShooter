using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{

    public class Player : SingletonBase<Player>
    {
        [SerializeField] private int m_NumLives;

        [SerializeField] private SpaceShip m_Ship;

        [SerializeField] private GameObject m_PlayerShipPrefab;

        public SpaceShip ActiveShip => m_Ship;

        [SerializeField] private Camera_Controlled m_CameraController;

        [SerializeField] private MovementController m_MovementController;

        private void Start()
        {
            m_Ship.EventOnDeath.AddListener(OnShopDeath);
        }
        private void OnShopDeath()
        {
            m_NumLives--;

            if (m_NumLives > 0) Respawn();
        }
        private void Respawn()
        {
            var newPlayerShip = Instantiate(m_PlayerShipPrefab);

            m_Ship = newPlayerShip.GetComponent<SpaceShip>();

            m_CameraController.SetTarget(m_Ship.transform);
            m_MovementController.SetTargetShip(m_Ship);

            m_Ship?.EventOnDeath.AddListener(OnShopDeath);
        }
    }
}
