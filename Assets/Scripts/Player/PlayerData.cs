using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour {
	#region keys
		[Header("KeyBinds")]
			public KeyCode activeBlockUpKey = KeyCode.E;
			public KeyCode activeBlockDownKey = KeyCode.Q;
	#endregion

	#region scripts
		PlayerInteractions pInt;
		BlockSystem bSys;
	#endregion

	[HideInInspector] public int activeBlock = 1;
	public Dictionary<int, int> inventory = new Dictionary<int, int>(); //? inventory[blockId] = quantity of that block in inventory

	void Start() {
		pInt = GetComponent<PlayerInteractions>();
		bSys = pInt.bSys;
		for (int i = 1; i < bSys.typeList.Length; i++)
			inventory.Add(i, 100);
		
	}
	
	void Update() {
		ActiveBlockUpdate();
	}

	void ActiveBlockUpdate(int val = 0) {
		if (Input.mouseScrollDelta.y == 1 || Input.GetKeyDown(activeBlockUpKey)) activeBlock++;
		if (Input.mouseScrollDelta.y == -1 || Input.GetKeyDown(activeBlockDownKey)) activeBlock--;
		if (activeBlock == 0) activeBlock = inventory.Count;
		if (activeBlock > inventory.Count) activeBlock = 1;
		Texture2D icon = bSys.typeList[activeBlock].icon;
	}
}