using System.Collections.Generic;
using QFramework;

namespace champs3.Hotfix.Map
{

    public interface IMapModel : IModel
    {
        
    }



    public class MapModel : AbstractModel, IMapModel
    {
        public List<MoveNode> MoveNodes;
        protected override void OnInit()
        {
            MoveNodes = new List<MoveNode>();
        }
    }
    
}