﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

namespace AngryPigs
{
    public class HomePanel : MonoBehaviour
    {
        public void BtnPlay_Pressed()
        {
            SceneManager.LoadScene("Level_1");
        }
    }
}