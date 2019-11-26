using System;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

/// <summary>
/// Unity iOSビルド後にinfo.plistを編集する為のPostprocessor
/// </summary>
public class PostProcessBuildPlayer
{
	// ここはアプリ名に合わせて変更すること
	public const string IOS_PROJECT_DIR_NAME = "xcodeproject";

	/// <summary>
	/// ビルド後の処理( Android / iOS共通 )
	/// </summary>
	/// <param name="target">OS</param>
	/// <param name="path">ビルドの出力パス</param>
	[PostProcessBuild(1)]
	public static void OnPostProcessBuild( BuildTarget target, string path )
	{
		if (target != BuildTarget.iOS) return;
		Debug.Log("Xcode!!!!!!!!!!!!");
		// XCode基本設定
		modPBXProject(path);
        AddCapability(path);
		AddPlist(path);

		// 画像をコピー
		CopyImages (path);
	}

	/// <summary>
	/// 使用するFramewaorkを追加します
	/// </summary>
	/// <param name="path">出力先のパス</param>
	public static void modPBXProject( string path )
	{
#if UNITY_IOS
		string projPath = PBXProject.GetPBXProjectPath(path);
		PBXProject proj = new PBXProject ();

		proj.ReadFromString (File.ReadAllText (projPath));
		string target = proj.TargetGuidByName ("Unity-iPhone");

		// プロビジョニングファイルの設定
		// 開発機ビルド用


		// Swiftプラグインとの連携に必要な設定
		proj.SetBuildProperty(target, "SWIFT_OBJC_BRIDGING_HEADER", "$(SRCROOT)/Libraries/Plugins/iOS/KaiminApp-Bridging-Header.h");
		proj.SetBuildProperty(target, "SWIFT_VERSION", "4.1");
		proj.SetBuildProperty(target, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
		proj.SetBuildProperty(target, "LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks");

		// 書き出し
		File.WriteAllText(projPath, proj.WriteToString());
#endif
	}

    public static void AddPlist(string path)
    {
#if UNITY_IOS
        var plistPath   = Path.Combine( path, "Info.plist" );
        var plist       = new PlistDocument();

        // 読み込み
        plist.ReadFromFile( plistPath );

		PlistElementArray capabilities;
		if (plist.root.values.ContainsKey("UIRequiredDeviceCapabilities")) {
			capabilities = plist.root["UIRequiredDeviceCapabilities"].AsArray();
		} else {
			capabilities = plist.root.CreateArray ("UIRequiredDeviceCapabilities");
		}
		capabilities.AddString("bluetooth-le");

		PlistElementArray backGroundModes;
		if (plist.root.values.ContainsKey("UIBackgroundModes")) {
			backGroundModes = plist.root["UIBackgroundModes"].AsArray();
		} else {
			backGroundModes = plist.root.CreateArray ("UIBackgroundModes");
		}
		backGroundModes.AddString("audio");
		backGroundModes.AddString("bluetooth-central");

        // 書き込み
        plist.WriteToFile(plistPath);
#endif
    }

    public static void AddCapability(string path)
    {
#if UNITY_IOS
        string projPath = PBXProject.GetPBXProjectPath(path);
        PBXProject proj = new PBXProject ();
        proj.ReadFromString(File.ReadAllText(projPath));

        string target = proj.TargetGuidByName ("Unity-iPhone");

        proj.AddCapability(target, PBXCapabilityType.BackgroundModes);
        File.WriteAllText(projPath, proj.WriteToString());
#endif
    }

	private static void CopyImages(string xcodeProjectPath)
	{
		var destDirName = Path.Combine(xcodeProjectPath, "Unity-iPhone/Images.xcassets/");
		var path = Application.dataPath;
		path = path + "/Plugins/iOS";
		var sourceDirNames = Directory.GetDirectories(path, "*.dataset", SearchOption.AllDirectories);
		Debug.Log("CopyImages");
		Debug.Log(sourceDirNames.Length);
		foreach(var sourceDirName in sourceDirNames)
		{
			var dirName = Path.GetFileName(sourceDirName);
			CopyDirectory(sourceDirName, Path.Combine(destDirName, dirName));
		}
	}
		
	public static void CopyDirectory(string sourceDirName, string destDirName)
	{
		//コピー先のディレクトリがないときは作る
		if (!System.IO.Directory.Exists(destDirName))
		{
			System.IO.Directory.CreateDirectory(destDirName);
			//属性もコピー
			System.IO.File.SetAttributes(destDirName,
				System.IO.File.GetAttributes(sourceDirName));
		}

		//コピー先のディレクトリ名の末尾に"\"をつける
		if (destDirName[destDirName.Length - 1] !=
			System.IO.Path.DirectorySeparatorChar)
			destDirName = destDirName + System.IO.Path.DirectorySeparatorChar;

		//コピー元のディレクトリにあるファイルをコピー
		string[] files = System.IO.Directory.GetFiles(sourceDirName);
		foreach (string file in files)
		{
			if(file.EndsWith(".meta")) continue; // metaファイルはコピーしない.
			System.IO.File.Copy(file, destDirName + System.IO.Path.GetFileName(file), true);
		}

		//コピー元のディレクトリにあるディレクトリについて、再帰的に呼び出す
		string[] dirs = System.IO.Directory.GetDirectories(sourceDirName);
		foreach (string dir in dirs)
			CopyDirectory(dir, destDirName + System.IO.Path.GetFileName(dir));
	}
}
