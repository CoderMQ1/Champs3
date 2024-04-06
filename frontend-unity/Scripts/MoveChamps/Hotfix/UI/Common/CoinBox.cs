using System;
using UnityEngine;
using QFramework;
using SquareHero.Hotfix.Main;
using SquareHero.Hotfix.Model;

namespace SquareHero.Hotfix
{
	public partial class CoinBox : ViewController
	{
		protected float _coin;

		protected MainModel _model;
		void Start()
		{
			// Code Here
			
		}
		

		public void Refresh()
		{
			var mainModel = MainController.Instance.GetArchitecture().GetModel<MainModel>();
			_coin = mainModel.Bag.Coin;
			DiamondCount.text = mainModel.Bag.Diamond.ToString();
			CoinCount.text = _coin.ToString("0.0000");
			SeasonPointCount.text = mainModel.Bag.SpinChip.ToString();
			LotteryKeyCount.text = mainModel.Bag.LotteryKey.ToString();

			_model = mainModel;
		}

		private void LateUpdate()
		{
			if (_model != null && _model.Bag.Coin != _coin)
			{
				Refresh();
			}
		}
	}
}