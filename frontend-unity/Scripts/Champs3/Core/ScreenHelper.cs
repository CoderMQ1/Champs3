using System;
using UnityEngine;

namespace champs3.Core
{
    public class ScreenHelper : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        #endregion

        #region FIELDS

        #endregion

        #region PROPERTIES

        #endregion

        #region EVENT FUNCTION

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        #endregion

        #region METHODS


        public void LandscapeLeft()
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
        
        public void LandscapeRight()
        {
            Screen.orientation = ScreenOrientation.LandscapeRight;
        }

        public void SetResolution(string resolution)
        {
            var split = resolution.Split("*");
            int width = Mathf.FloorToInt(float.Parse(split[0]));
            int height = Mathf.FloorToInt(float.Parse(split[1]));
            Screen.SetResolution(width, height, false);
        }

        #endregion

        
    }

}