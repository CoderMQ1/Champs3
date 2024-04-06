// 
// 2023/12/15

using System.Collections.Generic;
using QFramework;
using SquareHero.Hotfix.Generate;

namespace SquareHero.Hotfix.Model
{
    public interface IMainModel : IModel
    {
    }


    public class MainModel : AbstractModel, IMainModel
    {
        public Player Player;
        public List<RoleData> RoleDatas;
        public List<PropsData> AllPropDatas { get; protected set; }
        public List<PropsData> PlayerPropDatas{ get; protected set; }
        public PlayerBag Bag;


        

        public BagRefresh BagRefreshHandler;
        public delegate void BagRefresh(PlayerBag bag);
        protected override void OnInit()
        {
            Player = new Player();
            Bag = new PlayerBag();

            TypeEventSystem.Global.Register<SystemEvents.UpdatePlayerProps>(UpdatePropDatas);
            TypeEventSystem.Global.Register<SystemEvents.UpdatePlayerAssets>(UpdatePlayerAsset);
            Web3.Instance.UpdateBalanceHandler += UpdateBalanceData;
            
        }

        
        public void UpdateBalanceData(float balance)
        {
            // Bag.Coin = balance;
            
            // BagRefreshHandler?.Invoke(Bag);
            Bag.UpdateAssetNumber(ConstValue.TokenId, balance);
        }

        public void UpdatePropDatas(SystemEvents.UpdatePlayerProps evt)
        {

            if (AllPropDatas != null)
            {
                AllPropDatas.Clear();
                AllPropDatas = null;
            }

            AllPropDatas = evt.PropDatas;

            if (PlayerPropDatas != null)
            {
                PlayerPropDatas.Clear();
            }
            else
            {
                PlayerPropDatas = new List<PropsData>();
            }

            for (int i = 0; i < AllPropDatas.Count; i++)
            {
                if (AllPropDatas[i].amount > 0)
                {
                    PlayerPropDatas.Add(AllPropDatas[i]);
                }
            }
        }

        public void UpdatePlayerAsset(SystemEvents.UpdatePlayerAssets evt)
        {
            var response = evt.Response;
            var itemConfigTable = ExcelConfig.ItemConfigTable;
            for (int i = 0; i < response.data.Count; i++)
            {
                var playerAsset = response.data[i];


                var itemConfig = itemConfigTable.Data.Find(config =>
                {
                    return config.AssetName == playerAsset.asset_name;
                });

                if (itemConfig == null)
                {
                    LogKit.E($"Not Find Asset [{playerAsset.asset_name}]");
                    continue;
                }

                if (Bag.PlayerAssetsMap.TryGetValue(itemConfig.Id, out PlayerAssets asset))
                {
                    asset.Number = playerAsset.available_amount;
                }
                else
                {
                    Bag.PlayerAssetsMap.Add(itemConfig.Id, new PlayerAssets(){Id = itemConfig.Id, ItemConfig = itemConfig, Number = playerAsset.available_amount});
                    if (itemConfig.Id > 11000 && itemConfig.Id < 15000)
                    {
                        Bag.PlayerGameProps.Add(itemConfig.Id, Bag.PlayerAssetsMap[itemConfig.Id]);
                    }
                    
                    if (itemConfig.Id == 10010)
                    {
                        Bag.PlayerOtherProps.Add(itemConfig.Id, Bag.PlayerAssetsMap[itemConfig.Id]);
                    }
                }

                // if (playerAsset.asset_name.Equals(ConstValue.CoinAssetsName))
                // {
                //     Bag.Coin = playerAsset.available_amount;
                // }
                // if (playerAsset.asset_name.Equals(ConstValue.DiamondAssetsName))
                // {
                //     Bag.Diamond = playerAsset.available_amount;
                // }
                
                // if (playerAsset.asset_name.Equals(ConstValue.SpinChip))
                // {
                //     Bag.SpinChip = (int)playerAsset.available_amount;
                //     
                //     
                //     
                // }
                //
                // if (playerAsset.asset_name.Equals(ConstValue.SpinKey))
                // {
                //     Bag.LotteryKey = (int)playerAsset.available_amount;
                // }
            }
            
            // BagRefreshHandler?.Invoke(Bag);
        }

        public void Dispose()
        {
            TypeEventSystem.Global.UnRegister<SystemEvents.UpdatePlayerProps>(UpdatePropDatas);
            TypeEventSystem.Global.UnRegister<SystemEvents.UpdatePlayerAssets>(UpdatePlayerAsset);
            // Web3.Instance.UpdateBalanceHandler -= UpdateBalanceData;
        }

    }
    

    public class Player
    {
        public long UserId;
        public string PlayerName;
        public string PlayerCode;
        public long SkinId;
        public int SkinIndex;
        
        
    }

    public class PlayerBag
    {
        public float Coin {
            get
            {
                return GetAssetNumber(ConstValue.TokenId) / TokenHelper.TokenAccuracy;
            }
        }
        public float Diamond;

        public int SpinChip
        {
            get { return (int)GetAssetNumber(ConstValue.SpinChipId); }
        }

        public int LotteryKey        {
            get { return (int)GetAssetNumber(ConstValue.SpinKeyId); }
        }
        public int SeasonPoint;
        
        public Dictionary<int, PlayerAssets> PlayerAssetsMap = new Dictionary<int, PlayerAssets>();
        
        public  Dictionary<int, PlayerAssets> PlayerGameProps = new Dictionary<int, PlayerAssets>();
        public  Dictionary<int, PlayerAssets> PlayerOtherProps = new Dictionary<int, PlayerAssets>();
        
        public float GetAssetNumber(int id)
        {
            if (PlayerAssetsMap.TryGetValue(id, out PlayerAssets assets))
            {
                return assets.Number;
            }

            return 0;
        }

        public void UpdateAssetNumber(int id ,float value)
        {
            if (PlayerAssetsMap.TryGetValue(id, out PlayerAssets assets))
            {
                assets.Number = value;
            }
            else
            {
                var itemConfig = ExcelConfig.ItemConfigTable.Data.Find(config =>
                {
                    return config.Id == id;
                });
                PlayerAssetsMap.Add(id, new PlayerAssets(){Id = id, Number = value, ItemConfig = itemConfig});
            }
        }
        
        public void AddAssetNumber(int id ,float addtive)
        {
            if (PlayerAssetsMap.TryGetValue(id, out PlayerAssets assets))
            {
                assets.Number += addtive;
            }
            else
            {
                
                var itemConfig = ExcelConfig.ItemConfigTable.Data.Find(config =>
                {
                    return config.Id == id;
                });
                PlayerAssetsMap.Add(id, new PlayerAssets(){Id = id, Number = addtive, ItemConfig = itemConfig});
                LogKit.E($"Not Assets Model [{id}]");
            }
        }
        
    }

    public enum AttributesType
    {
        RunSpeed = 1,
        SwimSpeed,
        ClimbSpeed,
        FlySpeed
    }

    public static class RoleDataHelper
    {
        public static int GetRunAttributeValue(this RoleData slef)
        {
            var roleAttribute = slef.Attributes.Find(attribute =>
            {
                return attribute.AttriType == (int)AttributesType.RunSpeed;
            });

            return roleAttribute.AttriValue + roleAttribute.TalentValue;
        }
        
        public static int GetTalentValue(this RoleData slef, AttributesType type)
        {
            var roleAttribute = slef.Attributes.Find(attribute =>
            {
                return attribute.AttriType == (int)type;
            });

            return roleAttribute.TalentValue;
        }
        
        public static int GetAttributeValue(this RoleData slef, AttributesType type)
        {
            var roleAttribute = slef.Attributes.Find(attribute =>
            {
                return attribute.AttriType == (int)type;
            });

            return roleAttribute.AttriValue;
        }
        
        public static float GetRunSpeed(this RoleData slef)
        {
            var roleAttribute = slef.Attributes.Find(attribute =>
            {
                return attribute.AttriType == (int)AttributesType.RunSpeed;
            });

            return roleAttribute.Speed;
        }
        
        public static int GetSwimAttributeValue(this RoleData slef)
        {
            var roleAttribute = slef.Attributes.Find(attribute =>
            {
                return attribute.AttriType == (int)AttributesType.SwimSpeed;
            });

            return roleAttribute.AttriValue + roleAttribute.TalentValue;
            
        }
        
        public static float GetSwimSpeed(this RoleData slef)
        {
            var roleAttribute = slef.Attributes.Find(attribute =>
            {
                return attribute.AttriType == (int)AttributesType.SwimSpeed;
            });

            return roleAttribute.Speed;
        }
        
        public static int GetClimbAttributeValue(this RoleData slef)
        {
            var roleAttribute = slef.Attributes.Find(attribute =>
            {
                return attribute.AttriType == (int)AttributesType.ClimbSpeed;
            });

            return roleAttribute.AttriValue + roleAttribute.TalentValue;
        }
        
        public static float GetClimbSpeed(this RoleData slef)
        {
            var roleAttribute = slef.Attributes.Find(attribute =>
            {
                return attribute.AttriType == (int)AttributesType.ClimbSpeed;
            });

            return roleAttribute.Speed;
        }
        
        public static int GetFlyAttributeValue(this RoleData slef)
        {
            var roleAttribute = slef.Attributes.Find(attribute =>
            {
                return attribute.AttriType == (int)AttributesType.FlySpeed;
            });

            return roleAttribute.AttriValue + roleAttribute.TalentValue;
        }
        
        public static float GetFlySpeed(this RoleData slef)
        {
            var roleAttribute = slef.Attributes.Find(attribute =>
            {
                return attribute.AttriType == (int)AttributesType.FlySpeed;
            });

            return roleAttribute.Speed;
        }
    }

    public class PlayerAssets
    {
        public int Id;
        public ItemConfig ItemConfig;
        public float Number;
    }
}