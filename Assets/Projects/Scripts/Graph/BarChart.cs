using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Graph
{

    public class BarChart : MonoBehaviour
    {

        public RectTransform frame;
        List<Image> barElementList = new List<Image>();

        /// <summary>
        /// バーグラフにデータを設定します
        /// 連続したデータの設定のみ可能です
        /// </summary>
        /// <param name="xValueRateList">X軸の始点を設定したリスト(値は0~1f)</param>
        /// <param name="yValueRateList">Y軸の高さの比率を設定したリスト(値は0~1f)</param>
        /// <param name="labelList">ラベルのリスト</param>
        public void SetData(List<float> xValueRateList, List<float> yValueRateList, List<LabelData.Label> labelList)
        {
            SetBarElements(xValueRateList, yValueRateList, labelList);
        }

        /// <summary>
        /// バーグラフにデータを設定します
        /// 隙間が空くグラフにも対応可能です
        /// </summary>
        /// <param name="xValueRangeList">X軸の始点と終点をまとめたリスト(値は0~1f)</param>
        /// <param name="yValueRateList">Y軸の高さの比率を設定したリスト</param>
        /// <param name="labelList">ラベルのリスト</param>
        public void SetData(List<Vector2> xValueRangeList, List<float> yValueRateList, List<LabelData.Label> labelList)
        {
            //初期化
            ClearBarElements();
            //データをまとめて最適化する
            CleaningData(ref xValueRangeList, ref yValueRateList, ref labelList);
            //バーチャートに要素を設定する
            for (int i = 0; i < xValueRangeList.Count; i++)
            {
                AddBarElement(xValueRangeList[i], yValueRateList[i], labelList[i]);
            }
        }

        //データをまとめて最適化する
        void CleaningData(ref List<Vector2> xValueRangeList, ref List<float> yValueRateList, ref List<LabelData.Label> labelList)
        {
            for (int i = 0; i < xValueRangeList.Count - 1; i++)
            {
                //labelList [i].GetColor ().Equals (labelList [i + 1].GetColor ()) )
                while (labelList[i].GetName().Equals(labelList[i + 1].GetName()) && yValueRateList[i] == yValueRateList[i + 1] && xValueRangeList[i].y == xValueRangeList[i + 1].x)
                {
                    //現在のデータと次のデータが同じであれば、現在のデータを次のデータの終点まで伸ばす
                    xValueRangeList[i] = new Vector2(xValueRangeList[i].x, xValueRangeList[i + 1].y);
                    //次のデータは不要になったため削除。i+2のデータがi+1の位置に移動
                    xValueRangeList.RemoveAt(i + 1);
                    yValueRateList.RemoveAt(i + 1);
                    labelList.RemoveAt(i + 1);
                    if (xValueRangeList.Count - i <= 1)
                        break;
                }
            }
        }

        //バーの高さの割合から実際のスケールを返します
        float GetBarHeight(float heightRatio)
        {
            return frame.rect.height * heightRatio;
        }

        float GetBarWidth(float widthRatio)
        {
            return frame.rect.width * widthRatio;
        }

        /// <summary>
        /// バーチャートに要素を追加します
        /// </summary>
        /// <param name="frame">Frame.</param>
        /// <param name="barData">Bar data.</param>
        public void AddBarElement(Vector2 xRangeRate, float yValueRate, LabelData.Label label)
        {
            var barElement = new GameObject("bar");
            barElement.transform.parent = frame;
            var image = barElement.AddComponent<Image>();
            image.color = label.GetColor();
            //RectTransform初期化
            image.rectTransform.anchorMax = new Vector2(1f, 1f);
            image.rectTransform.anchorMin = new Vector2(0, 0);
            image.rectTransform.offsetMin = new Vector2(0, 0);
            image.rectTransform.offsetMax = new Vector2(0, 0);
            image.rectTransform.localScale = Vector3.one;
            image.rectTransform.localPosition = Vector3.zero;
            //指定の位置・大きさに設定
            //RectTransformのアンカーがストレッチで設定しているため、位置・大きさの指定を以下のようにする
            float left = xRangeRate.x * frame.rect.width;			//親となるフレームの左端からどれだけ離れた位置か
            float right = (1f - xRangeRate.y) * frame.rect.width;	//・・・右端から
            float top = (1f - yValueRate) * frame.rect.height;		//・・・上から
            float bottom = 0;										//・・・下から
            image.rectTransform.offsetMin = new Vector2(left, bottom);
            image.rectTransform.offsetMax = new Vector2(-1f * right, -1f * top);	//-1をかけるのは仕様なので仕方ない
            if (label.IsUseTexture)
            {
                image.sprite = label.Texture;
                image.type = Image.Type.Tiled;
            }
            barElementList.Add(image);
        }

        /// <summary>
        /// バーチャートに要素を設定します
        /// その際、元あった要素は破棄します
        /// </summary>
        /// <param name="barDataList">Bar data list.</param>
        void SetBarElements(List<float> xValueRateList, List<float> yValueRateList, List<LabelData.Label> labelList)
        {
            ClearBarElements();
            for (int i = 0; i < xValueRateList.Count - 1; i++)
            {
                Vector2 xDataRange = new Vector2(xValueRateList[i], xValueRateList[i + 1]);
                AddBarElement(xDataRange, yValueRateList[i], labelList[i]);
            }
        }

        /// <summary>
        /// バーチャート内にあるすべての要素を削除します
        /// </summary>
        public void ClearBarElements()
        {
            if (barElementList == null)
                return;
            foreach (Image element in barElementList)
            {
                Object.Destroy(element.gameObject);
            }
            barElementList.Clear();
        }
    }
}
