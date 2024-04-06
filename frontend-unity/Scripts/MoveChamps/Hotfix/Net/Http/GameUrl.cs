// 
// 2023/12/15

namespace SquareHero.Hotfix
{
    public class GameUrl
    {
        public string Location;

        public GameUrlType Type;

        public GameUrl(string location, GameUrlType type)
        {
            Location = location;
            Type = type;
        }

        public string Url()
        {
            switch (Type)
            {
                case GameUrlType.Guest :

                    return GameUrlConstValue.GuestAddress + Location;
                case GameUrlType.Asset:
                    return GameUrlConstValue.AssetAddress + Location;
                    break;
            }

            return Location;
        }
    }


    public enum GameUrlType
    {
        Customize,
        Guest,
        Asset
    }


}