// 
// 2023/12/26

using System;
using System.Security.Cryptography.X509Certificates;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomAdapters.GridView;
using Com.ForbiddenByte.OSA.CustomParams;
using UnityEngine;

namespace SquareHero.Hotfix
{
    public class SHScrollGridView : GridAdapter<SHGridParams, SHCellViewsHolder>
    {
        #region EDITOR EXPOSED FIELDS

        #endregion

        #region FIELDS

        public SHScrollGridViewAdapter Adapter = new SHScrollGridViewAdapter();
        
        #endregion

        #region PROPERTIES

        #endregion

        #region EVENT FUNCTION

        #endregion

        #region METHODS

        #endregion
        
        /// <inheritdoc/>
        protected override CellGroupViewsHolder<SHCellViewsHolder> GetNewCellGroupViewsHolder()
        {
            // Create cell group holders of our custom type (which stores the CSF component)
            return Adapter.GetNewCellGroupViewsHolder();
        }
        
        protected override void UpdateViewsHolder(CellGroupViewsHolder<SHCellViewsHolder> newOrRecycled)
        {
            base.UpdateViewsHolder(newOrRecycled);

            // Constantly triggering a twin pass after the current normal pass, so the CSFs will be updated
            ScheduleComputeVisibilityTwinPass();
        }
        protected override void UpdateCellViewsHolder(SHCellViewsHolder viewsHolder)
        {
            Adapter.UpdateCellViewsHolder(viewsHolder);
        }
    }
    [Serializable] // serializable, so it can be shown in inspector
    public class SHGridParams : GridParams
    {
        
    }

    public class SHCellViewsHolder : CellViewsHolder
    {
       
    }
    
    public class SHCellGroupViewsHolder : CellGroupViewsHolder<SHCellViewsHolder>
    {
        public UnityEngine.UI.ContentSizeFitter contentSizeFitterComponent;


        protected override SHCellViewsHolder CreateCellViewsHolder()
        {
            return base.CreateCellViewsHolder();
            
            
        }

        /// <inheritdoc/>
        public override void CollectViews()
        {
            base.CollectViews();
            
            // Since the group views holder is created at runtime internally, we also need to add the CSF by code
            contentSizeFitterComponent = root.gameObject.AddComponent<UnityEngine.UI.ContentSizeFitter>();
            contentSizeFitterComponent.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;

            // Keeping the CSF always enabled is easier to manage. We'll trigger a Twin pass very frequently, anyway
            contentSizeFitterComponent.enabled = true;
        }
    }

    public class SHScrollGridViewAdapter
    {
        public OnUpdateCellViewHolder UpdateHandler;
        public delegate void OnUpdateCellViewHolder(SHCellViewsHolder viewsHolder);
        public virtual CellGroupViewsHolder<SHCellViewsHolder> GetNewCellGroupViewsHolder()
        {
            return new SHCellGroupViewsHolder();
        }
        
        public virtual void UpdateCellViewsHolder(SHCellViewsHolder viewsHolder)
        {
            UpdateHandler?.Invoke(viewsHolder);
        }
    }
}