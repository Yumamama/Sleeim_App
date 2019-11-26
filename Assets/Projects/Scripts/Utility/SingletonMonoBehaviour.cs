using UnityEngine;
/// <summary>
/// MonoBehaviourを継承したシングルトン
/// https://qiita.com/valbeat/items/4b0175755b0cc47d4f6e
/// </summary>
public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    /// インスタンス
    /// </summary>
    private static volatile T instance;

    /// <summary>
    /// 同期オブジェクト
    /// </summary>
    private static object syncObj = new object();

    /// <summary>
    /// インスタンスのgetter/setter
    /// </summary>
    public static T Instance
    {
        get
        {
            // アプリ終了時に，再度インスタンスの呼び出しがある場合に，オブジェクトを生成することを防ぐ
            if (applicationIsQuitting)
            {
                return null;
            }
            // インスタンスがない場合に探す
            if (instance == null)
            {
                instance = FindObjectOfType<T>() as T;

                // 複数のインスタンスがあった場合
                if (FindObjectsOfType<T>().Length > 1)
                {
                    return instance;
                }

                // Findで見つからなかった場合、新しくオブジェクトを生成
                if (instance == null)
                {
                    // 同時にインスタンス生成を呼ばないためにlockする
                    lock (syncObj)
                    {
                        GameObject singleton = new GameObject();
						//シーン遷移で消えないようにする (追加)
						DontDestroyOnLoad (singleton);
                        // シングルトンオブジェクトだと分かりやすいように名前を設定
                        singleton.name = typeof(T).ToString() + " (singleton)";
                        instance = singleton.AddComponent<T>();
                        // シーン変更時に破棄させない
                        DontDestroyOnLoad(singleton);
                    }
                }

            }
            return instance;
        }
        // インスタンスをnull化するときに使うのでprivateに
        private set
        {
            instance = value;
        }
    }

    /// <summary>
    /// アプリが終了しているかどうか
    /// </summary>
    static bool applicationIsQuitting = false;

    void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }

    void OnDestroy()
    {
        Instance = null;
    }

	public virtual void OnInitialize () { }

    // コンストラクタをprotectedにすることでインスタンスを生成出来なくする
    protected SingletonMonoBehaviour() { }

    protected virtual void OnApplicationPause(bool pause){ }

	public string objectName{ get{ return instance.gameObject.name; } }
}