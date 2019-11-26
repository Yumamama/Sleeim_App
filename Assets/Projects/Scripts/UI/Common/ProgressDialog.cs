using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// データダウンロード、アップロードなどの進捗率を表すためのダイアログ
/// </summary>
public class ProgressDialog : DialogBase {

	[SerializeField] Text textMessage = null;
	[SerializeField] Text progressMessage = null;
	[SerializeField] Image progressBar = null;
	const string dialogPath = "Prehabs/Dialogs/ProgressDialog";
	static int allDataCount = 0;

	/// <summary>
	/// データ件数の進捗表示ダイアログを表示します
	/// </summary>
	/// <param name="message">初期説明文</param>
	/// <param name="allDataCount">全データ件数</param>
	public static void Show (string _message, int _allDataCount, int _completeDataCount = 0) {
		GameObject prehab = CreateDialog (dialogPath);
		ProgressDialog dialog = prehab.GetComponent<ProgressDialog> ();
		dialog.Init (_message, _completeDataCount, _allDataCount);
	}

	void Init (string _message, int _completeDataCount, int _allDataCount) {
		allDataCount = _allDataCount;
		textMessage.text = _message;
		UpdateProgressMessage (_completeDataCount, _allDataCount);
		UpdateProgressBar (_completeDataCount, _allDataCount);
	}

	void UpdateProgressMessage (int completeDataCount, int allDataCount) {
		string message = completeDataCount.ToString () + "/" + allDataCount.ToString () + "件";
		progressMessage.text = message;
	}

	void UpdateProgressBar (int completeDataCount, int allDataCount) {
		float progressPercent = (float)completeDataCount / (float)allDataCount;
		progressBar.fillAmount = progressPercent;
	}

	/// <summary>
	/// 進捗の表示を変化させます
	/// </summary>
	/// <param name="completeDataCount">完了したデータ個数</param>
	public static void UpdateProgress (int completeDataCount) {
		Debug.Log ("UpdateDialog_Update:" + completeDataCount);
		var progressDialog = dialogObj.GetComponent<ProgressDialog> ();
		progressDialog.UpdateProgressMessage (completeDataCount, allDataCount);
		progressDialog.UpdateProgressBar (completeDataCount, allDataCount);
	}
}
