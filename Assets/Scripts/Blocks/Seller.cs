using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seller : CBlock {
	GameObject cannon;
	public override void OnDeployment() {
		cannon = transform.GetChild(0).gameObject;
		cannon.transform.position += transform.up * .5f;
	}
	public override void Active() {
		if (activeObjs.Count == 0) return;
		foreach (var obj in activeObjs) {
			Debug.Log($"{obj.name} sold");
			Destroy(obj);
		}
	}
	public override void Edit() {
	}
}