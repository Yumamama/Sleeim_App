﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class SleepListElement : MonoBehaviour {

	Data myData;
	public Text Date;
	public Text SleepTime;
	public Text ApneaTime;
	public Text ApneaCount;

	/// <summary>
	/// 表示に必要なデータをまとめたクラス
	/// </summary>
	public class Data {
		DateTime myDate;			//いつのデータか
		List<DateTime> timeList;	//睡眠データリスト
		int longestApneaTime;		//無呼吸最長時間
		int apneaCount;				//無呼吸検知回数
		int dateIndex;				//同日データの何件目か
		int crossSunCount;			//日付またぎデータが何件あったか
		int sameDateNum;			//同一日の全てのデータ個数
		int crossSunNum;			//同一日の日マタギのみのデータ個数

		public Data (DateTime myDate, List<DateTime> dateList, int longestApneaTime, int apneaCount, int dateIndex, int crossSunCount, int sameDateNum, int crossSunNum) {
			this.myDate = myDate;
			this.timeList = dateList;
			this.longestApneaTime = longestApneaTime;
			this.apneaCount = apneaCount;
			this.dateIndex = dateIndex;
			this.crossSunCount = crossSunCount;
			this.sameDateNum = sameDateNum;
			this.crossSunNum = crossSunNum;
		}
		public DateTime GetDate () {
			return this.myDate;
		}
		public List<DateTime> GetTimeList () {
			return this.timeList;
		}
		public int GetLongestApneaTime () {
			return this.longestApneaTime;
		}
		public int GetApneaCount () {
			return this.apneaCount;
		}
		/// <summary>
		/// 同日の何件目のデータか返します
		/// </summary>
		public int GetTodayCount () {
			return this.dateIndex;
		}
		/// <summary>
		/// 日付またぎデータが何件あったかを返します
		/// </summary>
		public int GetCrossSunCount () {
			return this.crossSunCount;
		}
		/// <summary>
		/// 同一日のデータ個数を返します
		/// </summary>
		public int GetSameDateNum () {
			return this.sameDateNum;
		}
		/// <summary>
		/// 同一日の日マタギデータ個数を返します
		/// </summary>
		public int GetCrossSunNum () {
			return this.crossSunNum;
		}
	}

	//グラフに遷移するボタンをタップした際に呼び出される
	public void OnToGraphButtonTap () {
		//指定の日付のグラフを開くために日付保存
		if (myData == null)
			return;
		DateTime myDate = myData.GetDate ();
		UserDataManager.Scene.SaveGraphDate (myDate);
		//タブは初期状態で選択されるように設定
		UserDataManager.Scene.InitGraphTabSave ();
		UserDataManager.Scene.InitGraphDataTabSave ();
		//グラフ画面に遷移
		SceneTransitionManager.LoadLevel (SceneTransitionManager.LoadScene.Graph);
	}

	//日付
	String DateText (DateTime startTime, DateTime endTime, int dateIndex, int crossSunCount, int sameDateNum, int crossSunNum) {
		//就寝時
		string start_day = startTime.Day.ToString ();
		string start_dayOfWeek = startTime.ToString ("ddd", new System.Globalization.CultureInfo ("ja-JP"));	//曜日
		//起床時
		string end_day = endTime.Day.ToString ();
		string end_dayOfWeek = endTime.ToString ("ddd", new System.Globalization.CultureInfo ("ja-JP"));	//曜日

		if (isCrossTheSun (startTime, endTime)) {
			bool isNecessaryIndex = crossSunNum > 1;
			int indexCount = crossSunCount;
			//就寝時と起床時の日付が異なっていたら「就寝日～起床日」を返す
			return start_day + "(" + start_dayOfWeek + ")" + "～" + end_day + "(" + end_dayOfWeek + ")" + (isNecessaryIndex ? " (" + indexCount.ToString () + ")" : "");
		} else {
			bool isNecessaryIndex = (sameDateNum - crossSunNum) > 1;
			int indexCount = dateIndex + 1;
			//就寝時と起床時の日付が同じであれば「就寝日」を返す
			return start_day + "(" + start_dayOfWeek + ")" + (isNecessaryIndex ? " (" + indexCount.ToString () + ")" : "");
		}
	}

	//日付をまたいでいるかどうか
	bool isCrossTheSun (DateTime start, DateTime end) {
		return start.Month != end.Month || start.Day != end.Day;
	}

	//睡眠時間
	String GetSleepTime (Data data) {
		//睡眠時間を秒に変換して取得
		//就寝時間はCSVのヘッダーを使うように注意
		int sec = Graph.Time.GetDateDifferencePerSecond (data.GetDate (), data.GetTimeList ().Last ());
		int min = (sec / 60) % 60;		//分に変換
		int hour = (sec / 60) / 60;		//時間に変換
		string hh = hour.ToString ("00");
		string mm = min.ToString ("00");
		string result = hh + ":" + mm;
		return result.ToUpper ();
	}

	public void SetInfo (Data data) {
		this.myData = data;
		//日時設定
		Date.text = DateText (
			data.GetDate (), 
			data.GetTimeList ().Last (), 
			data.GetTodayCount (),
			data.GetCrossSunCount (),
			data.GetSameDateNum (),
			data.GetCrossSunNum ());
		//睡眠時間設定
		SleepTime.text = GetSleepTime (data);
		//最長無呼吸時間設定
		ApneaTime.text = data.GetLongestApneaTime ().ToString () + "秒";
		//無呼吸検知時間設定
		ApneaCount.text = data.GetApneaCount ().ToString () + "回";
	}
}
