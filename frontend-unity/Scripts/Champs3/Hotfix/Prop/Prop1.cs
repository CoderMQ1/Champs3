using System.Collections.Generic;
using QFramework;
using champs3.Hotfix.Effect;
using champs3.Hotfix.Events;
using champs3.Hotfix.Generate;
using champs3.Hotfix.Player;
using UnityEngine;

namespace champs3.Hotfix
{
    public class Prop1 : AbstractProp
    {
        public List<AbstractParticleEffect> Effects = new List<AbstractParticleEffect>();

        public override int LeftUseTimes()
        {
            return Config.UsageTimes - UsedTimes;
        }

        protected override void OnUpdate(float deltaTime)
        {
        }

        protected override void OnUse()
        {
            LogKit.I($"Prop {Config.Name}_{Config.Id} is Using, LeftTimes {Config.UsageTimes - UsedTimes}");

            CharacterAnimatorController animatorController = Affecter.GetComponent<CharacterAnimatorController>();
            animatorController.SetProp(true);
            if (PropObjs != null)
            {
                for (int i = 0; i < PropObjs.Count; i++)
                {
                    PropObjs[i].SetActive(true);
                }
            }


            TypeEventSystem.Global.Send(new PlayerEvents.OnPropUse()
            {
                Config = Config,
                User = User,
                Affecter = Affecter
            });


            DashEffect show = new DashEffect("Prefabs_PortalGlow", Affecter.transform, new Vector3(0, 1f, 0));
            Effects.Add(show);
            // if (Config.Id < 12000)
            // {
            //     var characterParts = Affecter.GetComponentInChildren<CharacterParts>();
            //     LogKit.I($"Add Prop {Config.Id} effect");
            //     // DashEffect effectL = new DashEffect("Prefabs_Sparks", characterParts.FootL);
            //     // DashEffect effectR = new DashEffect("Prefabs_Sparks", characterParts.FootR);
            //     // Effects.Add(effectL);
            //     // Effects.Add(effectR);
            //     DashEffect effect = new DashEffect("Prefabs_Dash", Affecter.transform, new Vector3(0,0.75f, 1.1f));
            //     // DashEffect effect2 = new DashEffect("Prefabs_Windlines", Affecter.transform, new Vector3(0,0.75f, 1.1f), Quaternion.Euler(0,90,0));
            //     DashEffect effect2 = new DashEffect("Prefabs_Windlines", Affecter.transform, new Vector3(0,0.75f, 1.1f), Quaternion.Euler(0,90,0));
            //     Effects.Add(effect);
            //     Effects.Add(effect2);
            // }

            if (Config.Id > 11000 && Config.Id < 12000)
            {
                DashEffect effect = new DashEffect("Prefabs_Dash", Affecter.transform, new Vector3(0, 0.75f, 1.1f));
                DashEffect effect2 = new DashEffect("Prefabs_Windlines", Affecter.transform,
                    new Vector3(0, 0.75f, 1.1f), Quaternion.Euler(0, 90, 0));
                Effects.Add(effect);
                Effects.Add(effect2);
            }
            
            if (Config.Id > 12000 && Config.Id < 13000)
            {
                DashEffect effect = new DashEffect("Prefabs_Dash", Affecter.transform, new Vector3(0, 0.75f, 1.1f));
                Effects.Add(effect);
            }

            if (Config.Id > 13000 && Config.Id < 14000)
            {
                DashEffect effect2 = new DashEffect("Prefabs_Windlines", Affecter.transform,
                    new Vector3(0, 0.75f, 1.1f), Quaternion.Euler(0, 0, -90));
                Effects.Add(effect2);
            }

            for (int i = 0; i < Effects.Count; i++)
            {
                Effects[i].Enable();
            }
        }

        protected override void OnFinish()
        {
            LogKit.I(
                $"Prop {Config.Name}_{Config.Id} is Finished, LeftTimes {Config.UsageTimes - UsedTimes} - time {LeftDistance}");


            if (PropObjs != null)
            {
                for (int i = 0; i < PropObjs.Count; i++)
                {
                    PropObjs[i].SetActive(false);
                }
            }

            CharacterAnimatorController animatorController = Affecter.GetComponent<CharacterAnimatorController>();

            if (animatorController)
            {
                animatorController.SetProp(false);
            }

            SHPlayerController controller = Affecter.GetComponent<SHPlayerController>();
            // controller.PropAffectEnd(this);

            TypeEventSystem.Global.Send(new PlayerEvents.OnPropAffectEnd()
            {
                Config = Config,
                User = User,
                Affecter = Affecter
            });

            // if (playerMoveController.isServerOnly)
            // {
            //     return;
            // }


            for (int i = 0; i < Effects.Count; i++)
            {
                Effects[i].Disable();
                // if (!CanUse())
                // {
                    Effects[i].Destroy();
                // }
            }
            Effects.Clear();
        }

        public Prop1(long propsId) : base(propsId)
        {
        }
    }
}