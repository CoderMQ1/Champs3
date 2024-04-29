using System;
using System.Collections.Generic;
using QFramework;
using champs3.Hotfix.Effect;
using UnityEngine;

namespace champs3.Hotfix.Map
{
    public class Water : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS
    
        #endregion

        #region FIELDS

        public List<AbstractParticleEffect> Effects = new List<AbstractParticleEffect>();
        
        #endregion

        #region PROPERTIES

        #endregion

        #region EVENT FUNCTION
        
        private void Start()
        {
            // if (NetworkManager.singleton.mode == NetworkManagerMode.ServerOnly)
            // {
            //     GetComponent<Collider>().enabled = false;
            // }
        }

        private void OnTriggerEnter(Collider other)
        {
            
            this.Log($"OnTriggerEnter {other.name}");
            if (other.CompareTag("Player"))
            {
                RipplesEffect effect = new RipplesEffect("Prefabs_Ripples", transform);
                
                effect.BindTran(other.transform);
                effect.Enable();
                Effects.Add(effect);
            }
        }


        private void OnTriggerExit(Collider other)
        {
            this.Log($"OnTriggerExit {other.name}");
            if (other.CompareTag("Player"))
            {
                RipplesEffect effect = null;
                for (int i = 0; i < Effects.Count; i++)
                {
                    if (Effects[i] is RipplesEffect ripplesEffect)
                    {
                        if (ripplesEffect.Binder == other.transform)
                        {
                            effect = ripplesEffect;
                            break;
                        }
                    }
                }
                
                if (effect != null)
                {
                    LogKit.I($"Player [{other.name}] Leave water");
                    Effects.Remove(effect);
                    effect.Destroy();
                }
            }
        }

        private void Update()
        {
            for (int i = 0; i < Effects.Count; i++)
            {
                Effects[i].Update(Time.deltaTime);
            }
        }

        #endregion

        #region METHODS

        #endregion

        
    }
}