/**
 * @file
 * @brief	数値入力ダイアログ ルーラーUI表示制御
 */
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

/// <summary>
/// 数値入力ダイアログ
/// ルーラーUI表示制御
/// </summary>
[RequireComponent(typeof(InfiniteScroll))]
public class RulerController : UIBehaviour, IInfiniteScrollSetup {

	/// <summary>
	/// 現在の数値
	/// </summary>
	public float NowValue
	{
		get;
		private set;
	}

	// 目盛りの総数
	// 両端のパディング分も含む
	private int itemNumMax
	{
		get
		{
			return (int)(((decimal)rulerMaxValue - (decimal)rulerMinValue) / (decimal)rulerStepValue) + (paddingItemNum * 2);
		}
	}

	// 目盛り最小値、最大値、刻み幅
	private decimal rulerMinValue = 0m;
	private decimal rulerMaxValue = 20m;
	private decimal rulerStepValue = 0.1m;
	private decimal rulerDefaultValue = 0m;

	private decimal scaleBoldStartNum = 0m;

    private bool isTimeMode = false;

	// 両はじに置く空白アイテムの数
	// レイアウトにあわせて設定してあるので固定
	private const int paddingItemNum = 12;
	// 目盛り（太）の感覚
	private const int scaleTypeBoldStepNum = 10;
	// 目盛り（中）の感覚
	private const int scaleTypeMiddleStepNum = 5;

    // 目盛り（時間）の感覚
    private const int scaleTypeTimeStepNum = 6;

	/// <summary>
	/// ルーラー初期化
	/// </summary>
	/// <param name="minValue">最小値</param>
	/// <param name="maxValue">最大値</param>
	/// <param name="stepValue">刻み幅</param>
	/// <param name="firstValue">初期値</param>
    /// <param name="isTime">時間表示モード</param>
    public void Init(float minValue , float maxValue , float stepValue , float defaultValue, bool isTime = false)
	{
		// 範囲外の値が来たときは丸める
		if (defaultValue > maxValue)
		{
			defaultValue = maxValue;
		}
		if (defaultValue < minValue)
		{
			defaultValue = minValue;
		}

		rulerMinValue = (decimal)minValue;
		rulerMaxValue = (decimal)maxValue;
		rulerStepValue = (decimal)stepValue;
		rulerDefaultValue = (decimal)defaultValue;

        isTimeMode = isTime;

		// 目盛太字開始位置を算出
		scaleBoldStartNum = 0;
		while(scaleBoldStartNum < 100)
		{
			if((rulerMinValue + (rulerStepValue * scaleBoldStartNum)) % (rulerStepValue * 10) == 0)
			{
				break;
			}
			scaleBoldStartNum++;
		}
	}

	/// <summary>
	/// アイテム（目盛）更新後処理
	/// </summary>
	public void OnPostSetupItems()
	{
		// アイテム更新時リスナ設定
		var infiniteScroll = GetComponent<InfiniteScroll>();
		infiniteScroll.onUpdateItem.AddListener(OnUpdateItem);

		// スクロール制限設定
		// アイテムの総数にあわせて矩形のサイズを調整する
		GetComponentInParent<ScrollRect>().movementType = ScrollRect.MovementType.Elastic;
		var rectTransform = GetComponent<RectTransform>();
		var delta = rectTransform.sizeDelta;
		delta.x = infiniteScroll.itemScale * itemNumMax;
		rectTransform.sizeDelta = delta;

		// スクロール初期位置設定
		// floatのまま計算すると丸められるのでdecimalで計算
		int itemCount = (int)((rulerDefaultValue - rulerMinValue) / rulerStepValue);
		GetComponentInParent<FixedScrollRect>().SetAnchoredPosition(itemCount);
	}

	/// <summary>
	/// アイテム（目盛）更新処理
	/// </summary>
	/// <param name="itemCount">対象アイテム</param>
	/// <param name="obj">対象アイテムのオブジェクト</param>
	public void OnUpdateItem(int itemCount, GameObject obj)
	{
		if(itemCount < 0 || itemCount >= itemNumMax) {
			obj.SetActive (false);
		}
		else {
			obj.SetActive (true);
			RulerUnit ru = obj.GetComponent<RulerUnit>();

			ru.ID = itemCount;

			if (itemCount < paddingItemNum || itemCount > (itemNumMax - paddingItemNum))
			{
				// 両端のパディング
				ru.SetUnit(RulerUnit.scaleType.None, "");
			}
            else if (isTimeMode && 
                    ((itemCount - paddingItemNum - scaleBoldStartNum) % scaleTypeTimeStepNum == 0))
            {
                // 大目盛り（時間）
                decimal countnum = rulerMinValue + ((itemCount - paddingItemNum) * rulerStepValue);
                TimeSpan timespan = new TimeSpan(0, (int)countnum,0);
                string valuetext = timespan.Hours.ToString("00") + ":" + timespan.Minutes.ToString("00");
                ru.SetUnit(RulerUnit.scaleType.Bold, valuetext, onEnterCenterMark);
            }
            else if (isTimeMode)
            {
                // 小目盛り（時間）
                ru.SetUnit(RulerUnit.scaleType.Normal, "", onEnterCenterMark);
            }
			else if ((itemCount- paddingItemNum - scaleBoldStartNum) % scaleTypeBoldStepNum == 0)
			{
                // 大目盛り
				string valuetext = (rulerMinValue + ((itemCount- paddingItemNum) * rulerStepValue)).ToString("0");
				ru.SetUnit(RulerUnit.scaleType.Bold, valuetext, onEnterCenterMark);
			}
			else if ((itemCount - paddingItemNum - scaleBoldStartNum) % scaleTypeMiddleStepNum == 0)
			{
				// 中目盛り
				ru.SetUnit(RulerUnit.scaleType.Middle, "", onEnterCenterMark);
			}
			else
			{
				// 小目盛り
				ru.SetUnit(RulerUnit.scaleType.Normal, "", onEnterCenterMark);
			}
		}
	}

	/// <summary>
	/// 中央の▽印と接触した時の処理
	/// 現在値を特定するのに使用する
	/// </summary>
	/// <param name="ID">接触したアイテムのID</param>
	void onEnterCenterMark(int ID)
	{
		// 現在値更新
		NowValue = (float)(rulerMinValue + (ID - paddingItemNum) * rulerStepValue);
	}
}
