using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CBlock : MonoBehaviour {
	#region static properties
		protected ItemSystem iSys;
		protected BlockSystem bSys;
		protected PlayerData pData;
		protected Vector3Int coord;
		protected int rotIndex;
	#endregion

	#region block actions
		public List<GameObject> passiveObjs;
		public List<GameObject> activeObjs;
		public bool doAccept = true;
	#endregion

	void Start() {
		iSys = transform.parent.parent.Find("Items").GetComponent<ItemSystem>();
		bSys = transform.parent.parent.Find("Blocks").GetComponent<BlockSystem>();
		pData = transform.Find("/Player").GetComponent<PlayerData>();
		coord = new Vector3Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), Mathf.FloorToInt(transform.position.z));
		rotIndex = (int)transform.rotation.eulerAngles.y / 90;
		OnDeployment();
	}
	public virtual void ReceiveObjs(List<GameObject> objList) {
		passiveObjs = new List<GameObject>(objList);
	}
	public virtual void Passive() {
		activeObjs = activeObjs.Concat(passiveObjs).ToList();
		passiveObjs.Clear();
	}
	public virtual void Active() {}
	public virtual void Edit() {}
	public virtual void OnDeployment() {}
}