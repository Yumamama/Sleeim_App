using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// Bluetooth接続で、検出したデバイスを表示・接続するためのリストの要素
/// </summary>
public class DeviceListElement : MonoBehaviour {

	public Text deviceName;
	public Button connectButton;

	/// <summary>
	/// 初期化します。
	/// </summary>
	/// <param name="deviceName">表示するデバイス名</param>
	/// <param name="connectCallback">接続ボタンを押したときに実行されるメソッド</param>
	public void Initialize (string deviceName, Action connectCallback) {
		SetDeviceName (deviceName);
		SetConnectButtonCallBack (connectCallback);
	}

	// デバイス名を設定
	void SetDeviceName (string name) {
		deviceName.text = name;
	}

	// 接続ボタンを押したときに実行されるメソッドを登録
	void SetConnectButtonCallBack (Action action) {
		connectButton.onClick.AddListener (() => {
			action ();
		});
	}
}
