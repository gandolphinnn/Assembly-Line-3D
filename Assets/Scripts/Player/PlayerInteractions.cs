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
		Vector3Int selectedCoord;
		Vector3Int placingCoord;
	#endregion

	#region scripts
		PlayerData pData;
		[Header("Scripts")]
			public GameCanvas guiMan;
			public BlockSystem bSys;
	#endregion

	void Start() {
		pData = GetComponent<PlayerData>();
	}

	void Update() {
		#region change gameMode
			if (Input.GetKeyDown(gameModeSwapKey) && gameMode != "Build")
				gameMode = "Build";
			if (Input.GetKeyDown(gameModeSwapKey) && gameMode != "Edit")
				gameMode = "Edit";
			if (Input.GetKeyDown(gameModePlayKey)) {
				gameModePlay = !gameModePlay;
				guiMan.PlayPause(gameModePlay);
			}
		#endregion
		
		if (Physics.Raycast(cam.position, cam.forward, out rayResult, 15, rayLayer)) {
			selectedCoord = bSys.GetSelectedCoord(rayResult);
			if (gameMode == "Build") {
				placingCoord = bSys.GetPlacingCoord(rayResult);
				int rotIndex = Mathf.FloorToInt(((cam.transform.rotation.eulerAngles.y +45) %360) /90);
				#region place and break
					if (Input.GetMouseButtonDown(1) && pData.inventory[pData.activeBlock] > 0 && !IsTooClose(placingCoord) && rayResult.distance <= 15
						&& bSys.Place(pData.activeBlock, placingCoord, rotIndex))
						pData.inventory[pData.activeBlock]--;
					if (Input.GetMouseButtonDown(0) && !bSys.OutOfBorders(selectedCoord)) {
						//int b = bSys.BlockTypeIndex(selectedCoord);
						int breakedTypeIndex = bSys.Break(selectedCoord);
						if (breakedTypeIndex > 0)
							pData.inventory[breakedTypeIndex]++;
					}
				#endregion
			}
			else if (Input.GetMouseButtonDown(1) && rayResult.transform.gameObject.layer == LayerMask.NameToLayer("Block")
					&& gameMode == "Edit")
				bSys.Edit(selectedCoord);
		}
		if (gameModePlay) {
			bSys.Play();
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