using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AlermDialog : DialogBase {

	[SerializeField] Text textTitle;
	[SerializeField] Text textMessage;
	[SerializeField] Button btnStop;

	Action onStopButtonTap;
	const string dialogPath = "Prehabs/Dialogs/AlermDialog";

	/// <summary>
	/// アラームダイアログを表示します。
	/// </summary>
	/// <param name="title">タイトル名</param>
	/// <param name="message">表示するメッセージ</param>
	/// <param name="onStopButtonTap">停止ボタンを押した際に実行されるコールバック</param>
	public static void Show (string title, string message, Action onStopButtonTap) {
		GameObject prehab = CreateDialog (dialogPath);
		//コンテンツ設定
		AlermDialog dialog = prehab.GetComponent <AlermDialog> ();
		dialog.Init (title, message, onStopButtonTap);
		dialogObj = prehab;
	}

	//初期化
	void Init (string title, string message, Action onStopButtonTap) {
		textTitle.text = title;
		textMessage.text = message;
		this.onStopButtonTap = onStopButtonTap;
	}

	//「停止」ボタンが押されたときに呼び出される
	public void OnStopButtonTap () {
		Debug.Log ("STOP");
		if (onStopButtonTap != null)
			onStopButtonTap ();
		Dismiss ();
	}
}
