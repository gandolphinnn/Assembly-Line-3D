using System;
using System.Collections.Generic;
using UnityEngine;

public class BlockSystem : MonoBehaviour {
	static Vector3Int mapDim = new Vector3Int(10, 10, 10); //change to maxDim
	[SerializeField]
		//public Vector3Int mapDim = new Vector3Int(10,10,10);
	//? array to store every type of block in the game
		public BlockType[] blockList;
	//? matrix to store the index of every handler in the dictionary
		int[,,] map = new int[mapDim.x, mapDim.y, mapDim.z];
	//? dictionary to store an handler for every single block placed (air is 0)
		Dictionary<int, BlockHandler> handlers = new Dictionary<int, BlockHandler>();
		int dictCounter = 1;
		//private GameData game;

	public void Init() {
		//game.seed = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
		handlers[0] = new BlockHandler(blockList[0], new Vector3Int(0,0,0));
		for (int x = 0; x < mapDim.x; x++) {
			for (int y = 0; y < mapDim.y; y++) {
				for (int z = 0; z < mapDim.z; z++) {
					map[x,y,z] = 0;
					//todo add seed import for not empty or saved games:
					//int seedIndex = x * mapDim.x + y * mapDim.y + z * mapDim.z;
					//map[x,y,z] = (int)game.seed[seedIndex];
				}
			}
		}
	}
	#region Place
		//? -1: out of boundaries, n: handler id of previous block
		public int Place(int blockTypeId, Vector3Int coord) {
			if (OutOfBorders(coord)) return -1;
			if (blockTypeId == 0) //? case if placing air
				return Break(coord);
			int hIndex = HandlerIndex(coord);
			int previousBlock = Break(coord);
			MapUpdate(dictCounter, coord);
			handlers.Add(dictCounter, new BlockHandler(blockList[blockTypeId], coord));
			handlers[dictCounter].gObj = Instantiate(blockList[blockTypeId].prefab, new Vector3(coord.x+.5f, coord.y+.5f, coord.z+.5f), new Quaternion(), transform);
			handlers[dictCounter].gObj.GetComponent<MeshRenderer>().material = blockList[blockTypeId].material;
			dictCounter++;
			return previousBlock;
		}
		public int Place(int blockTypeId, int x, int y, int z) {
			return Place(blockTypeId, new Vector3Int(x, y, z));
		}
	#endregion
	#region Break
		//? -1: out of boundaries, n: handler id braked
		public int Break(Vector3Int coord, float t = 0f) {
			if (OutOfBorders(coord)) return -1;
			int index = HandlerIndex(coord);
			if (index > 0) {
				MapUpdate(0, coord);
				Destroy(handlers[index].gObj, t);
				handlers.Remove(index);
			}
			return index;
		}
		public int Break(int x, int y, int z, float t = 0f) {
			return Break(new Vector3Int(x, y, z), t);
		}
	#endregion
	public Vector3Int NewBlockCoord(RaycastHit ray) {
		Vector3Int selectedBlock = SelectedBlockCoord(ray);
		
		return selectedBlock;
	}
	public Vector3Int SelectedBlockCoord(RaycastHit ray) {
		Vector3 pos = ray.transform.gameObject.transform.position;
		Vector3Int coord = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
		return coord;
	}
	#region useful mini functions
		void MapUpdate(int handlerId, Vector3Int coord) {
			map[coord.x, coord.y, coord.z] = handlerId;
		}
		int HandlerIndex(Vector3Int coord) {
			return map[coord.x, coord.y, coord.z];
		}
		public int GetBlockType(Vector3Int coord) {
			return Array.IndexOf(blockList, handlers[map[coord.x, coord.y, coord.z]].type);
		}
		public bool OutOfBorders(Vector3Int coord) {
			return coord.x < 0 || coord.y < 0 || coord.z< 0 || coord.x >= mapDim.x || coord.y >= mapDim.y || coord.z>= mapDim.z;
		}
	#endregion
}

public class BlockHandler{
	public BlockType type;
	public Vector3Int coord;
	public GameObject gObj;
	public BlockHandler(BlockType type, Vector3Int coord) {
		this.type = type;
		this.coord = coord;
	}
}

[Serializable]
public struct BlockType {
	public string name;
	public Material material;
	public GameObject prefab;
}
/* private struct GameData {
	public string seed;
	public Vector3Int playerCoord;
} */