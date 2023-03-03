using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour {
	#region game mode
		[Header("KeyBinds")]
			public KeyCode gameModeSwapKey = KeyCode.LeftAlt;
			public KeyCode gameModePlayKey = KeyCode.P;
		string gameMode = "Build";
		bool gameModePlay = false;
	#endregion

	#region raycast
		[Header("RayCast")]
			[SerializeField] Transform cam;
			[SerializeField] LayerMask rayLayer;
		RaycastHit rayResult;
	#endregion

	#region scripts
		[Header("Scripts")]
			[SerializeField] BlockSystem bSys;
			[SerializeField] MainUIManager cMan;
	#endregion

	#region player data
		Vector3Int selectedCoord;
		Vector3Int placingCoord;
		Dictionary<int, int> inventory = new Dictionary<int, int>(); //? inventory[blockId] = quantity of that block in inventory
		int activeInventoryId = 1;
	#endregion

	void Start() {
		for (int i = 1; i < bSys.everyBlockList.Length; i++) {
			inventory.Add(i, 0);
		}
		//? testing
		inventory[1] = 100;
		inventory[2] = 5;
	}
	void Update() {
		#region change gameMode
			if (Input.GetKeyDown(gameModeSwapKey) && gameMode != "Build")
				gameMode = "Build";
			if (Input.GetKeyDown(gameModeSwapKey) && gameMode != "Edit")
				gameMode = "Edit";
			if (Input.GetKeyDown(gameModePlayKey)) {
				gameModePlay = !gameModePlay;
				cMan.PlayPause(gameModePlay);
			}
		#endregion
		
		if (Physics.Raycast(cam.position, cam.forward, out rayResult, Mathf.Infinity, rayLayer)) {
			selectedCoord = bSys.GetSelectedCoord(rayResult);
			if (gameMode == "Build") {
				placingCoord = bSys.GetPlacingCoord(rayResult);
				//* place
				if (Input.GetMouseButtonDown(1) && inventory[activeInventoryId] > 0 && !IsTooClose(placingCoord) && rayResult.distance <= 15 && bSys.Place(activeInventoryId, placingCoord) == 0) {
					inventory[activeInventoryId]--;
				}
				//* break
				if (Input.GetMouseButtonDown(0) && !bSys.OutOfBorders(selectedCoord)) {
					int breakedTypeIndex = bSys.GetBlockType(selectedCoord);
					if( bSys.Break(selectedCoord) > 0) 
						inventory[breakedTypeIndex]++;
				}
			}
			if (gameMode == "Edit") {
				bSys.Edit(selectedCoord);
			}
			if (gameModePlay) {
				bSys.Play();
			}
		}
	}

	bool IsTooClose(Vector3Int block) {
		Vector3 coord = transform.position;
		List<Vector3Int> occupiedBlocks = new List<Vector3Int>();
		int[] arrX = {Mathf.FloorToInt(coord.x - .45f), Mathf.FloorToInt(coord.x + .45f)};
		int[] arrY = {Mathf.FloorToInt(coord.y - .9f), Mathf.FloorToInt(coord.y), Mathf.FloorToInt(coord.y + .9f)};
		int[] arrZ = {Mathf.FloorToInt(coord.z - .45f), Mathf.FloorToInt(coord.z + .45f)};
		foreach (var x in arrX)
			foreach (var y in arrY)
				foreach (var z in arrZ)
					occupiedBlocks.Add(new Vector3Int(x, y, z));
		return occupiedBlocks.IndexOf(block) != -1;
	}
}