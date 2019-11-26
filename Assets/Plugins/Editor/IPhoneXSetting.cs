#if UNITY_IOS

using System;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

/// <summary>
/// Unity iOSビルド後にiPhoneX用の設定を編集する為のPostprocessor
/// </summary>
public class IPhoneXSetting
{

	/// <summary>
	///
	/// </summary>
	/// <param name="target">OS</param>
	/// <param name="path">ビルドの出力パス</param>
	[PostProcessBuild]
	public static void OnPostProcessBuild( BuildTarget target, string path )
	{
		SetStatusBarSetting(path);
	}

	/// <summary>
	/// iPhoneXの場合のみステータスバーを表示させる
	/// </summary>
	/// <param name="path"></param>
	private static void SetStatusBarSetting(string path) 
	{
		// ステータスバーが常に表示の場合はそのまま
		if (!PlayerSettings.statusBarHidden) return;
		
		//Unityで出力されるViewControllerファイルを書き換える
		string viewControllerPath = Path.Combine (path, "Classes/UI/UnityViewControllerBase+iOS.mm");
		string viewControllerContent = File.ReadAllText (viewControllerPath);
		string vcOldText = 
			"    return _PrefersStatusBarHidden;";
		string vcNewText = 
			"    CGSize size = [UIScreen mainScreen].nativeBounds.size;\n" +
			"    if ( (int)size.width == 1125 && (int)size.height == 2436)\n" +
			"    {\n" +
			"        return NO;\n" +
			"    }\n" +
            "    else if ( (int)size.width == 1242 && (int)size.height == 2688)\n" +
            "    {\n" +
            "        return NO;\n" +
            "    }\n" +
            "    else\n" +
			"    {\n" +
			"        return YES;\n" +
			"    }";
		viewControllerContent = viewControllerContent.Replace (vcOldText, vcNewText);
		File.WriteAllText (viewControllerPath, viewControllerContent);
	}

}

#endif