using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    [RequireComponent(typeof(SpaceShip))]
    public class AIController : MonoBehaviour
    {
        public enum AIBehaviour
        {
            Null,
            Patrol
        }
        [SerializeField] private AIBehaviour m_AIBehaviour;

        [SerializeField] private AIPointPatrol m_PatrolPoint;

        [Range(0.0f, 1.0f)]
        [SerializeField] private float m_NavigationLinear;

        [SerializeField] private float m_NavigationAngular;

        [SerializeField] private float m_RandomSelectMovePointTime;

        [SerializeField] private float m_FindNewTargetTime;

        [SerializeField] private float m_ShootDelay;

        [SerializeField] private float m_EvadeRayLenght;

        [SerializeField]
        private Transform[] m_PatrolPoints; 

        private int m_CurrentPatrolIndex = 0; 


        private SpaceShip m_SpaceShip;

        private Vector3 m_MovePosition;

        private Destructible m_SelectTarget;

        private Timer m_RandomizeDirectionTimer;
        private Timer m_FireTimer;
        private Timer m_FindNewTargetTimer;

        private void Start()
        {
            m_SpaceShip = GetComponent<SpaceShip>();
            InitTimers();
        }

        private void Update()
        {
            UpdateTimers();
            UpdateAI();
        }
        
        private void UpdateAI()
        {
          
            if (m_AIBehaviour == AIBehaviour.Patrol)
            {
                UpdateBehaviourPatrol();
            }
        }
        private void UpdateBehaviourPatrol()
        {
            ActionFindNewMovePosition();
            ActionControlShip();
            ActionFindNewAttackTarget();
            ActionFire();
            ActionEvadeCollision();
        }
        private void ActionFindNewMovePosition()
        {
            if (m_AIBehaviour == AIBehaviour.Patrol)
            {
                if(m_SelectTarget != null)
                {
                    m_MovePosition = m_SelectTarget.transform.position;
                }
                else
                {
                    if (m_PatrolPoints != null && m_PatrolPoints.Length > 0)
                    {
                        // Проверяем, достиг ли бот текущей точки
                        float distanceToPoint = Vector3.Distance(transform.position, m_PatrolPoints[m_CurrentPatrolIndex].position);
                        if (distanceToPoint < 1.0f) // Если расстояние меньше 1 единицы
                        {
                            // Переход к следующей точке
                            m_CurrentPatrolIndex = (m_CurrentPatrolIndex + 1) % m_PatrolPoints.Length;
                        }

                        // Устанавливаем текущую точку как цель
                        m_MovePosition = m_PatrolPoints[m_CurrentPatrolIndex].position;
                    }
                    else if (m_PatrolPoint != null) // Если используется старый режим патрулирования
                    {
                        bool isInsidePatrolZone = (m_PatrolPoint.transform.position - transform.position).sqrMagnitude <
                                                  m_PatrolPoint.Radius * m_PatrolPoint.Radius;

                        if (isInsidePatrolZone)
                        {
                            if (m_RandomizeDirectionTimer.IsFinished)
                            {
                                Vector2 newPoint = UnityEngine.Random.onUnitSphere * m_PatrolPoint.Radius + m_PatrolPoint.transform.position;
                                m_MovePosition = newPoint;
                                m_RandomizeDirectionTimer.Start(m_RandomSelectMovePointTime);
                            }
                        }
                        else
                        {
                            m_MovePosition = m_PatrolPoint.transform.position;
                        }
                    }
                }
            }


        }

        private void ActionEvadeCollision()
        {
            if (Physics2D.Raycast(transform.position, transform.up, m_EvadeRayLenght) == true)
            {
                m_MovePosition = transform.position + transform.right * 100.0f;
            }
        }

        private void ActionControlShip()
        {
           m_SpaceShip.ThrustControl = m_NavigationLinear;
           m_SpaceShip.TorqueControl = ComputeAlliginTorqueNormalized(m_MovePosition, m_SpaceShip.transform)*m_NavigationAngular;
        }
        private const float MAX_ANGLE = 45.0f;
        private static float ComputeAlliginTorqueNormalized(Vector3 targetPosition, Transform ship)
        {
            Vector2 localTargetPosition = ship.InverseTransformPoint(targetPosition);

            float angle = Vector3.SignedAngle(localTargetPosition, Vector3.up, Vector3.forward);

            angle = Mathf.Clamp(angle, -MAX_ANGLE, MAX_ANGLE) / MAX_ANGLE;
            return -angle;
        }


        private void ActionFindNewAttackTarget()
        {
  
            if (m_FindNewTargetTimer.IsFinished == true)
            {
                m_SelectTarget = FindNearesDestructibleTarget();
                m_FindNewTargetTimer.Start(m_ShootDelay);
            }
        }
        private void ActionFire()
        {
                    if (m_SelectTarget != null)
                    {
                        if (m_FireTimer.IsFinished)
                        {
                            // Получаем позицию и скорость цели
                            Vector3 targetPosition = m_SelectTarget.transform.position;
                            Rigidbody2D targetRigidbody = m_SelectTarget.GetComponent<Rigidbody2D>();
                            Vector3 targetVelocity = targetRigidbody != null ? targetRigidbody.velocity : Vector3.zero;

                            // Рассчитываем точку упреждения
                            Vector3 leadPosition = MakeLead(targetPosition, targetVelocity, transform.position, m_SpaceShip.ProjectileSpeed);

                            // Поворачиваем турели к точке упреждения
                            foreach (var turret in m_SpaceShip.Turrets)
                            {
                                turret.transform.up = (leadPosition - turret.transform.position).normalized;
                            }

                            // Открываем огонь
                            m_SpaceShip.Fire(TurretMode.Primary);
                            m_FireTimer.Start(m_ShootDelay);
                        }
                    }
        }
        private Vector3 MakeLead(Vector3 targetPosition, Vector3 targetVelocity, Vector3 shooterPosition, float projectileSpeed)
        {
            Vector3 toTarget = targetPosition - shooterPosition;
            float distance = toTarget.magnitude;
            float timeToTarget = distance / projectileSpeed;

            Vector3 leadPosition = targetPosition + targetVelocity * timeToTarget;
            return leadPosition;
        }

        private Destructible FindNearesDestructibleTarget()
        {
            float maxDist = float.MaxValue;
            Destructible potentialTarget = null;

            foreach (var v in Destructible.AllDestructibles)
            {
                if (v.GetComponent<SpaceShip>() == m_SpaceShip) continue;
                if (v.TeamId == Destructible.TeamIdNeutral) continue;
                if (v.TeamId == m_SpaceShip.TeamId) continue;

                float dist = Vector2.Distance(m_SpaceShip.transform.position, v.transform.position);
                if (dist < maxDist)
                {
                    maxDist = dist;
                    potentialTarget = v;

                }
            }
            return potentialTarget;
        }
        #region Timers
        private void InitTimers()
        {
         m_RandomizeDirectionTimer = new Timer(m_RandomSelectMovePointTime);
         m_FireTimer = new Timer(m_ShootDelay);
         m_FindNewTargetTimer = new Timer(m_FindNewTargetTime);
    }
        private void UpdateTimers()
        {
            m_RandomizeDirectionTimer.RemoveTime(Time.deltaTime);

            m_FireTimer.RemoveTime(Time.deltaTime);

            m_FindNewTargetTimer.RemoveTime(Time.deltaTime);
        }

        public void SetPatrolBehaviour(AIPointPatrol point)
        {
            m_AIBehaviour = AIBehaviour.Patrol;
            m_PatrolPoint = point;
        }

        #endregion
    }
}