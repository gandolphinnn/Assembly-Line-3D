using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buyer : CBlock {
	int matId = 2;
	public override void OnDeployment() {
		doAccept = false;
	}
	//? no passive needed
	public override void Active() {
		if (activeObjs.Count == 0)
			activeObjs.Add(base.iSys.Spawn(1, transform, base.rotIndex, matId));
		else {
			GameObject neighBour = base.bSys.GetNeighbour(base.coord, base.rotIndex);
			if (neighBour != null && neighBour.GetComponent<CBlock>().doAccept) {
				base.iSys.TransferObj(activeObjs, neighBour.transform);
				activeObjs[0] = base.iSys.Spawn(Random.Range(0, 5), transform, base.rotIndex, matId);
			}
		}
	}
	public override void Passive() {}
	public override void Edit() {}
}