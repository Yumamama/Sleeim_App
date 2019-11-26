using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateDialog : DialogBase {

	[SerializeField] Text textMessage;
	const string dialogPath = "Prehabs/Dialogs/UpdateDialog";

	//ダイアログを表示する
	public static void Show (string message) {
		GameObject prehab = CreateDialog (dialogPath);
		UpdateDialog dialog = prehab.GetComponent <UpdateDialog> ();
		dialog.Init (message);
	}

	/// <summary>
	/// 表示するメッセージを変更します
	/// </summary>
	public static void ChangeMessage (string message) {
		dialogObj.GetComponent <UpdateDialog> ().Init (message);
	}

	void Init (string message) {
		textMessage.text = message;
	}
}
