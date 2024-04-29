using System;
using UnityEngine;

namespace champs3.Hotfix
{
    public class MoveNode : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        public int Index;
        public ActionType ActionType;
        public MoveType MoveType;
        public Vector3 MoveDir = Vector3.forward;
        public bool CanUseProp = false;

        #endregion

        #region FIELDS
        private Vector3 _startPosition;

        #endregion

        #region PROPERTIES

        #endregion

        #region EVENT FUNCTION

        #endregion

        #region METHODS

        private void OnDrawGizmos()
        {
            Color color = Gizmos.color;
            Gizmos.color = Color.green;
            
            if (!Application.isPlaying)
            {
                this._startPosition = this.transform.position;
            }

            Vector3 startPos = this._startPosition;
            Vector3 endPos = startPos + this.MoveDir;
            float dist = Vector3.Distance(startPos, endPos);
            Gizmos.DrawLine(startPos, endPos);
            Gizmos.DrawLine(endPos, ((startPos + this.transform.right * dist) - endPos).normalized * 0.5f + endPos);
            Gizmos.DrawLine(endPos, ((startPos - this.transform.right * dist) - endPos).normalized * 0.5f + endPos);

            Gizmos.color = color;
        }

        #endregion

    }
    /// <summary>
    /// 角色动作
    /// </summary>
    public enum ActionType
    {
        Idle,
        Run,
        Dive,
        Swiming,
        ClimbStart,
        Climbing = 5,
        ClimbingOnTop,
        FlyStart,
        Flying,
        FlyEnd
    }
    
    /// <summary>
    /// 移动类型
    /// </summary>
    public enum MoveType
    {
        None,
        Run,
        Swim,
        Climb,
        Fly
    }
}