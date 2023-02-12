using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour {
	[Header("Keys")]
		public KeyCode gameModeBuild = KeyCode.Alpha1;
		public KeyCode gameModeEdit = KeyCode.Alpha2;
		public KeyCode gameModePlay = KeyCode.Alpha3;

	#region raycast
		[Header("RayCast")]
			[SerializeField] private Transform cam;
			[SerializeField] private GameObject pointer;
			[SerializeField] private Material pointerYES;
			[SerializeField] private Material pointerNO;
			[SerializeField] private LayerMask layer;
		private RaycastHit rayResult;
		private Vector3 point;
		private bool tooClose;
	#endregion

	#region block system
		public GameObject blockManager;
		private BlockSystem bSys;
	#endregion

	#region player data
		private string gameMode = "Build";
		private int activeInventoryId = 1;
		private Vector3Int selectedBlock;
		private Vector3Int placingTarget;
		private bool canBuild = false;
		//? inventory[blockId] = quantity of that block in inventory
		private Dictionary<int, int> inventory = new Dictionary<int, int>();
	#endregion

	void Start() {
		bSys = blockManager.GetComponent<BlockSystem>();
		bSys.Init();  
		for (int i = 1; i < bSys.blockList.Length; i++) {
			inventory.Add(i, 0);
		}
		inventory[1] = 10;
		inventory[2] = 5;
		pointer = Instantiate(pointer);
	}
	void Update() {
		#region change gameMode
		if (Input.GetKeyDown(gameModeBuild) && gameMode != "Build")
			gameMode = "Build";
		if (Input.GetKeyDown(gameModeEdit) && gameMode != "Edit")
			gameMode = "Edit";
		if (Input.GetKeyDown(gameModePlay) && gameMode != "Play")
			gameMode = "Play";
		if (Input.GetKeyDown(gameModeBuild) && gameMode != "Build" || Input.GetKeyDown(gameModeEdit) && gameMode != "Edit" || Input.GetKeyDown(gameModePlay) && gameMode != "Play")
			Debug.Log(gameMode);
		#endregion
		
		if (Physics.Raycast(cam.position, cam.forward, out rayResult, Mathf.Infinity, layer)) {
			point = rayResult.point;
			pointer.transform.position = point;
			selectedBlock = bSys.SelectedBlockCoord(rayResult);
			if (gameMode == "Build") {
				placingTarget = bSys.NewBlockCoord(rayResult);
				tooClose = IsTooClose(placingTarget, transform.position);
				Debug.Log(tooClose);
				if (canBuild && (tooClose || rayResult.distance > 15 || inventory[activeInventoryId] == 0))  {
					canBuild = false;
					pointer.GetComponent<MeshRenderer>().material = pointerNO;
				}
				else if (!canBuild && !tooClose && rayResult.distance <= 15 && inventory[activeInventoryId] > 0) {
					canBuild = true;
					pointer.GetComponent<MeshRenderer>().material = pointerYES;
				}

				if (Input.GetMouseButtonDown(0) && canBuild) {
					if(bSys.Place(activeInventoryId, placingTarget) == 0) {
						inventory[activeInventoryId]--;
						Debug.Log(inventory);
					}
				}
				if (Input.GetMouseButtonDown(1) && !bSys.OutOfBorders(selectedBlock)) {
					int breakedTypeIndex = bSys.GetBlockType(selectedBlock);
					if( bSys.Break(selectedBlock) > 0) {
						inventory[breakedTypeIndex]++;
					}
				}
			}
			/* if (gameMode == "Edit") {
				
			} */
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