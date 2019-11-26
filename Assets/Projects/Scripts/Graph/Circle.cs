using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Graph {
	[RequireComponent (typeof(RectTransform))]
	public class Circle : MonoBehaviour {
		[SerializeField]
		Sprite circleImage;
		[SerializeField]
		float paddingSize;
		RectTransform circleField;					//作成する円の大きさを決定する
		List<Fan> fanList = new List<Fan>();		//円を形成する扇の要素のリスト

		void Awake () {
			circleField = GetComponent<RectTransform> ();
		}

		/// <summary>
		/// 要素を指定し、円グラフを作成します
		/// </summary>
		/// <param name="dataList">Data list.</param>
		public void Create (List<Fan.Data> dataList) {
			ClearCircle ();	//初期化
			if (circleImage == null) {
				Debug.Log ("Set the circle image");
				return;
			}
			foreach (Fan.Data data in dataList) {
				fanList.Add (CreateFanObj (data));
			}
			UpdateVisual ();
		}

		/// <summary>
		/// 要素を指定し、円グラフを作成します
		/// </summary>
		/// <param name="dataList">Data list.</param>
		public void Create (List<List<Fan.Data>> dataList) {
			ClearCircle ();	//初期化
			if (circleImage == null) {
				Debug.Log ("Set the circle image");
				return;
			}
			List<List<Fan>> unitList = new List<List<Fan>> ();
			for (int i = 0; i < dataList.Count; i++) {
				List<Fan> elementList = new List<Fan> ();
				for (int j = 0; j < dataList [i].Count; j++) {
					Fan fan = CreateFanObj (dataList [i] [j]);
					elementList.Add (fan);
					fanList.Add (fan);	//要素の削除などに対応するためこちらも必要
				}
				unitList.Add (elementList);
			}

			for (int i = 0; i < unitList.Count; i++) {
				for (int j = 0; j < unitList [i].Count; j++) {
					unitList [i] [j].UpdateVisual (i, j, unitList);
				}
			}
		}

		/// <summary>
		/// 円グラフに要素を追加します
		/// </summary>
		/// <param name="data">Data.</param>
		public void AddElement (Fan.Data data) {
			fanList.Add (CreateFanObj (data));
			UpdateVisual ();
		}

		//円の要素を追加したり値を変更した際に、見た目への反映を行います
		void UpdateVisual () {
			for (int i = 0; i < fanList.Count; i++) {
				fanList [i].UpdateVisual (i, fanList);
			}
		}

		//円を形成する扇オブジェクトを作成
		//大枠を作成するだけで、色や形などの細かい設定は行わない
		Fan CreateFanObj (Fan.Data data) {
			var fanObj = new GameObject ("fan");
			fanObj.transform.parent = circleField;
			//今ある同階層のオブジェクトよりも上のレイヤーに配置
			fanObj.transform.SetSiblingIndex (0);
			//扇のプロパティ設定
			Fan fan = fanObj.AddComponent<Fan>();
			fan.Initialize (circleImage, data, paddingSize);
			return fan;
		}

		/// <summary>
		/// 円グラフの要素をすべて削除します
		/// 自分自身は削除しません
		/// </summary>
		public void ClearCircle () {
			foreach (GameObject fanObj in fanList.Select(fan => fan.gameObject).ToList()) {
				Destroy (fanObj);
			}
			fanList.Clear ();
		}
	}
}