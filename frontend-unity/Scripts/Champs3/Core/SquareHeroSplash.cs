using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.Video;

namespace champs3.Core
{
    public class champs3Splash : MonoBehaviour
    {
        public Canvas canvas;
        public Animation animation;
        public VideoPlayer VideoPlayer;
        [Range(1, 10)]
        public float duration = 2f;
        void Start()
        {
            // animation.Play("SplashIn");
            // StartCoroutine(ExitSplash());


            ActionKit.Delay(5, () =>
            {
                VideoPlayer.Pause();
                GameStart.Instance.CompletSplash();
            }).Start(this);
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        // IEnumerator ExitSplash()
        // {
            // yield return new WaitForSeconds(duration);
            // animation.Play("SplashOut");
            // GameStart.Instance.CompletSplash();
        // }
    }
 
}
