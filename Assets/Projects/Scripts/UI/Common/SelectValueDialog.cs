/**
 * @file
 * @brief	数値選択ダイアログコントローラ
 * @par Copyright (C) 2016 nanoconnect,Inc All Rights Reserved.
 */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// 数値選択ダイアログコントローラ
/// </summary>
public class SelectValueDialog : DialogBase
{
	[SerializeField]Text titleDispText;
	[SerializeField]Text valueDispText;	//目盛りの現在値を表示するテキストフィールド
	[SerializeField]Text unitDispText;	//現在値の単位を表示するテキストフィールド 
	const string dialogPath = "Prehabs/Dialogs/SelectValueDialog";
	public delegate void ValueSelectCallback (ButtonItem status, float value, GameObject dialog);
	float currentVal = 0;

	public enum ButtonItem {
		OK,
		Cancel,
		Clear
	}

	void Awake()
	{
		getController();
		currentValueText = valueDispText;
		currentValueText.text = "";
		//ドラッグ設定
		EventSystem eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
		eventSystem.pixelDragThreshold = 30;
	}

	/// <summary>
	/// 数値選択ダイアログを表示します
	/// </summary>
	/// <param name="resultSetField">設定値の出力先</param>
	/// <param name="valueSet">共通入力UI設定値</param>
	/// <param name="onTapOK">OKボタンが選択された際に実行されるコールバック</param>
	/// <param name="onTapCancel">Cancelボタンが選択された際に実行されるコールバック</param>
	public static void Show (SelectValueDialogParamSet valueSet, ValueSelectCallback valueCallBack) {
		//ダイアログ作成
		GameObject dialog = CreateDialog (dialogPath);
		var dialogController = dialog.GetComponent<SelectValueDialog>();
		// ダイアログに値を設定
		dialogController.Init (valueSet);
		// ダイアログの各ボタンにコールバック登録
		if (valueSet.displayType == SelectValueDialogParamSet.DISPLAY_TYPE.Numeric)
		{
			dialogController.SetCallback (
				delegate {
					valueCallBack (ButtonItem.OK, dialogController.currentValue, dialog);
				},
				delegate
				{
					valueCallBack (ButtonItem.Cancel, dialogController.currentValue, dialog);
				}
			);
		}
		else if (valueSet.displayType == SelectValueDialogParamSet.DISPLAY_TYPE.NumericClear)
		{
			dialogController.SetCallback (
				delegate {
					valueCallBack (ButtonItem.OK, dialogController.currentValue, dialog);
				},
				delegate
				{
					valueCallBack (ButtonItem.Cancel, dialogController.currentValue, dialog);
				},
				delegate
				{
					valueCallBack (ButtonItem.Clear, dialogController.currentValue, dialog);
				}
			);
		}
	}

	/// <summary>
	/// 現在値（数値）
	/// </summary>
	public float currentValue
	{
		get {
			return currentVal;
		}
		private set {
			//少数第二位で四捨五入する
			currentVal = (float) Math.Round (value, 2, MidpointRounding.AwayFromZero);
		}
	}

    /// <summary>
    /// 現在値（時間）
    /// </summary>
    public TimeSpan currentTime
    {
        get
        {
            // 分をTimeSpan形式に変換
            return new TimeSpan(0,(int)currentValue, 0);
        }
    }

	Text currentValueText;
	ValueType valuetype = ValueType.Decimal;
	RulerController rc = null;

	enum ValueType
	{
		Integer,
		Decimal,
        Time,
	}

	void Update()
	{
		if (rc != null)
		{
			// 表示値更新
			currentValue = rc.NowValue;
			// 表示
            if (valuetype == ValueType.Time)
            {
                // 整数
                currentValueText.text = currentTime.Hours.ToString("00") + ":" + currentTime.Minutes.ToString("00");
            }
            else if (valuetype == ValueType.Integer)
			{
				// 整数
				currentValueText.text = currentValue.ToString("0");
			}
			else
			{
				// 小数
				currentValueText.text = currentValue.ToString("0.0");
			}
			
		}
	}

	/// <summary>
	/// 初期化
	/// </summary>
	/// <param name="valueset">初期値</param>
	public void Init(SelectValueDialogParamSet valueset)
	{
		if (rc == null)
		{
			getController();
		}

        // パラメータチェック
        if (valueset.maxValue < valueset.minValue)
        {
            // 常にminValueがmaxValueより小さくなるように調整
            float temp = valueset.maxValue;
            valueset.maxValue = valueset.minValue;
            valueset.minValue = temp;
        }
        if (valueset.currentValue < valueset.minValue || valueset.currentValue > valueset.maxValue)
        {
            // currentValueが設定範囲外の場合は強制的にminValueを初期値とする
            valueset.currentValue = valueset.minValue;
        }

        // nullを空文字に置き換える
        valueset.valueTitle = valueset.valueTitle ?? "";
        valueset.valueUnit = valueset.valueUnit ?? "";


        // 通知表示タイプ設定
        if (valueset.displayType == SelectValueDialogParamSet.DISPLAY_TYPE.Time)
        {
            valuetype = ValueType.Time;

            // ルーラー部分初期化
            rc.Init(0, 60*24, 10, (float)valueset.currentTime.TotalMinutes, true);
        }
        else
        {
			if (valueset.stepValue < 1)
			{
				valuetype = ValueType.Decimal;
			}
			else
			{
				valuetype = ValueType.Integer;
			}

			// ルーラー部分初期化
			rc.Init(valueset.minValue, valueset.maxValue, valueset.stepValue, valueset.currentValue);
        }

		// クリアボタン有無設定
		//使用しない
		if (valueset.displayType != SelectValueDialogParamSet.DISPLAY_TYPE.NumericClear)
		{
//			transform.Find("Background/Footer/Child1/ClearButton").gameObject.SetActive(false);
		}
		
		// タイトル初期化
		titleDispText.text = valueset.valueTitle;
		unitDispText.text = valueset.valueUnit;
	}

	/// <summary>
	/// コールバック設定
	/// </summary>
	/// <param name="okCB">決定時コールバック</param>
	/// <param name="calcelCB">キャンセル時コールバック</param>
	/// <param name="clearCB">クリア時コールバック</param>
	public void SetCallback(UnityAction okCB, UnityAction calcelCB, UnityAction clearCB = null)
	{
		Button okButton = transform.Find("Background/Footer/Child2/OkButton").GetComponent<Button>();
		okButton.onClick.RemoveAllListeners();
		okButton.onClick.AddListener(okCB);
		okButton.onClick.AddListener (() => Dismiss ());	//OKボタン押下でダイアログを閉じるように
		Button ngButton = transform.Find("Background/Footer/Child2/NgButton").GetComponent<Button>();
		ngButton.onClick.RemoveAllListeners();
		ngButton.onClick.AddListener(calcelCB);
		ngButton.onClick.AddListener (() => Dismiss ());	//NGボタン押下でダイアログを閉じるように
		if (clearCB != null)
		{
			Button ClearButton = transform.Find("Background/Footer/Child1/ClearButton").GetComponent<Button>();
			ClearButton.onClick.RemoveAllListeners();
			ClearButton.onClick.AddListener(clearCB);
		}
	}

	/// <summary>
	///  ルーラーコントローラ取得
	/// </summary>
	private void getController()
	{
		rc = transform.Find("Background/Ruler/TouchArea/View").GetComponent<RulerController>();
	}
}

/// <summary>
/// 数値入力ダイアログコントローラー
/// 設定値クラス
/// </summary>
public class SelectValueDialogParamSet
{
	/// <summary>
	/// 数値入力ダイアログコントローラー設定値作成
	/// </summary>
	/// <param name="type">表示形式(Numericが通常)</param> 
	/// <param name="title">タイトル名</param>
	/// <param name="unit">数値の単位</param>
	/// <param name="maxVal">最大値</param>
	/// <param name="minVal">最小値</param>
	/// <param name="stepVal">値の間隔</param>
	/// <param name="currentVal">現在値</param>
	public SelectValueDialogParamSet (DISPLAY_TYPE type, string title, string unit, float maxVal, float minVal, float stepVal, float currentVal) {
		this.valueTitle = title;
		this.valueUnit = unit;
		this.maxValue = maxVal;
		this.minValue = minVal;
		this.stepValue = stepVal;
		this.currentValue = currentVal;
	}

    // 表示形式
    public enum DISPLAY_TYPE
    {
        Numeric,     // 数値
		NumericClear,// 数値・クリア機能あり
		Time,        // 時間
    }

    /// <summary>
    /// 表示形式
    /// </summary>
    public DISPLAY_TYPE displayType = DISPLAY_TYPE.Numeric;

	/// <summary>
	/// タイトル表示
	/// </summary>
	public string valueTitle;
	/// <summary>
	/// 数値の単位名
	/// </summary>
	public string valueUnit;

    // ---------------------------
    // 表示形式「数値」時のみ使用
    // ---------------------------
	/// <summary>
	/// 設定化可能な最大値
	/// </summary>
	public float maxValue;
	/// <summary>
	/// 設定可能な最小値
	/// </summary>
	public float minValue;
	/// <summary>
	/// 現在の値
	/// </summary>
	public float currentValue;
	/// <summary>
	/// １メモリごとの増加値
	/// </summary>
	public float stepValue;

	// ---------------------------
	// 表示形式「時間」時のみ使用
	// ---------------------------
	/// <summary>
	/// 現在の時間（0:00～24:00）
	/// </summary>
	public TimeSpan currentTime = TimeSpan.Zero;
}
