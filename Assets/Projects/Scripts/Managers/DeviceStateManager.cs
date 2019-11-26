using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// デバイスとの接続状態をどのシーンからでも受け取れるようにするためのクラス
/// スプラッシュの初期化処理でデバイス切断などのコールバックを設定し、ホーム画面でリアルタイムにコールバックを受け取ることができなかったため、その橋渡しとして作成。
/// </summary>
public class DeviceStateManager : SingletonMonoBehaviour<DeviceStateManager> {

	/// <summary>
	/// デバイスとの接続が切れた際に呼び出されるイベント
	/// </summary>
	public event Action OnDeviceDisConnectEvent;

	/// <summary>
	/// デバイスとのペアリングが切れた際に呼び出されるイベント
	/// </summary>
	public event Action OnDevicePareringDisConnectEvent;

	/// <summary>
	/// ファームウェアアップデートが必要になった際に呼び出されるイベント
	/// </summary>
	public event Action OnFarmwareUpdateNecessaryEvent;

	/// <summary>
	/// ファームウェアアップデートが必要なくなった際に呼び出されるイベント
	/// </summary>
	public event Action OnFarmwareUpdateNonNecessaryEvent;

	/// <summary>
	/// デバイス接続が切れた際に呼び出し
	/// </summary>
	public void OnDeviceDisConnect() {
		if (OnDeviceDisConnectEvent != null)
			OnDeviceDisConnectEvent();
	}

	/// <summary>
	/// デバイスとのペアリングが切れた際に呼び出し
	/// </summary>
	public void OnDevicePrearingDisConnect () {
		if (OnDevicePareringDisConnectEvent != null)
			OnDevicePareringDisConnectEvent ();
	}

	/// <summary>
	/// ファームウェアアップデートが必要になった際に呼び出す
	/// </summary>
	public void OnFirmwareUpdateNecessary () {
		if (OnFarmwareUpdateNecessaryEvent != null)
			OnFarmwareUpdateNecessaryEvent ();
	}

	/// <summary>
	/// ファームウェアアップデートが必要なくなった際に呼び出す
	/// </summary>
	public void OnFirmwareUpdateNonNecessary () {
		if (OnFarmwareUpdateNonNecessaryEvent != null)
			OnFarmwareUpdateNonNecessaryEvent ();
	}
}
