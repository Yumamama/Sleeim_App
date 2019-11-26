using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Toggle))]
public class ToggleExtendsColorChanger : MonoBehaviour {

	[SerializeField]Graphic targetGraphic;		//色を変更するターゲット
	[SerializeField]Color isOnColor;
	[SerializeField]Color isOffColor;

	void Start () {
		Debug.Log ("Toggle Initialize.");
		//トグルがONでターゲットがisOnColorに、OFFでisOffColorになるように設定する
		var toggle = GetComponent<Toggle> ();
		toggle.onValueChanged.AddListener ((bool isOn) => {
			Debug.Log (this.gameObject.name + ":" + isOn);
			targetGraphic.color = isOn ? isOnColor : isOffColor;
		});
	}
}
