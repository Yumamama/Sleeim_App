using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Graph {
	public class CustomCircleGraph : MonoBehaviour {

		public Circle circle;
		public Circle innerCircle;

		void Start () {
			var testData = CreateTestDataList (1);
			circle.Create (testData);
			innerCircle.Create (testData);
		}

		void Update () {
			if (Input.GetKeyDown (KeyCode.Space)) {
				circle.AddElement (new Fan.Data (GetRandomValue (), GetRandomColor ()));
				innerCircle.AddElement (new Fan.Data (GetRandomValue (), GetRandomColor ()));
			}
		}

		List<Fan.Data> CreateTestDataList (int count) {
			List<Fan.Data> dataList = new List<Fan.Data> ();
			for (int i = 0; i < count; i++) {
				dataList.Add (new Fan.Data(GetRandomValue(),GetRandomColor()));
			}
			return dataList;
		}

		float GetRandomValue () {
			return 1f;
		}

		Color GetRandomColor () {
			Color color = new Color (Random.Range (0, 1f), Random.Range (0, 1f), Random.Range (0, 1f), 1f);
			return color;
		}
	}
}