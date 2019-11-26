using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Projects.Scripts.Graph
{
    public class GraphLayout : MonoBehaviour
    {
        /// <summary>
        /// とりあえずホームに戻る用に
        /// ボタンから呼び出される
        /// </summary>
        public void ChangeBackHome()
        {
            SceneManager.LoadSceneAsync("Home");
        }

    }
}
