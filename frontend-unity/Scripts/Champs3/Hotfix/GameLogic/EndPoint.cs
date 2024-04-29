// 
// 2023/12/06

using System;
using System.Collections.Generic;
using Cinemachine;
using QFramework;
using champs3.Core;
using champs3.Hotfix.Events;
using UnityEngine;

namespace champs3.Hotfix.GameLogic
{
    public class EndPoint : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        public CinemachineVirtualCamera VirtualCamera;
        #endregion

        #region FIELDS

        public List<GameObject> ArrivedPlayer = new List<GameObject>();
        
        #endregion

        #region PROPERTIES

        #endregion

        #region EVENT FUNCTION

        private void Start()
        {
            TypeEventSystem.Global.Register<GameEvents.OnClientGameOver>(OnGameOver);
        }

        private void OnDestroy()
        {
            TypeEventSystem.Global.UnRegister<GameEvents.OnClientGameOver>(OnGameOver);
        }

        private void OnTriggerEnter(Collider other)
        {
            LogKit.I($"Player Arroved {other.name}");
            if (other.CompareTag("Player"))
            {
                if (!VirtualCamera.gameObject.activeSelf)
                {
                    VirtualCamera.gameObject.SetActive(true);
                    
                    if (GameStart.isCreated && GameStart.Instance.IsMobilePlatform())
                    {
                        transform.localPosition = new Vector3(0, 3.7f, 6.38f);
                    }
                }
                
                ArrivedPlayer.Add(other.gameObject);
            }
        }

        #endregion

        #region METHODS

        private void OnGameOver(GameEvents.OnClientGameOver evt)
        {
            ParticleSystem[] Particle = GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < Particle.Length; i++)
            {
                Particle[i].Play();
            }
            
            
        }
        
        #endregion


    }

}