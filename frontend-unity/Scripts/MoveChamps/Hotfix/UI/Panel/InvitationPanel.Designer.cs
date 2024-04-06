using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace SquareHero.Hotfix.UI
{
	// Generate Id:bcfd49cc-c8e9-4b82-aeeb-0dbb29d79677
	public partial class InvitationPanel
	{
		public const string Name = "InvitationPanel";
		
		[SerializeField]
		public UnityEngine.CanvasGroup Views;
		[SerializeField]
		public UnityEngine.UI.Button BackBtn;
		[SerializeField]
		public TMPro.TextMeshProUGUI Link;
		[SerializeField]
		public UnityEngine.UI.Button CopyBtn;
		[SerializeField]
		public UnityEngine.UI.Button Twitter;
		[SerializeField]
		public UnityEngine.UI.Button FaceBook;
		[SerializeField]
		public UnityEngine.UI.Button Telegram;
		[SerializeField]
		public TMPro.TextMeshProUGUI Tip;
		
		private InvitationPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Views = null;
			BackBtn = null;
			Link = null;
			CopyBtn = null;
			Twitter = null;
			FaceBook = null;
			Telegram = null;
			Tip = null;
			
			mData = null;
		}
		
		public InvitationPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		InvitationPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new InvitationPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
