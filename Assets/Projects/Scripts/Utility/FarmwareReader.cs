using DG.Tweening.Core;
using Kaimin.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using UnityEngine;

/// <summary>
/// ファームウェア管理クラス
/// </summary>{
class FirmwareReader
{
    private const string _codeTypeRecord = "S3"; //コードレコードタイプ
    private const string _IdentifyingCode = "524438303031"; //識別コード ASCIIでRD8001(本製品名)
    private const int _startAddress = 0x00007000; //アプリケーション開始アドレス（～00007000はブート領域）
    private const int _endAddress = 0x0001FBF0; //アプリケーション終了アドレス（0001FBF0は識別コード領域のため、実際は0x0001FBEFまでがアプリケーション領域）
    private static string FirmwareChecksumG1d = null;
    private static List<byte[]> _FarmwareCodeDataG1d = null;

    /// <summary>
    /// G1Dプログラムファイルの識別コードをチェックする
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns> bool :識別コードが正しいならtrue</returns>
    public static Boolean FirmwareFileCheckG1d(string filePath)
    {
        try
        {
            // ファイル共有モードで開く
            using (FileStream fs = new FileStream(filePath,
                FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var reversedLines = File.ReadAllLines(@filePath).Reverse();
                foreach (var line in reversedLines)
                {
                    // コードレコードのみをチェック対象とする
                    if ((line.Substring(0, 2) == _codeTypeRecord))
                    {
                        // 同一レコードにサム値(4byte)と識別コード(6byte)は必ず入ります。
                        // 7byte～22byteがロード・アドレス部分
                        // 識別コードとしては最短11byteからはじまる

                        String _LoadAreaAdr = line.Substring(4, 8); // 4byteを抽出(アドレス)
                        String _IdArea = line.Substring(20, 24);    // 識別コードが存在しうる12byteを抽出
                        String _loadArea = line.Substring(12, 32);  // ロードアドレス部分を抽出

                        uint readAddress = uint.Parse(_LoadAreaAdr, System.Globalization.NumberStyles.AllowHexSpecifier);

                        if (_endAddress == readAddress) //識別エリアのみチェック
                        {
                            if (_IdArea.Contains(_IdentifyingCode))
                            {
                                int _positionId = _loadArea.IndexOf(_IdentifyingCode); //識別コード開始位置

                                if ((_positionId - 8) >= 0)
                                {
                                    FirmwareChecksumG1d = _loadArea.Substring(_positionId - 8, 8); //サム値を抽出(開始位置-8)

                                    return true; //存在する場合
                                }
                            }
                            else
                            {
                                return false; //存在しない場合
                            }
                        }
                    }

                    Debug.Log("コードレコードエラー");
                }
                return false; //存在しない場合
            }
        }
        catch (SecurityException e)
        {
            Debugger.LogError("呼び出し元に、必要なアクセス許可がありません。:" + e.Message);
            return false;

        }
        catch (FileNotFoundException e)
        {
            Debugger.LogError("path が指定したファイルが存在しません。 :" + e.Message);
            return false;
        }
        catch (IOException e)
        {
            Debugger.LogError("I/O エラーが発生しました。" + e.Message);
            return false;
        }
        catch (ArgumentOutOfRangeException e)
        {
            Debugger.LogError("ArgumentOutOfRangeException" + e.Message);
            return false;
        }
        catch (Exception e)
        {
            Debugger.LogError(e.Message);
            return false;
        }
    }

    /// <summary>
    /// H1Dプログラムファイルの転送用コードを作成
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns> bool :正常終了</returns>
    public static Boolean FirmwareFileTransCreateG1d(string filePath)
    {
        try
        {
            _FarmwareCodeDataG1d = new List<byte[]>();
            _FarmwareCodeDataG1d.Clear();

            // ファイル共有モードで開く
            using (FileStream fs = new FileStream(filePath,
            FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var readLines = File.ReadAllLines(@filePath);
                foreach (var line in readLines)
                {    //コードレコードのみをチェック対象とする
                    if ((line.Substring(0, 2) == _codeTypeRecord))
                    {
                        String _LoadAreaAdr = line.Substring(4, 8); //転送領域の4byteを抽出(アドレス)
                        String _LoadAreaCode = line.Substring(12, 32); //転送領域の16byteを抽出(コード)

                        uint readAddress = uint.Parse(_LoadAreaAdr, System.Globalization.NumberStyles.AllowHexSpecifier);

                        if ((_startAddress<= readAddress)&&(readAddress < _endAddress)){ //アプリケーション領域のみ読込

                            byte[] data1 = Utility.StringToBytes(_LoadAreaAdr);
                            byte[] data2 = Utility.StringToBytes(_LoadAreaCode);
                            byte[] result = Enumerable.Concat(data1, data2).ToArray();

                            _FarmwareCodeDataG1d.Add(result);
                        }
                    }
                }
                return true;
            }
        }
        catch (SecurityException e)
        {
            Debugger.LogError("呼び出し元に、必要なアクセス許可がありません。:" + e.Message);
            return false;

        }
        catch (FileNotFoundException e)
        {
            Debugger.LogError("path が指定したファイルが存在しません。 :" + e.Message);
            return false;
        }
        catch (IOException e)
        {
            Debugger.LogError("I/O エラーが発生しました。" + e.Message);
            return false;
        }
        catch (ArgumentOutOfRangeException e)
        {
            Debugger.LogError("ArgumentOutOfRangeException" + e.Message);
            return false;
        }
        catch (ArgumentException e)
        {
            Debugger.LogError("ArgumentException " + e.Message);
            return false;
        }
    }

    /// <summary>
    /// H1Dプログラムファイルのサム値を返却
    /// </summary>
    /// <returns> byte[] :H1Dチェックサム値</returns>
    ///
    public static byte[] FirmwareFileGetSumG1d()
    {
        byte[] bytes = Utility.StringToBytes(FirmwareChecksumG1d);

        //Debug.Log(BitConverter.ToString(bytes)); //デバッグ用ログ

        //byte[] rBytes = Utility.Reverse(bytes, Utility.Endian.Big); //反転処理は不要に変更

        //Debug.Log(BitConverter.ToString(rBytes)); //デバッグ用ログ

        return bytes;
    }

    /// <summary>
    /// G1Dプログラムファイルの転送用コードを返却
    /// </summary>
    /// <returns> string :G1Dチェックサム値</returns>
    ///
    public static List<byte[]> FirmwareFileGetTransCreatG1d()
    {
        return _FarmwareCodeDataG1d;
    }
}
