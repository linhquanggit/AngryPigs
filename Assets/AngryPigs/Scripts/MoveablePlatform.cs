using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AngryPigs

{

    public class MoveablePlatform : MonoBehaviour
    {

        [SerializeField] private Transform[] m_WayPoints;
        [SerializeField] private Transform m_OneWayPlatform;
        [SerializeField] private float m_MoveSpeed;

        private int m_CurrentWayPointIndex;
        private void Start()
        {
            m_OneWayPlatform.position = m_WayPoints[0].position;
        }

        private void Update()
        {
            m_OneWayPlatform.position = Vector3.MoveTowards(m_OneWayPlatform.position, m_WayPoints[m_CurrentWayPointIndex].position, m_MoveSpeed * Time.deltaTime);
            if(m_OneWayPlatform.position == m_WayPoints[m_CurrentWayPointIndex].position)
            {
                m_CurrentWayPointIndex++;
                if(m_CurrentWayPointIndex >= m_WayPoints.Length)
                {
                    m_CurrentWayPointIndex = 0;
                }
            }
        }
        private void OnDrawGizmos()
        {
            if (m_WayPoints.Length <= 1)
                return;
            for(int i=0; i < m_WayPoints.Length-1; i++)
            {
                Gizmos.DrawLine(m_WayPoints[i].position, m_WayPoints[i + 1].position);
            }
            Gizmos.DrawLine(m_WayPoints[0].position, m_WayPoints[m_WayPoints.Length - 1].position);
        }
    }
}
