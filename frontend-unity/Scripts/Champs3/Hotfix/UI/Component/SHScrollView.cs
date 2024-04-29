// 
// 2023/12/15
using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;
using Com.ForbiddenByte.OSA.Util;
using Com.ForbiddenByte.OSA.DataHelpers;
using UnityEngine.Events;

namespace champs3.Hotfix
{
    public class SHScrollView : OSA<SHParams, SHItemViewsHolder>
    {
        public CreateItem OnCreate;
        public OnUpdateViewHolder OnUpdate;
        public delegate SHItemViewsHolder CreateItem(int itenIdex, GameObject prefab);

        public delegate void OnUpdateViewHolder(SHItemViewsHolder viewHolder);
        protected override SHItemViewsHolder CreateViewsHolder(int itemIndex)
        {
            SHItemViewsHolder shItemViewsHolder = null;
            if (OnCreate != null)
            {
                shItemViewsHolder = OnCreate(itemIndex, _Params.ItemPrefab.gameObject);
            }
            else
            {
                shItemViewsHolder = new SHItemViewsHolder();
            }

            shItemViewsHolder.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
            return shItemViewsHolder;
        }

        protected override void UpdateViewsHolder(SHItemViewsHolder newOrRecycled)
        {;
            OnUpdate?.Invoke(newOrRecycled);
        }
    }

    [Serializable] // serializable, so it can be shown in inspector
    public class SHParams : BaseParamsWithPrefab
    {
        
    }

    public class SHItemViewsHolder : BaseItemViewsHolder
    {
        
    }
}