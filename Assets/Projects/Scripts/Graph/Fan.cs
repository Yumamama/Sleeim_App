using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Graph {
	public class Fan : MonoBehaviour {

		Data data;
		GameObject mask;	//扇状にクリッピングするためのオブジェクト
		GameObject panel;	//扇の中身となるオブジェクト
		float padding;

		public class Data {
			float value;
			Color color;

			public Data (float value, Color color) {
				this.value = value;
				this.color = color;
			}

			public float GetValue () {
				return this.value;
			}

			public Color GetColor () {
				return this.color;
			}
		}

		void Awake () {
			BuildFanObjects (out mask, out panel);
		}

		//自分以外のインスタンスのデータにアクセスするために必要
		public Data MyData {
			get {
				return data;
			}
		}

		/// <summary>
		/// 初期化します。
		/// </summary>
		/// <param name="clippingImage">Clipping image.</param>
		public void Initialize (Sprite clippingImage, Data data, float padding = 0) {
			SetClippingImage (clippingImage);
			this.data = data;
			this.padding = padding;
		}

		//データの追加や変更が発生した際に、自分自身の全体に対する割合の変化に伴い見た目の更新を行う
		public void UpdateVisual (int index, List<Fan> fanList) {
			SetColor (this.data.GetColor ());
			float wholeValue = fanList.Select (fan => fan.MyData.GetValue()).Sum ();
			float ownAngleRate = this.data.GetValue () / wholeValue;
			float otherAngleRate = fanList.Take (index).Select (fan => fan.MyData.GetValue ()).Sum () / wholeValue;
			float angleRate = ownAngleRate + otherAngleRate;
			SetPadding (fanList.Count > 1 ? this.padding : 0, otherAngleRate, angleRate);
			SetFanRotation (otherAngleRate);
			SetFanAngleSize (ownAngleRate);
		}

		/// <summary>
		/// データの追加や変更が発生した際に、自分自身の全体に対する割合の変化に伴い見た目の更新を行う
		/// 扇をグループに分け、そのグループごとにパディングを設定したい場合に使用する
		/// </summary>
		/// <param name="unitIndex">何番目のグループか</param>
		/// <param name="index">グループの中で何番目か</param>
		/// <param name="fanList">Fan list.</param>
		public void UpdateVisual (int unitIndex, int index, List<List<Fan>> fanList) {
			SetColor (this.data.GetColor ());

			float wholeValue = fanList.Select (fanUnit => fanUnit.Select (fan => fan.MyData.GetValue ()).Sum ()).Sum ();
			float ownAngleRate = this.data.GetValue () / wholeValue;
			float ownUnitAngle = fanList [unitIndex].Select (fan => fan.MyData.GetValue ()).Sum ();
			float ownUnitAngleRate = ownUnitAngle / wholeValue;
			float otherUnitAngle = fanList.Take (unitIndex).Select (fanUnit => fanUnit.Select (fan => fan.MyData.GetValue ()).Sum ()).Sum ();
			float otherUnitAngleRate = otherUnitAngle / wholeValue;
			float otherFanAngle = fanList [unitIndex].Take (index).Select (fan => fan.MyData.GetValue ()).Sum ();
			float otherAngle = otherUnitAngle + otherFanAngle;
			float otherAngleRate = otherAngle / wholeValue;
			SetPadding (fanList.Count > 1 ? this.padding : 0, otherUnitAngleRate, otherUnitAngleRate + ownUnitAngleRate);
			SetFanRotation (otherAngleRate);
			SetFanAngleSize (ownAngleRate);
		}

		//オブジェクト構造を構築する
		void BuildFanObjects (out GameObject mask, out GameObject panel) {
			//クリッピング用のマスクオブジェクト設定
			var maskObj = this.gameObject;
			var maskImage = maskObj.AddComponent<Image> ();
			//RectTransform初期化
			maskImage.rectTransform.anchorMax = new Vector2 (1f, 1f);
			maskImage.rectTransform.anchorMin = new Vector2 (0, 0);
			maskImage.rectTransform.offsetMin = new Vector2 (0,0);
			maskImage.rectTransform.offsetMax = new Vector2 (0,0);
			maskImage.rectTransform.localScale = Vector3.one;
			maskImage.rectTransform.localPosition = Vector3.zero;
			Mask m = maskObj.AddComponent<Mask> ();
			m.showMaskGraphic = false;
			mask = maskObj;
			//扇の中身となるパネルオブジェクト設定
			var panelObj = new GameObject ("panel");
			//パネルオブジェクトがクリッピングされるよう、マスクオブジェクトの子にせてい
			panelObj.transform.parent = maskImage.rectTransform;
			var panelImage = panelObj.AddComponent<Image> ();
			//RectTransform初期化
			panelImage.rectTransform.anchorMax = new Vector2 (1f, 1f);
			panelImage.rectTransform.anchorMin = new Vector2 (0, 0);
			panelImage.rectTransform.offsetMin = new Vector2 (0,0);
			panelImage.rectTransform.offsetMax = new Vector2 (0,0);
			panelImage.rectTransform.localScale = Vector3.one * 2f;		//マスクよりも十分に大きなサイズにしておく
			panelImage.rectTransform.localPosition = Vector3.zero;
			panelObj.AddComponent<Mask> ();
			panel = panelObj;
		}

		//自分自身の扇型の角度を設定する
		void SetFanAngleSize (float angleRate) {
			Image panelImage = this.panel.GetComponent<Image> ();
			//パネルのClipping設定
			panelImage.type = Image.Type.Filled;
			panelImage.fillMethod = Image.FillMethod.Radial360;
			panelImage.fillOrigin = (int)Image.Origin360.Top;
			panelImage.fillAmount = Mathf.Clamp01 (angleRate);
		}

		//回転値を設定する
		void SetFanRotation (float rotRate) {
			if (rotRate == 0)
				return;	//0の場合エラーになるため回避
			float dirAngle = rotRate * 360f;
			this.panel.transform.localRotation = Quaternion.Euler (0, 0, -1f * dirAngle);	//時計周りに回転させるために-1をかける必要がある
		}
			
		//扇型の要素と要素の間の隙間を設定する
		void SetPadding (float paddingSize, float startAngleRate, float endAngleRate) {
			Vector2 moveDir = CulcFanDir (startAngleRate * 360f, endAngleRate * 360f);
			UpdateLocalPosition (paddingSize * moveDir);
			//隙間を作って大きくなった分のサイズを扇の径を変更することで調整する
			ChangeFanSize (-1f * paddingSize);
		}

		//扇型の要素と要素の間の隙間を等間隔に設定します
		void SetUniformPadding (float paddingSize, int index, List<Fan.Data> fanDataList) {
			float wholeValue = fanDataList.Select (fan => fan.GetValue()).Sum ();
			float ownAngleRate = this.data.GetValue () / wholeValue;
			float otherAngleRate = fanDataList.Take (index).Select (fan => fan.GetValue ()).Sum () / wholeValue;
			float angleRate = ownAngleRate + otherAngleRate;
			Vector2 moveDir = CulcFanDir (otherAngleRate * 360f, angleRate * 360f);
			Vector2 prevMoveDir;
			if (fanDataList.Count == 1) {
				prevMoveDir = new Vector2 (0, 1f);
			} else {
				index = index == 0 ? fanDataList.Count - 1 : index - 1;
				ownAngleRate = fanDataList [index].GetValue () / wholeValue;
				otherAngleRate = fanDataList.Take (index).Select (fan => fan.GetValue ()).Sum () / wholeValue;
				prevMoveDir = CulcFanDir (otherAngleRate * 360f, angleRate * 360f);
			}
			float angleDiff = Mathf.Abs(Vector3.Angle (prevMoveDir, moveDir));
			float angleFactor = 1f / (((int)(angleDiff / 90f) + Mathf.Sin ((angleDiff % 90f) * Mathf.Deg2Rad)) <= 0 ? 0.1f : ((int)(angleDiff / 90f) + Mathf.Sin ((angleDiff % 90f) * Mathf.Deg2Rad)));
			paddingSize *= angleFactor;
			UpdateLocalPosition (paddingSize * moveDir);
			//隙間を作って大きくなった分のサイズを扇の径を変更することで調整する
			ChangeFanSize (-1f * paddingSize);
		}

		//扇のサイズを変更する
		//元の大きさから指定した値だけ半径を大きく(小さくする)
		void ChangeFanSize (float diff) {
			RectTransform rect = this.mask.GetComponent<RectTransform> ();
			//パディングで移動した分を加味する
			Vector2 positionDiff = new Vector2 (rect.localPosition.x, rect.localPosition.y);
			//offsetMinとoffsetMaxでdiffの符号が逆になってるのはRectTransformの仕様
			rect.offsetMin = new Vector2 (positionDiff.x - diff, positionDiff.y - diff);
			rect.offsetMax = new Vector2 (positionDiff.x + diff, positionDiff.y + diff);
		}

		//扇型の中心から外向きに二等分する方向ベクトル
		//angle:時計の0時を0とし、時計回りに大きくなっていく(単位：度)
		Vector2 CulcFanDir (float startAngle, float endAngle) {
			float middleAngle = (startAngle + endAngle) / 2f;	//扇型を二等分する角度に変換する
			float xVal = Mathf.Sin (middleAngle * Mathf.Deg2Rad);
			float yVal = Mathf.Cos (middleAngle * Mathf.Deg2Rad);
			return new Vector2 (xVal, yVal).normalized;
		}

		//扇の位置を更新する
		void UpdateLocalPosition (Vector2 pos) {
			//回転を無視して設定できるようにする
			Vector3 positionDiff = new Vector3 (pos.x, pos.y, 0);
			this.panel.transform.localPosition = positionDiff;
		}

		//何色で表示するか設定する
		void SetColor (Color color) {
			Image panelImage = this.panel.GetComponent<Image> ();
			panelImage.color = color;
		}

		//クリッピングしたい形の画像を設定します
		void SetClippingImage (Sprite image) {
			Image maskImage = this.mask.GetComponent<Image> ();
			Image panelImage = this.panel.GetComponent<Image> ();
			maskImage.sprite = image;
			panelImage.sprite = image;
		}
	}
}