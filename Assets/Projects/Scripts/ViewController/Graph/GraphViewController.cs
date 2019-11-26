using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphViewController : ViewControllerBase {

	protected override void Start () {
		base.Start ();
	}

	public override SceneTransitionManager.LoadScene SceneTag {
		get {
			return SceneTransitionManager.LoadScene.Graph;
		}
	}
}
