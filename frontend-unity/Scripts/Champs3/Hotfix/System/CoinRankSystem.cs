using System.Linq;
using QFramework;
using champs3.Hotfix.Events;
using champs3.Hotfix.Model;
using NotImplementedException = System.NotImplementedException;

namespace champs3.Hotfix.System
{

    public interface ICoinRankSystem : ISystem
    {
    }

    public class CoinRankSystem : AbstractSystem , ICoinRankSystem
    {
        protected override void OnInit()
        {
            TypeEventSystem.Global.Register<PlayerEvents.GetCoin>(OnGetCoin);
        }


        protected void OnGetCoin(PlayerEvents.GetCoin evt)
        {
            var playerModel = this.GetModel<PlayerModel>();

            if (playerModel.PlayerModes.TryGetValue(evt.UserId,out PlayerModel.Player player))
            {
                player.Coins.Value += evt.Coin;
                
                playerModel.CoinRank.Clear();
                
                var players = playerModel.PlayerModes.Values.ToList();
                
                players.Sort((x,y) =>
                {
                    return y.Coins.Value.CompareTo(x.Coins.Value);
                });

                
                for (int i = 0; i < players.Count; i++)
                {
                    playerModel.CoinRank.Add(players[i].UserId);
                }
                
                TypeEventSystem.Global.Send(new GameEvents.OnCoinRankChange());
            }
            
        }
    }
}