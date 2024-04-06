using SquareHero.Hotfix.Map;

namespace SquareHero.Hotfix.Events
{
    public class MapEvents
    {
        
        public struct OnServerFinishCreateMap
        {
            public int RelPlayerNum;
        }
        
        
        public struct OnClientCreateMap
        {
            
        }

        public struct OnClientFinishCreateMap
        {
            public long UserId;
        }
        
        public struct RegisteMoveNode
        {
            public MoveNode MoveNode;
        }
    }
}