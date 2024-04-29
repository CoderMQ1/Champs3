// 
// 2023/12/15

using QFramework;
using champs3.Hotfix.Model;
using UnityEngine;

namespace champs3.Hotfix.Main
{
    public class MainController : MonoSingleton<MainController>, IController
    {
        #region EDITOR EXPOSED FIELDS

        #endregion

        #region FIELDS

        public static bool IsNull
        {
            get { return mInstance == null; }
        }

        #endregion

        #region PROPERTIES

        #endregion

        #region EVENT FUNCTION
        protected override void OnDestroy()
        {
            base.OnDestroy();
            GetArchitecture().GetModel<MainModel>().Dispose();
        }
        
        #endregion

        #region METHODS
        
        #endregion


        public IArchitecture GetArchitecture()
        {
            return MainArchitecture.Interface;
        }
        
        
        
    }
    
    
    public class MainArchitecture :Architecture<MainArchitecture>
    {
        protected override void Init()
        {
            this.RegisterModel(new MainModel());
        }
    }
}