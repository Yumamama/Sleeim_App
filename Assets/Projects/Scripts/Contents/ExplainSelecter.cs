using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Toggle))]
/// <summary>
/// Toggleで画像と説明文を合わせて切り替えるためのコンポーネントです。
/// 一つのトグルに一対一対応します。
/// </summary>
public class ExplainSelecter : MonoBehaviour {

	Toggle toggle;
	public bool isDefalt;		//初期選択項目かどうか
	public Image imageField;	//画像を表示する場所
	public Text textField;		//説明文を表示する場所
	public Sprite image;		//設定する画像
	public string text;			//設定する文章

	void Start () {
		toggle = GetComponent<Toggle> ();
		//選択されたときの挙動を設定
		toggle.onValueChanged.AddListener (isOn => {
			if (isOn) {
				DispExlpain ();
			} else {
				ClearExplain ();
			}
		});
		//初期選択項目ならアクティブにする
		if (isDefalt)
			toggle.isOn = true;
	}

	//説明文・画像を表示
	void DispExlpain () {
		imageField.sprite = image;
		textField.text = text;
	}

	//説明文・画像を非表示
	void ClearExplain () {
		imageField.sprite = null;
		textField.text = "";
	}
}
