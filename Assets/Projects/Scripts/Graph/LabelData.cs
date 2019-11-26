using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graph {
	[System.Serializable]
	/// <summary>
	/// 実データとラベルをまとめたデータクラス
	/// グラフのもつデータとして使用します
	/// </summary>
	public class LabelData {

		[SerializeField] float value;
		[SerializeField] Label label;

		public LabelData (float value, Label label) {
			this.value = value;
			this.label = label;
		}
			
		[System.Serializable]	//インスペクタに表示したかったため必要
		/// <summary>
		/// 何のデータか分かるようにするために名前と色をラベルとしてまとめたクラス
		/// </summary>
		public class Label {
			[SerializeField]
			string name;
			[SerializeField]
			Color color;
			[SerializeField]
			bool isUseTexture;
			[SerializeField]
			Sprite texture;

			public Label (string name, Color color) {
				this.name = name;
				this.color = color;
			}
			/// <summary>
			/// ラベル名を取得します
			/// </summary>
			public string GetName () {
				return name;
			}
			/// <summary>
			/// ラベル色を取得します
			/// </summary>
			public Color GetColor () {
				return color;
			}

			/// <summary>
			/// テクスチャを使用するかどうか
			/// </summary>
			/// <value><c>true</c> if this instance is use texture; otherwise, <c>false</c>.</value>
			public bool IsUseTexture {
				get {
					return isUseTexture;
				}
			}

			/// <summary>
			/// 使用するテクスチャ
			/// </summary>
			/// <value>The texture.</value>
			public Sprite Texture {
				get {
					return texture;
				}
			}
		}

		/// <summary>
		/// 実データを取得します
		/// </summary>
		public float GetValue () {
			return value;
		}

		/// <summary>
		/// ラベル(名前と色をまとめたもの)を取得します
		/// </summary>
		public Label GetLabel () {
			return label;
		}
	}
}
