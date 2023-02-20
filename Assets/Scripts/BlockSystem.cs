using System;
using System.Collections.Generic;
using UnityEngine;

public class BlockSystem : MonoBehaviour {
	static Vector3Int mapDim = new Vector3Int(30, 10, 30);
	bool[,] unlockedChunks = new bool[3, 3] {
		// +----> NORTH
		/* | */	{false, false, false},
		/* | */	{false, true, false},
		/* v */	{false, false, false}
	};
	[SerializeField]
	//? array to store every type of block in the game
		public BlockType[] blockList;
	//? matrix to store the index of every handler in the dictionary
		int[,,] map = new int[mapDim.x, mapDim.y, mapDim.z];
	//? dictionary to store an handler for every single block placed (air is 0)
		Dictionary<int, BlockHandler> handlers = new Dictionary<int, BlockHandler>();
		int dictCounter = 1;

		public BordersGenerator bGen;

	void Start() {
		handlers[0] = new BlockHandler(blockList[0], new Vector3Int(0,0,0));
		for (int x = 0; x < mapDim.x; x++) {
			for (int y = 0; y < mapDim.y; y++) {
				for (int z = 0; z < mapDim.z; z++) {
					map[x,y,z] = 0;
				}
			}
		}
		bGen.GenerateChunks(mapDim, unlockedChunks);
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
			handlers[dictCounter].gObj.name = blockList[blockTypeId].name;
			dictCounter++;
			return previousBlock;
		}
		public int Place(int blockTypeId, int x, int y, int z) {
			return Place(blockTypeId, new Vector3Int(x, y, z));
		}
	#endregion

	#region Break\
		//? -1: out of boundaries, n: braked handler id
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

	#region Fill
		public List<int> Fill(int blockTypeId, Vector3Int c1, Vector3Int c2) {
			Vector3Int[] c = NormalizeCube(c1, c2);
			List<int> res = new List<int>();
			for (int x = c[0].x; x <= c[1].x; x++) {
				for (int y = c[0].y; y <= c[1].y; y++) {
					for (int z = c[0].z; z <= c[1].z; z++) {
						res.Add(Place(blockTypeId, x, y, z));
					}
				}
			}
			return res;
		}
		public List<int> Fill(int blockTypeId, int x1, int y1, int z1, int x2, int y2, int z2) {
			return Fill(blockTypeId, new Vector3Int(x1, y1, z1), new Vector3Int(x2, y2, z2));
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
		Vector3Int[] NormalizeCube(Vector3Int c1, Vector3Int c2) {
			Vector3Int[] c = new Vector3Int[] {
				new Vector3Int((int)MathF.Min(c1.x, c2.x), (int)MathF.Min(c1.y, c2.y), (int)MathF.Min(c1.z, c2.z)),
				new Vector3Int((int)MathF.Max(c1.x, c2.x), (int)MathF.Max(c1.y, c2.y), (int)MathF.Max(c1.z, c2.z))
			};
			return c;
		}
		public int GetBlockType(Vector3Int coord) {
			return Array.IndexOf(blockList, handlers[map[coord.x, coord.y, coord.z]].type);
		}
		public bool OutOfBorders(Vector3Int coord) {
			return coord.x < 0 || coord.y < 0 || coord.z< 0 || coord.x >= mapDim.x || coord.y >= mapDim.y || coord.z>= mapDim.z;
		}
		public void unlockChunk() {
			bGen.GenerateChunks(mapDim, unlockedChunks);
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