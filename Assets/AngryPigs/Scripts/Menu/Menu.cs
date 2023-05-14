using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AngryPigs
{
    public class Menu : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        public void Play()
        {
            SceneManager.LoadScene(0);
        }   
        public void Exit()
        {
            if (UnityEditor.EditorApplication.isPlaying)
            {
                UnityEditor.EditorApplication.ExitPlaymode();
            }
             Application.Quit();

        }
        public void Tutorial()
        {
            SceneManager.LoadScene(6);
        } 
        public void BackToMenu()
        {
            SceneManager.LoadScene(5);
        }    
    }
}
