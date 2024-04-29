namespace champs3.Hotfix.Model
{
    public struct GiftModel
    {
        public int Track { get; set; }

        public bool NeedCreate;
        
        public RewardType RewardType;
        
        public int CoinCount;

        public int DiamondCount;
    }

    public enum RewardType
    {
        Lv1 = 1,
        Lv2,
        Lv3
    }
}