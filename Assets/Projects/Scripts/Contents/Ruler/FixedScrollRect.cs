/**
 * @file
 * @brief InfiniteScrollを一定間隔で止まるように拡張するスクリプト
 */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// InfiniteScrollを一定間隔で止まるように拡張する
/// http://kohki.hatenablog.jp/entry/Unity-uGUI-Fixed-Scroll-Rect
/// </summary>
public class FixedScrollRect : ScrollRect
{
	// 戻る速度係数
	private const float POWER = 30;

	// アイテム静止位置 微調整値
	private const float ANCHOR_SHIFT = -4;
	private const float ANCHOR_SHIFT_FORCE = 1;

	private bool isForceSetAnchoredPosition = false;

	private float defaultdecelerationRate;
	private float defaultelasticity;

	private InfiniteScroll _infinityScroll;
	private InfiniteScroll infinityScroll {
		get {
			if (_infinityScroll == null)
				_infinityScroll = GetComponentInChildren<InfiniteScroll> ();
			return _infinityScroll;
		}
	}

	/// <summary>
	/// ドラッグ中か否か
	/// </summary>
	public bool isDrag
	{
		get;
		private set;
	}

	private float Velocity {
		get {
			return  (infinityScroll.direction == InfiniteScroll.Direction.Vertical) ? 
				-velocity.y :
				velocity.x;
		}
	}

	private RectTransform _rectTransform;
	private float AnchoredPosition {
		get {
			if (_rectTransform == null)
				_rectTransform = transform.GetChild (0).GetComponent<RectTransform> ();
			return  (infinityScroll.direction == InfiniteScroll.Direction.Vertical ) ? 
				-_rectTransform.anchoredPosition.y:
				_rectTransform.anchoredPosition.x;
		}
		set{
			if (_rectTransform == null)
				_rectTransform = transform.GetChild(0).GetComponent<RectTransform>();
			if (infinityScroll.direction == InfiniteScroll.Direction.Vertical)
				_rectTransform.anchoredPosition = new Vector2 (0, -value);
			else
				_rectTransform.anchoredPosition =  new Vector2 (value,0);
		}
	}

	override protected void Awake()
	{
		defaultdecelerationRate = decelerationRate;
		defaultelasticity = elasticity;

	}

	void Update()
	{
		// 操作中または速度が一定以上の場合は何もしない
		if (isDrag || Mathf.Abs (Velocity) > 200)
		{
			if (isForceSetAnchoredPosition)
			{
				isForceSetAnchoredPosition = false;
				this.movementType = MovementType.Elastic;
				this.decelerationRate = defaultdecelerationRate;
				this.elasticity = defaultelasticity;
			}
			return;
		}

		if (isForceSetAnchoredPosition)
		{
			return;
		}

		// 基準位置との差分を算出
		float diff = (AnchoredPosition % infinityScroll.itemScale) + (infinityScroll.itemScale / 2) + ANCHOR_SHIFT;

		if(Mathf.Abs(diff) < 2)
		{
			// 差分が一定以下になったら何もしない
		}
		else if (Mathf.Abs (diff) > infinityScroll.itemScale / 2)
		{
			var adjust = infinityScroll.itemScale * ((AnchoredPosition > 0f) ? 1 : -1);
			// #24777 powerが大きくなりすぎてターゲットから離れてしまわないよう調整
			float power = Time.deltaTime * POWER;
			if (power > 1) power = 1f;
			AnchoredPosition += (adjust - diff) * power;
		}
		else
		{
			float power = Time.deltaTime * POWER;
			if (power > 1) power = 1f;
			AnchoredPosition -= diff * power;
		}
	}

	/// <summary>
	/// 現在位置強制設定
	/// </summary>
	/// <param name="itemCount">調整位置</param>
	public void SetAnchoredPosition(int itemCount)
	{
		isForceSetAnchoredPosition = true;
		// 強制位置設定（0基準で算出）
		this.decelerationRate = 0;
		this.elasticity = 100;
		AnchoredPosition = 0 - (infinityScroll.itemScale * itemCount) + (infinityScroll.itemScale / 2) + ANCHOR_SHIFT_FORCE;
		//Debug.Log(AnchoredPosition);
	}

	public override void OnBeginDrag(PointerEventData eventData){
		base.OnBeginDrag (eventData);	// 削除した場合、挙動に影響有り
		isDrag = true;
	}

	public override void OnEndDrag(PointerEventData eventData){
		base.OnEndDrag (eventData);	// 削除した場合、挙動に影響有り
		isDrag = false;
	}

}
