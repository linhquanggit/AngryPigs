﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AngryPigs
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager m_Instance;
        public static AudioManager Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = FindObjectOfType<AudioManager>();
                return m_Instance;
            }
        }

        [SerializeField] private AudioSource m_SFX;

        [SerializeField] private AudioClip m_MeleeSplashSFXClip;
        [SerializeField] private AudioClip m_PlayerDeadSFXClip;
        [SerializeField] private AudioClip m_PlayerGetHitSFXClip;
        [SerializeField] private AudioClip m_EnemyGetHitSFXClip;
        [SerializeField] private AudioClip m_LevelCompletedSFXClip;
        [SerializeField] private AudioClip m_MainMusicSFXClip;

        private void Awake()
        {
            if (m_Instance == null)
                m_Instance = this;
            else if (m_Instance != this)
                Destroy(gameObject);
        }

        public void PlaySFX_PlayerGetHit()
        {
            m_SFX.pitch = Random.Range(1f, 2f);
            m_SFX.PlayOneShot(m_PlayerGetHitSFXClip);
        }

        public void PlaySFX_PlayerDead()
        {
            m_SFX.pitch = Random.Range(1f, 2f);
            m_SFX.PlayOneShot(m_PlayerDeadSFXClip);
        }

        public void PlaySFX_EnemyGetHit()
        {
            m_SFX.pitch = Random.Range(1f, 2f);
            m_SFX.PlayOneShot(m_EnemyGetHitSFXClip);
        }

        public void PlaySFX_MeleeSplash()
        {
            m_SFX.pitch = Random.Range(1f, 2f);
            m_SFX.PlayOneShot(m_MeleeSplashSFXClip);
        }
        public void PlaySFX_m_LevelCompleted()
        {
            m_SFX.pitch = Random.Range(1f, 2f);
            m_SFX.PlayOneShot(m_LevelCompletedSFXClip);
        }
        public void PlaySFX_MainMusicSFXClip()
        {
            /*m_SFX.pitch = Random.Range(1f, 2f);
            m_SFX.PlayOneShot(m_MainMusicSFXClip);*/
            m_SFX.clip = m_MainMusicSFXClip;
            m_SFX.loop = true;
            m_SFX.pitch = Random.Range(1f, 2f);
            m_SFX.volume = 0.5f;
            m_SFX.Play();
        }
    }
}