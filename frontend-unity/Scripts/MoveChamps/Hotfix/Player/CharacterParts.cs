// 
// 2023/12/08

using System;
using System.Collections.Generic;
using UnityEngine;

namespace SquareHero.Hotfix.Player
{
    public class CharacterParts : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        public Transform Head;
        public Transform Chest;
        public Transform HandL;
        public Transform HandR;
        public Transform FootL;
        public Transform FootR;
        [Space]
        public List<Transform> Parts;
        #endregion

        #region FIELDS



        #endregion

        #region PROPERTIES

        #endregion

        #region EVENT FUNCTION

        private void Start()
        {
            if (Parts == null)
            {
                Parts = new List<Transform>();
            }
            
            Parts.Add(Head);
            Parts.Add(Chest);
            Parts.Add(HandL);
            Parts.Add(HandR);
            Parts.Add(FootL);
            Parts.Add(FootR);
        }

        #endregion

        #region METHODS
        
        #endregion


    }

}