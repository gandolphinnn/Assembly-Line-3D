using System;
using System.Collections.Generic;
using UnityEngine;

public class BlockSystem : MonoBehaviour {
	[SerializeField] BordersGenerator bGen;
	static Vector3Int mapDim = new Vector3Int(30, 10, 30);
	bool[,] unlockedChunks = new bool[3, 3] {
		// +----> NORTH
		/* | */	{false, false, false},
		/* | */	{false, true, false},
		/* v */	{false, false, false}
	};
	int[,,] map = new int[mapDim.x, mapDim.y, mapDim.z]; //? store the index of every handler in the dictionary
	
	public BlockType[] everyBlockList; //? store every type of block in the game
	Dictionary<int, BlockHandler> handlers = new Dictionary<int, BlockHandler>(); //? store an handler for every single block placed (air is 0)
	int handlerCounter = 1; //? must be used as a PK for unique handlers id

	void Start() {
		bGen.GenerateChunks(mapDim, unlockedChunks);
		handlers.Add(0, new BlockHandler(everyBlockList[0]));
		for (int x = 0; x < mapDim.x; x++) {
			for (int y = 0; y < mapDim.y; y++) {
				for (int z = 0; z < mapDim.z; z++) {
					map[x,y,z] = 0;
				}
			}
		}
	}
	public void Play() {

	}
	public void Edit(Vector3Int coord) {
		//handlers[map[coord.x, coord.y, coord.z]];
	}

	#region Place
		//? -1: out of borders, n: handler index of previous block
		public int Place(int blockTypeId, Vector3Int coord) {
			if (OutOfBorders(coord)) return -1;
			if (blockTypeId == 0) return Break(coord); //? case if placing air
			int previousBlock = Break(coord);
			UpdateMapValue(handlerCounter, coord);
			handlers.Add(handlerCounter, new BlockHandler(everyBlockList[blockTypeId],
						Instantiate(everyBlockList[blockTypeId].prefab, new Vector3(coord.x+.5f, coord.y+.5f, coord.z+.5f), new Quaternion(), transform),
						everyBlockList[blockTypeId].name));
			handlerCounter++;
			return previousBlock;
		}
		public int Place(int blockTypeId, int x, int y, int z) {
			return Place(blockTypeId, new Vector3Int(x, y, z));
		}
	#endregion

	#region Break
		//? -1: out of borders, n: braked handler index
		public int Break(Vector3Int coord) {
			if (OutOfBorders(coord)) return -1;
			int hIndex = GetMapValue(coord);
			if (hIndex > 0) {
				UpdateMapValue(0, coord);
				Destroy(handlers[hIndex].gObj);
				handlers.Remove(hIndex);
			}
			return hIndex;
		}
		public int Break(int x, int y, int z) {
			return Break(new Vector3Int(x, y, z));
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

	public Vector3Int GetSelectedCoord(RaycastHit ray) {
		Vector3 pos = ray.transform.gameObject.transform.position;
		Vector3Int coord = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));

		return coord;
	}
	public Vector3Int GetPlacingCoord(RaycastHit ray) {
		Vector3Int selectedBlock = GetSelectedCoord(ray);
		return selectedBlock;
	}
	#region useful mini functions
		Vector3Int[] NormalizeCube(Vector3Int c1, Vector3Int c2) {
			Vector3Int[] c = new Vector3Int[] {
				new Vector3Int((int)MathF.Min(c1.x, c2.x), (int)MathF.Min(c1.y, c2.y), (int)MathF.Min(c1.z, c2.z)),
				new Vector3Int((int)MathF.Max(c1.x, c2.x), (int)MathF.Max(c1.y, c2.y), (int)MathF.Max(c1.z, c2.z))
			};
			return c;
		}
		void UpdateMapValue(int handlerId, Vector3Int coord) {
			map[coord.x, coord.y, coord.z] = handlerId;
		}
		int GetMapValue(Vector3Int coord) {
			return map[coord.x, coord.y, coord.z];
		}

		public int GetBlockType(Vector3Int coord) {
			return Array.IndexOf(everyBlockList, handlers[map[coord.x, coord.y, coord.z]].type);
		}
		public bool OutOfBorders(Vector3Int coord) {
			Vector3Int chunkDim = new Vector3Int(mapDim.x/unlockedChunks.GetLength(0), mapDim.y, mapDim.z/unlockedChunks.GetLength(1));
			return coord.x < 0 || coord.y < 0 || coord.z < 0 || coord.x >= mapDim.x || coord.y >= mapDim.y || coord.z >= mapDim.z || !unlockedChunks[Mathf.FloorToInt(coord.x / chunkDim.x), Mathf.FloorToInt(coord.z / chunkDim.z)];
		}
	#endregion
}

public class BlockHandler{
	public GameObject gObj; //? defined when the block is created
	public BlockType type;
	public BlockHandler(BlockType type) {
		this.type = type;
	}
	public BlockHandler(BlockType type, GameObject gObj, string name) {
		this.type = type;
		this.gObj = gObj;
		this.gObj.name = name;
	}
}

[Serializable]
public struct BlockType {
	public string name;
	public GameObject prefab;
}