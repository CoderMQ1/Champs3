// 
// 2023/12/12


using System;
using QFramework;

namespace SquareHero.Hotfix.Events
{
    public class UIEvents
    {
        public struct HidePanel<T>
        {
            public T Type;
        }
        public const int UIMgrID = 3000;
        public enum ID
        {
            RefreshPlayerAssets = UIMgrID,
            RefreshSelectRoleAttributes
        }



        public class RefreshPlayerAssets : QMsg
        {
            public RefreshPlayerAssets() : base((int)ID.RefreshPlayerAssets)
            {
            }

            public PlayerassetsResponse Response;

        }
    }


}