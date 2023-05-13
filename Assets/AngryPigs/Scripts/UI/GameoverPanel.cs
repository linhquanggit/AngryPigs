using System.Collections;
using System.Collections.Generic;
using UnityEngine;  
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace AngryPigs
{
    public class GameoverPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_TxtResult;
        [SerializeField] private string m_TxtResultWin;

        public void DisplayResult(bool isWin)
        {
            if (isWin)
                m_TxtResult.text = m_TxtResultWin;
            else
                m_TxtResult.text = "YOU LOSE";
        }

        public void BtnNextLevel_Pressed()
        {
            GamePlayManager.Instance.NextLevel();
        }

        public void BtnRestart_Pressed()
        {
            GamePlayManager.Instance.Restart();
        }
       
    }
}