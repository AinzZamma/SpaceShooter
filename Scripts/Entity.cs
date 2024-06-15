using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceShooter
{
    /// <summary>
    /// Базовый класс всех интерактивных игровых объектов на сцене.
    /// </summary>
    public abstract class Entity : MonoBehaviour
    {
        [SerializeField]
        private string m_NickName;
        public string NickName => m_NickName;
    }
}