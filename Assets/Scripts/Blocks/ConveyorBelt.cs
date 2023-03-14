using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConveyorBelt : CBlock {

	public override void Active() {
		if (activeObjs.Count > 0) {
			GameObject neighBour = base.bSys.GetNeighbour(base.coord, base.rotIndex);
			if (neighBour != null && neighBour.GetComponent<CBlock>().doAccept) {
				base.iSys.TransferObj(activeObjs, neighBour.transform);
				activeObjs.Clear();
			}
		}
	}
	public override void ReceiveObjs(List<GameObject> objList) {
		passiveObjs = passiveObjs.Concat(objList).ToList();;
		int overflow = (activeObjs.Count + passiveObjs.Count) - base.pData.cbMaxObjs;
		if (overflow >= 1)
			for (int i = 0; i < overflow; i++) {
				Destroy(activeObjs[0]);
				activeObjs.RemoveAt(0);
			};
	}
	public override void Edit() {
	}
}