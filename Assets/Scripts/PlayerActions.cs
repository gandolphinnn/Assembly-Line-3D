using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour {
	#region game mode
		[Header("Keys")]
			[SerializeField] KeyCode gameModeSwapKey = KeyCode.LeftAlt;
			[SerializeField] KeyCode gameModePlayKey = KeyCode.P;
		string gameMode = "Build";
		bool gameModePlay = false;
	#endregion

	#region raycast
		[Header("RayCast")]
			[SerializeField] Transform cam;
			[SerializeField] GameObject pointer;
			[SerializeField] Material pointerYES;
			[SerializeField] Material pointerNO;
			[SerializeField] LayerMask rayLayer;
		RaycastHit rayResult;
		bool tooClose;
	#endregion

	#region player data
		Vector3Int selectedCoord;
		Vector3Int placingCoord;
		bool canBuild = false;
		Dictionary<int, int> inventory = new Dictionary<int, int>(); //? inventory[blockId] = quantity of that block in inventory
		int activeInventoryId = 1;
	#endregion

	[SerializeField] BlockSystem bSys;

	void Start() {
		for (int i = 1; i < bSys.everyBlockList.Length; i++) {
			inventory.Add(i, 0);
		}
		pointer = Instantiate(pointer);
		//? testing
		inventory[1] = 10;
		inventory[2] = 5;
	}
	void Update() {
		#region change gameMode
			if (Input.GetKeyDown(gameModeSwapKey) && gameMode != "Build")
				gameMode = "Build";
			if (Input.GetKeyDown(gameModeSwapKey) && gameMode != "Edit")
				gameMode = "Edit";
			if (Input.GetKeyDown(gameModePlayKey))
				gameModePlay = !gameModePlay;
		#endregion
		
		if (Physics.Raycast(cam.position, cam.forward, out rayResult, Mathf.Infinity, rayLayer)) {
			pointer.transform.position = rayResult.point;
			Debug.Log(rayResult.triangleIndex);
			selectedCoord = bSys.GetSelectedCoord(rayResult);
			if (gameMode == "Build") {
				placingCoord = bSys.GetPlacingCoord(rayResult);
				#region can build
					tooClose = IsTooClose(placingCoord, transform.position);
					if (canBuild && (tooClose || rayResult.distance > 15 || inventory[activeInventoryId] == 0))  {
						canBuild = false;
						pointer.GetComponent<MeshRenderer>().material = pointerNO;
					}
					else if (!canBuild && !tooClose && rayResult.distance <= 15 && inventory[activeInventoryId] > 0) {
						canBuild = true;
						pointer.GetComponent<MeshRenderer>().material = pointerYES;
					}
				#endregion

				if (Input.GetMouseButtonDown(1) && canBuild) {
					if(bSys.Place(activeInventoryId, placingCoord) == 0) {
						inventory[activeInventoryId]--;
					}
				}
				if (Input.GetMouseButtonDown(0) && !bSys.OutOfBorders(selectedCoord)) {
					int breakedTypeIndex = bSys.GetBlockType(selectedCoord);
					if( bSys.Break(selectedCoord) > 0) {
						inventory[breakedTypeIndex]++;
					}
				}
			}
			if (gameMode == "Edit") {
				bSys.Edit(selectedCoord);
			}
			if (gameModePlay) {
				bSys.Play();
			}
		}
		else {
			pointer.transform.position = new Vector3(-1, -1, -1);
		}
	}

	bool IsTooClose(Vector3Int block, Vector3 coord) {
		List<Vector3Int> occupiedBlocks = new List<Vector3Int>();
		int[] arrx = {Mathf.FloorToInt(coord.x - .45f), Mathf.FloorToInt(coord.x + .45f)};
		int[] arry = {Mathf.FloorToInt(coord.y - .9f), Mathf.FloorToInt(coord.y), Mathf.FloorToInt(coord.y + .9f)};
		int[] arrz = {Mathf.FloorToInt(coord.z - .45f), Mathf.FloorToInt(coord.z + .45f)};
		foreach (var x in arrx)
			foreach (var y in arry)
				foreach (var z in arrz)
					occupiedBlocks.Add(new Vector3Int(x, y, z));
		return occupiedBlocks.IndexOf(block) != -1;
	}
}