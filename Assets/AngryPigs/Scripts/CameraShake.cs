using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AngryPigs
{

    public class CameraShake : MonoBehaviour
    {
        public static CameraShake m_Instance;
        public static CameraShake Instance
        {
            get
            {
                if(m_Instance==null)
                {
                    m_Instance = FindObjectOfType<CameraShake>();
                }
                return m_Instance;
            }
        }

        [SerializeField] private CinemachineVirtualCamera[] m_VirtualCameras;
        [SerializeField] private float m_ShakeAmlitude = 1.2f;
        [SerializeField] private float m_ShakeFrequency = 2.0f;

        private float m_ShakeDuration;

        //arr noise's virtualCamera
        private CinemachineBasicMultiChannelPerlin[] m_VirtualCameraNoise;


        private void Awake()
        {
            if (m_Instance == null)
                m_Instance = this;
            else if(m_Instance!=this)
                Destroy(gameObject);
        }
        // Start is called before the first frame update
        void Start()
        {
            m_VirtualCameraNoise = new CinemachineBasicMultiChannelPerlin[m_VirtualCameras.Length];
            for(int i = 0; i < m_VirtualCameras.Length; i++)
            {
                m_VirtualCameraNoise[i] = m_VirtualCameras[i].GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            }
        }
        public void Shake(float pDuration)
        {
            m_ShakeDuration = pDuration;
        }

        // Update is called once per frame
        void Update()
        {
            if(m_ShakeDuration > 0)
            {
                m_ShakeDuration -= Time.deltaTime;
                for(int i=0; i<m_VirtualCameraNoise.Length; i++)
                {
                    //set cinemachine Camera Noise  parameters
                    m_VirtualCameraNoise[i].m_AmplitudeGain = m_ShakeAmlitude;
                    m_VirtualCameraNoise[i].m_FrequencyGain = m_ShakeFrequency;
                }
                if(m_ShakeDuration <= 0)
                {
                    for(int i = 0; i< m_VirtualCameraNoise.Length; i++)
                    {
                        //set cinemachine Camera Noise  parameters
                        m_VirtualCameraNoise[i].m_AmplitudeGain = 0;
                        m_VirtualCameraNoise[i].m_FrequencyGain = 0;
                    }
                }
            }
        }
    }
}
