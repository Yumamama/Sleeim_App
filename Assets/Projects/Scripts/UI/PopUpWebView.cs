using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class PopUpWebView : MonoBehaviour {

	public string Url;
	WebViewObject webViewObject;

	public int webViewLeftMargin;
	public int webViewTopMargin;
	public int webViewRightMargin;
	public int webViewDownMargin;

	#if UNITY_IOS
	[DllImport("__Internal")]
	private extern static void GetSafeAreaImpl(out float x, out float y, out float w, out float h);
	#endif

	private Rect GetSafeArea()
	{
		float x, y, w, h;
		#if UNITY_IOS
		var device = UnityEngine.iOS.Device.generation;
		if (device == UnityEngine.iOS.DeviceGeneration.iPhoneX) {
			GetSafeAreaImpl(out x, out y, out w, out h);
		} else {
			x = 0;
			y = 0;
			w = Screen.width;
			h = Screen.height;
		}
		#else
		x = 0;
		y = 0;
		w = Screen.width;
		h = Screen.height;
		#endif
		return new Rect(x, y, w, h);
	}

    IEnumerator Start()
    {
        webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();
        webViewObject.Init(
            cb: (msg) =>
            {
                Debug.Log(string.Format("CallFromJS[{0}]", msg));
            },
            err: (msg) =>
            {
                Debug.Log(string.Format("CallOnError[{0}]", msg));
            },
            started: (msg) =>
            {
                Debug.Log(string.Format("CallOnStarted[{0}]", msg));
            },
            ld: (msg) =>
            {
                Debug.Log(string.Format("CallOnLoaded[{0}]", msg));
#if UNITY_EDITOR_OSX || !UNITY_ANDROID
                // NOTE: depending on the situation, you might prefer
                // the 'iframe' approach.
                // cf. https://github.com/gree/unity-webview/issues/189
#if true
                webViewObject.EvaluateJS(@"
                  if (window && window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.unityControl) {
                    window.Unity = {
                      call: function(msg) {
                        window.webkit.messageHandlers.unityControl.postMessage(msg);
                      }
                    }
                  } else {
                    window.Unity = {
                      call: function(msg) {
                        window.location = 'unity:' + msg;
                      }
                    }
                  }
                ");
#else
                webViewObject.EvaluateJS(@"
                  if (window && window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.unityControl) {
                    window.Unity = {
                      call: function(msg) {
                        window.webkit.messageHandlers.unityControl.postMessage(msg);
                      }
                    }
                  } else {
                    window.Unity = {
                      call: function(msg) {
                        var iframe = document.createElement('IFRAME');
                        iframe.setAttribute('src', 'unity:' + msg);
                        document.documentElement.appendChild(iframe);
                        iframe.parentNode.removeChild(iframe);
                        iframe = null;
                      }
                    }
                  }
                ");
#endif
#endif
                webViewObject.EvaluateJS(@"Unity.call('ua=' + navigator.userAgent)");
            },
            //ua: "custom user agent string",
            enableWKWebView: true);
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        webViewObject.bitmapRefreshCycle = 1;
#endif
		var safeArea = GetSafeArea ();
		Debug.Log ("ScreenSize:" + "w_" + Screen.width + ",h_" + Screen.height);
		Debug.Log ("SafeArea:" + "width_" + safeArea.width + ",height_" + safeArea.height + ",x_" + safeArea.x + ",y_" + safeArea.y);
		webViewObject.SetMargins(
            TransCanvasScalerResolutionToScreenResolution (webViewLeftMargin) + (int)(((Screen.width - safeArea.width) / 2)),
            TransCanvasScalerResolutionToScreenResolution (webViewTopMargin) + (int)(((Screen.height - safeArea.height) / 2)),
            TransCanvasScalerResolutionToScreenResolution (webViewRightMargin) + (int)(((Screen.width - safeArea.width) / 2)),
            TransCanvasScalerResolutionToScreenResolution (webViewDownMargin) + (int)(((Screen.height - safeArea.height) / 2)));
		webViewObject.SetVisibility(true);
#if !UNITY_WEBPLAYER
        if (Url.StartsWith("http")) {
            webViewObject.LoadURL(Url.Replace(" ", "%20"));
        } else {
            var exts = new string[]{
                ".jpg",
                ".js",
                ".html"  // should be last
            };
            foreach (var ext in exts) {
                var url = Url.Replace(".html", ext);
                var src = System.IO.Path.Combine(Application.streamingAssetsPath, url);
                var dst = System.IO.Path.Combine(Application.persistentDataPath, url);
                byte[] result = null;
                if (src.Contains("://")) {  // for Android
                    var www = new WWW(src);
                    yield return www;
                    result = www.bytes;
                } else {
                    result = System.IO.File.ReadAllBytes(src);
                }
                System.IO.File.WriteAllBytes(dst, result);
                if (ext == ".html") {
                    webViewObject.LoadURL("file://" + dst.Replace(" ", "%20"));
                    break;
                }
            }
        }
#else
        if (Url.StartsWith("http")) {
            webViewObject.LoadURL(Url.Replace(" ", "%20"));
        } else {
            webViewObject.LoadURL("StreamingAssets/" + Url.Replace(" ", "%20"));
        }
        webViewObject.EvaluateJS(
            "parent.$(function() {" +
            "   window.Unity = {" +
            "       call:function(msg) {" +
            "           parent.unityWebView.sendMessage('WebViewObject', msg)" +
            "       }" +
            "   };" +
            "});");
#endif
        yield break;
    }

	void CloseWindow () {
		//WebView削除
		webViewObject.SetVisibility (false);
		Destroy (this.gameObject);
	}

	//CanvasScalerの解像度からScreenResolutionに変換
	int TransCanvasScalerResolutionToScreenResolution (int canvasScalerResolution) {
		var canvas = FindObjectOfType <Canvas> ();
		var canvasScaler = canvas.GetComponent <CanvasScaler> ();
		float factor = Screen.currentResolution.height / canvasScaler.referenceResolution.y;	//CanvasScalerの解像度からScreenResolutionに変換するための係数
		Debug.Log ("TransCanvasToScreen:" + canvasScalerResolution + " → " + ((int)(canvasScalerResolution * factor)));
		return (int)(canvasScalerResolution * factor);
	}
}
