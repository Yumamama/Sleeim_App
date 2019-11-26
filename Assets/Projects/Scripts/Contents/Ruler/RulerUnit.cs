/**
 * @file
 * @brief	数値選択ダイアログ ルーラー目盛定義
 */
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 数値選択ダイアログ ルーラー目盛定義
/// </summary>
public class RulerUnit : MonoBehaviour {

	/// <summary>
	/// 目盛（大）
	/// </summary>
	public GameObject ImageScaleBold;
	/// <summary>
	/// 目盛（中）
	/// </summary>
	public GameObject ImageScaleMiddle;
	/// <summary>
	/// 目盛（小）
	/// </summary>
	public GameObject ImageScaleNormal;
	/// <summary>
	/// 表示値
	/// </summary>
	public Text Value;
	/// <summary>
	/// ID
	/// </summary>
	public int ID;

	/// <summary>
	/// 矢印接触時のコールバック
	/// </summary>
	/// <param name="ID">ID</param>
	public delegate void OnEnterCenterMark(int ID);

	private OnEnterCenterMark callback = null;

	/// <summary>
	/// 目盛種別
	/// </summary>
	public enum scaleType
	{
		None = 0,
		Bold = 1,
		Middle = 2,
		Normal = 3,
	}

	/// <summary>
	/// 目盛設定
	/// </summary>
	/// <param name="type">目盛種別</param>
	/// <param name="value">表示値</param>
	/// <param name="cbFunc">矢印接触時のコールバック</param>
	public void SetUnit(scaleType type , string value, OnEnterCenterMark cbFunc = null)
	{
		// コールバック登録
		callback = cbFunc;

		// 一旦全消しして必要な分だけ表示指定
		ImageScaleBold.SetActive(false);
		ImageScaleMiddle.SetActive(false);
		ImageScaleNormal.SetActive(false);
		switch (type)
		{
			case scaleType.Bold:
				{
					ImageScaleBold.SetActive(true);
					break;
				}
			case scaleType.Middle:
				{
					ImageScaleMiddle.SetActive(true);
					break;
				}
			case scaleType.Normal:
				{
					ImageScaleNormal.SetActive(true);
					break;
				}
		}

		// 表示値を設定
		Value.text = value;
	}

	// NOTE:OnTriggerEnter2Dでは判定が漏れる場合があるのでOnTriggerStay2Dに変更
	void OnTriggerStay2D(Collider2D other)
	{
		if (callback != null && other.gameObject.name == "ImageCenterMark")
		{
			callback(ID);
		}
	}
}
