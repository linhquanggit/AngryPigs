using Section4;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AngryPigs
{
    public class AudioTheme : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
            AudioManager.Instance.PlaySFX_MainMusicSFXClip();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
}
