using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEngine.SceneManagement.Scene;

namespace AngryPigs
{
    [CreateAssetMenu(fileName ="LevelsData", menuName ="Section4/Create Levels Data")]
    public class LevelsData : ScriptableObject
    {
        [SerializeField] private string[] levels;


    public string GetNextLevels()
    {
            Scene currentScene = SceneManager.GetActiveScene();
            for(int i = 0; i < levels.Length-1; i++)
            {
                if (levels[i] == currentScene.name)
                {
                    return levels[i + 1];
                }
            }
            return "";
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    }
}
