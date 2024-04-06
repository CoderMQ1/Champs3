using System;
using DG.Tweening;
using UnityEngine;
using QFramework;

namespace SquareHero.Hotfix
{
	public partial class LoadingScreen : ViewController
	{
		private Sequence sequence;
		void Start()
		{
			// Code Here
		}


		private void OnEnable()
		{
			if (sequence != null )
			{
				sequence.Play();
			}
			else
			{
				sequence = DOTween.Sequence();
				LoadingLabel.text = "Loading.";
				sequence.AppendInterval(0.4f);
				sequence.InsertCallback(0.4f,() =>
				{
					LoadingLabel.text = "Loading..";
				});
				sequence.AppendInterval(0.4f);
				sequence.InsertCallback(0.8f,() =>
				{
					LoadingLabel.text = "Loading...";
				});
				sequence.AppendInterval(0.4f);
				sequence.InsertCallback(1.2f,() =>
				{
					LoadingLabel.text = "Loading.";
				});
				sequence.SetLoops(-1);
				sequence.Play();
			}

			// Global.Instance.UICamera.enabled = false;
		}

		private void OnDisable()
		{
			sequence.Pause();
			// Global.Instance.UICamera.enabled = true;
		}

		private void OnDestroy()
		{
			sequence.SetLoops(0);
			sequence.Kill();
		}


		public void ShowLoading()
		{
			Loading.gameObject.SetActive(true);
			Global.Instance.UICamera.cullingMask = 0;
		}
		
		public void HideLoading()
		{
			Loading.gameObject.SetActive(false);
			Global.Instance.UICamera.cullingMask = LayerMask.GetMask("UI");
		}
	}
	
}
