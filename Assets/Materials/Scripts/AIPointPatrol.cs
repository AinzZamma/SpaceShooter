using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPointPatrol : MonoBehaviour
{
    [SerializeField] private float m_Radius;
    public float Radius => m_Radius;

    private static readonly Color GizmosColor = new Color(1, 0, 0, 0.3f);

    private void OnDrawGizmoSelected()
    {
        Gizmos.color = GizmosColor;
        Gizmos.DrawSphere(transform.position, m_Radius);
    }
}
