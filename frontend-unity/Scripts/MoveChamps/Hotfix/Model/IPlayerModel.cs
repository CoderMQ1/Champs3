using System.Collections.Generic;
using QFramework;

namespace SquareHero.Hotfix.Model
{
    public interface IPlayerModel : IModel
    {
        
    }



    public class PlayerModel : AbstractModel , IPlayerModel
    {

        public Dictionary<long, Player> PlayerModes = new Dictionary<long, Player>();

        public List<long> CoinRank = new List<long>();

        protected override void OnInit()
        {
            
        }

        public class Player
        {
            public long UserId;
            public string UserName;
            public int SkinIndex;
            public int PropId;
            public BindableProperty<int> Coins = new BindableProperty<int>();
        }
    }
}