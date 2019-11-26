using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NTP : SingletonMonoBehaviour<NTP> {

	static readonly string ntpUrl = "https://ntp-a1.nict.go.jp/cgi-bin/json";

	public void GetTimeStamp (Action<DateTime?> callback) {
		var startTimeStamp = DateTime.Now.TimeStamp ();
		StartCoroutine (Get ((response) => {
			if (response == null) {
				callback (null);
			} else {
				var latency = (DateTime.Now.TimeStamp () - startTimeStamp) / 2.0;
				var timeStamp = response.st + latency;
				var startDate = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
				var unixDate = startDate.AddSeconds (timeStamp);
				callback (unixDate.ToLocalTime ());
			}
		}));
	}

	IEnumerator Get (Action<NictResponse> callback) {
		var req = UnityWebRequest.Get (ntpUrl);
		yield return req.SendWebRequest ();
		if (req.isNetworkError) {
			callback (null);
		} else {
			var response = JsonUtility.FromJson<NictResponse> (req.downloadHandler.text);
			callback (response);
		}
	}

	[Serializable]
	public class NictResponse {
		public string id;
		public double it;
		public double st;
		public int leap;
		public long next;
		public int step;
	}
}
