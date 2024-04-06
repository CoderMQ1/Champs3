// 
// 2023/12/06

using System.Collections.Generic;
using QFramework;
using SquareHero.Hotfix.Events;
using SquareHero.Hotfix.GameLogic;
using SquareHero.Hotfix.Generate;
using SquareHero.Hotfix.Player;
using UnityEngine;

namespace SquareHero.Hotfix
{
    public static class SkinHelper
    {
        
        // public static List<AbstractProp> LoadProps(GameObject gameObject, int[] propIds)
        // {
        //     var props = ExcelConfig.GetPropConfigs(propIds);
        //     LoadProps(gameObject, props);
        //     return props;
        // }
        
        public static AbstractProp LoadProps(SHPlayerController shPlayerController, int propIds)
        {
            var props = new Prop1(propIds);
            if (shPlayerController.Data.UserId == RoomManager.Instance.LocalPlayerId)
            {
                LogKit.I("Local Player LoadProps");
            }
            if (props.IsValid())
            {
                LoadProps(shPlayerController, props);
            }
            else
            {
                shPlayerController.LoadedProps();
            }
            return props;
        }
        
        public static void LoadProps(SHPlayerController shPlayerController, AbstractProp prop)
        {
            
            if (prop.AssetConfig.AssetType == 1)
            {
                ApplyPropSkin(shPlayerController, prop);
            }
            
            if (prop.AssetConfig.AssetType == 2)
            {
                SuspendPropSkin(shPlayerController, prop);
            }
            
        }
        
        public static void LoadProps(GameObject gameObject, List<AbstractProp> props)
        {
            var transforms = gameObject.GetComponentsInChildren<Transform>();
            for (int i = 0; i < props.Count; i++)
            {
                var prop = props[i];


                if (prop.AssetConfig.AssetType == 1)
                {
                    ApplyPropSkin(gameObject.transform, transforms, prop);
                }
                
                if (prop.AssetConfig.AssetType == 2)
                {
                    SuspendPropSkin(transforms, prop);
                }
            }
        }

        private static void SuspendPropSkin(Transform[] sourceTransforms,AbstractProp prop)
        {
            AssetConfig assetConfig = prop.AssetConfig;
            ResourceManager.Instance.GetAssetAsync<GameObject>(assetConfig.PrefabName, asset =>
            {
                for (int i = 0; i < sourceTransforms.Length; i++)
                {
                    if (sourceTransforms[i].name.Contains(assetConfig.SuspendTransform))
                    {
                        var instantiate = GameObject.Instantiate(asset, sourceTransforms[i]);
                        instantiate.name = instantiate.name.Replace("(Clone)", "");
                        instantiate.SetActive(false);
                        prop.BindGameObject(instantiate);
                    }
                }
            });

        }
        
        private static void SuspendPropSkin(SHPlayerController shPlayerController,AbstractProp prop)
        {
            Transform[] sourceTransforms = shPlayerController.GetComponentsInChildren<Transform>();
            AssetConfig assetConfig = prop.AssetConfig;
            ResourceManager.Instance.GetAssetAsync<GameObject>(assetConfig.PrefabName, asset =>
            {
                for (int i = 0; i < sourceTransforms.Length; i++)
                {
                    if (sourceTransforms[i].name.Contains(assetConfig.SuspendTransform))
                    {
                        var instantiate = GameObject.Instantiate(asset, sourceTransforms[i]);
                        instantiate.name = instantiate.name.Replace("(Clone)", "");
                        instantiate.SetActive(false);
                        prop.BindGameObject(instantiate);
                    }
                }

                shPlayerController.LoadedProps();
            });

        }
        
        private static void ApplyPropSkin(SHPlayerController shPlayerController, AbstractProp prop)
        {
            Transform root = shPlayerController.transform;
            Transform[] sourceTransforms = shPlayerController.GetComponentsInChildren<Transform>();
            AssetConfig assetConfig = prop.AssetConfig;
            ResourceManager.Instance.GetAssetAsync<GameObject>(assetConfig.PrefabName, asset =>
            {
                var instantiate = GameObject.Instantiate(asset);
                var assetSmr = instantiate.transform.GetComponentInChildren<SkinnedMeshRenderer>();
                if (assetSmr)
                {
                    GameObject gameObject = new GameObject(assetSmr.gameObject.name);
                    gameObject.transform.parent = root;
                    gameObject.transform.localPosition = assetSmr.transform.localPosition;
                    gameObject.transform.localRotation = assetSmr.transform.localRotation;
                    var smr = gameObject.AddComponent<SkinnedMeshRenderer>();
                    var smrBones = GetBones(sourceTransforms, assetSmr.bones);
                    smr.sharedMesh = assetSmr.sharedMesh;
                    smr.materials = assetSmr.materials;
                    smr.bones = smrBones;
                    Transform rootBone = null;
                    for (int i = 0; i < sourceTransforms.Length; i++)
                    {
                        if (sourceTransforms[i].name == assetSmr.rootBone.name)
                        {
                            rootBone = sourceTransforms[i];
                            break;
                        }
                    }

                    if (!rootBone)
                    {
                            LogKit.E($"{root.name} has not bone {assetSmr.rootBone.name}");
                    }
                    smr.rootBone = rootBone;
                    smr.gameObject.SetActive(false);
                    prop.BindGameObject(gameObject);
                }
                instantiate.gameObject.SetActive(false);
                shPlayerController.LoadedProps();
            });
            
            
        }
        
        
        private static void ApplyPropSkin(Transform root, Transform[] sourceTransforms, AbstractProp prop)
        {
            AssetConfig assetConfig = prop.AssetConfig;
            ResourceManager.Instance.GetAssetAsync<GameObject>(assetConfig.PrefabName, asset =>
            {
                var instantiate = GameObject.Instantiate(asset);
                var assetSmr = instantiate.transform.GetComponentInChildren<SkinnedMeshRenderer>();
                if (assetSmr)
                {
                    GameObject gameObject = new GameObject(assetSmr.gameObject.name);
                    gameObject.transform.parent = root;
                    gameObject.transform.localPosition = assetSmr.transform.localPosition;
                    gameObject.transform.localRotation = assetSmr.transform.localRotation;
                    var smr = gameObject.AddComponent<SkinnedMeshRenderer>();
                    var smrBones = GetBones(sourceTransforms, assetSmr.bones);
                    smr.sharedMesh = assetSmr.sharedMesh;
                    smr.materials = assetSmr.materials;
                    smr.bones = smrBones;
                    Transform rootBone = null;
                    for (int i = 0; i < sourceTransforms.Length; i++)
                    {
                        if (sourceTransforms[i].name == assetSmr.rootBone.name)
                        {
                            rootBone = sourceTransforms[i];
                            break;
                        }
                    }

                    if (!rootBone)
                    {
                        LogKit.E($"{root.name} has not bone {assetSmr.rootBone.name}");
                    }
                    smr.rootBone = rootBone;
                    smr.gameObject.SetActive(false);
                    prop.BindGameObject(gameObject);
                }
            });
            
            
        }
        
        private static Transform[] GetBones(Transform[] sourceTransforms, Transform[] bones)
        {
            List<Transform> searched = new List<Transform>();
            for (int i = 0; i < bones.Length; i++)
            {
                for (int j = 0; j < sourceTransforms.Length; j++)
                {
                    if (bones[i].name == sourceTransforms[j].name)
                    {
                        searched.Add(sourceTransforms[j]);
                        
                    }
                }
            }

            LogKit.I($"search bone count {searched.Count}");
            return searched.ToArray();
        }




    }
}