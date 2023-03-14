using System;
using System.Collections.Generic;
using UnityEngine;

public class BlockSystem : MonoBehaviour {

	#region scripts
		[SerializeField] CanvasManager cMan;
		[SerializeField] BordersGenerator bGen;
	#endregion

	#region map info
		static readonly Vector3Int mapDim = new Vector3Int(30, 6, 30);
		static readonly Vector3Int[] mapOffsets = new Vector3Int[] {
			new Vector3Int(0, 0, 1),
			new Vector3Int(1, 0, 0),
			new Vector3Int(0, 0, -1),
			new Vector3Int(-1, 0, 0)
		};
		bool[,] unlockedChunks = new bool[3, 3] {
			// +----> NORTH
			/* | */	{false, false, false},
			/* | */	{false, true, false},
			/* v */	{false, false, false}
		};
		Vector3Int chunkDim;
		int[,,] map = new int[mapDim.x, mapDim.y, mapDim.z]; //? store the index of every handler in the dictionary
		public BlockType[] typeList; //? store every type of block in the game
		Dictionary<int, BlockHandler> handlers = new Dictionary<int, BlockHandler>(); //? store an handler for every single block placed (air is 0)
		int handlerCounter = 1; //? must be used as a PK for unique handlers id
	#endregion
	
	bool playTick = true;

	void Start() {
		chunkDim = new Vector3Int(mapDim.x/unlockedChunks.GetLength(0), mapDim.y, mapDim.z/unlockedChunks.GetLength(1));
		bGen.Init(mapDim, unlockedChunks);
		for (int x = 0; x < mapDim.x; x++)
			for (int y = 0; y < mapDim.y; y++)
				for (int z = 0; z < mapDim.z; z++)
					map[x,y,z] = 0;
	}
	public void Play() {
		if (playTick) {
			foreach (var block in handlers)
				block.Value.Passive();
			foreach (var block in handlers)
				block.Value.Active();
			playTick = false;
			Invoke("PlayTickReset", 1);
		}
	}
	void PlayTickReset() {
		playTick = true;
	}
	public void Edit(Vector3Int coord) {
		BlockHandler handl = handlers[map[coord.x, coord.y, coord.z]];
		handl.Edit();
		cMan.EditPanel(handl);
	}

	#region Place
		//? false: out of borders or block occupied, true: success
		public bool Place(int blockTypeId, Vector3Int coord, int rotation = 0) {
			if (OutOfBorders(coord) || GetMapValue(coord) != 0 || blockTypeId == 0) return false;
			map[coord.x, coord.y, coord.z] = handlerCounter;
			BlockType bType = typeList[blockTypeId];
			handlers.Add(handlerCounter++, new BlockHandler(bType, 
					Instantiate(bType.prefab, new Vector3(coord.x+.5f, coord.y+.5f, coord.z+.5f), Quaternion.Euler(0, rotation*90, 0), transform)));
			return true;
		}
		public bool Place(int blockTypeId, int x, int y, int z, int rotation = 0) {
			return Place(blockTypeId, new Vector3Int(x, y, z), rotation);
		}
	#endregion

	#region Break
		//? -1: out of borders, n: breaked indexType 
		public int Break(Vector3Int coord) {
			if (OutOfBorders(coord)) return -1;
			int hIndex = GetMapValue(coord);
			int breakedType = Array.IndexOf(typeList, handlers[hIndex].type);
			if (hIndex > 0) {
				map[coord.x, coord.y, coord.z] = 0;
				Destroy(handlers[hIndex].gObj);
				handlers.Remove(hIndex);
			}
			return breakedType;
		}
		public int Break(int x, int y, int z) {
			return Break(new Vector3Int(x, y, z));
		}
	#endregion

	#region Fill
		/* public List<int> Fill(int blockTypeId, Vector3Int c1, Vector3Int c2, int rotation = 0) {
			Vector3Int[] c = new Vector3Int[] {
				new Vector3Int((int)MathF.Min(c1.x, c2.x), (int)MathF.Min(c1.y, c2.y), (int)MathF.Min(c1.z, c2.z)),
				new Vector3Int((int)MathF.Max(c1.x, c2.x), (int)MathF.Max(c1.y, c2.y), (int)MathF.Max(c1.z, c2.z))
			};
			List<int> res = new List<int>();
			for (int x = c[0].x; x <= c[1].x; x++)
				for (int y = c[0].y; y <= c[1].y; y++)
					for (int z = c[0].z; z <= c[1].z; z++)
						res.Add(Place(blockTypeId, x, y, z, rotation));
			return res;
		}
		public List<int> Fill(int blockTypeId, int x1, int y1, int z1, int x2, int y2, int z2, int rotation = 0) {
			return Fill(blockTypeId, new Vector3Int(x1, y1, z1), new Vector3Int(x2, y2, z2), rotation);
		} */
	#endregion
	
	#region select and placing
		public Vector3Int GetSelectedCoord(RaycastHit ray) {
			Vector3 pos = ray.transform.gameObject.transform.position;
			return new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
		}
		public Vector3Int GetPlacingCoord(RaycastHit ray) {
			Vector3Int coord = GetSelectedCoord(ray);
			GameObject objHit = ray.transform.gameObject;
			Vector3Int offset;
			if (OutOfBorders(coord)) {
				Vector3 rot = objHit.transform.rotation.eulerAngles;
				rot = new Vector3((rot.x == 270)? -90 : rot.x, (rot.y == 270)? -90 : rot.y, (rot.z == 270)? -90 : rot.z);
				offset = new Vector3Int((int)rot.z/90, (int)rot.y/90, (int)rot.x/90);
			}
			else {
				BoxCollider bColl = objHit.GetComponent<BoxCollider>();
				Vector3 dim = bColl.size;
				Vector3 relCoord = ray.point - objHit.transform.position;
				relCoord.y -= bColl.center.y;
				if (Mathf.FloorToInt(objHit.transform.rotation.eulerAngles.y / 90) % 2 == 1) {
					float temp = dim.x;
					dim.x = dim.z;
					dim.z = temp;
				}
				offset = new Vector3Int(Mathf.FloorToInt((Mathf.Abs(relCoord.x)+(1-dim.x/2)))*(int)Mathf.Sign(relCoord.x),
													Mathf.FloorToInt((Mathf.Abs(relCoord.y)+(1-dim.y/2)))*(int)Mathf.Sign(relCoord.y),
													Mathf.FloorToInt((Mathf.Abs(relCoord.z)+(1-dim.z/2)))*(int)Mathf.Sign(relCoord.z));
			}
			return coord + offset;
		}
	#endregion

	#region useful mini functions
		public GameObject GetNeighbour(Vector3Int coord, int rotIndex, int distance = 1) {
			Vector3Int c = coord + mapOffsets[rotIndex % 4] * distance;
			return (!OutOfBorders(c) && GetMapValue(c) != 0)? handlers[GetMapValue(c)].gObj : null;
		}
		public GameObject GetNeighbour(Vector3Int coord, Vector3Int offset) {
			Vector3Int c = coord + offset;
			return (!OutOfBorders(c) && GetMapValue(c) != 0)? handlers[GetMapValue(c)].gObj : null;
		}
		int GetMapValue(Vector3Int coord) {
			return map[coord.x, coord.y, coord.z];
		}
		public bool UnlockChunk(Vector2Int chunkIndex) {
			unlockedChunks[chunkIndex.x, chunkIndex.y] = true;
			return bGen.GenerateChunks(unlockedChunks);
		}
		public bool OutOfBorders(Vector3Int coord) {
			return coord.x < 0 || coord.y < 0 || coord.z < 0 || coord.x >= mapDim.x || coord.y >= mapDim.y || coord.z >= mapDim.z || !unlockedChunks[Mathf.FloorToInt(coord.x / chunkDim.x), Mathf.FloorToInt(coord.z / chunkDim.z)];
		}
	#endregion
}

public class BlockHandler {
	public BlockType type;
	public GameObject gObj;
	public BlockHandler(BlockType type, GameObject gObj) {
		this.type = type;
		this.gObj = gObj;
		this.gObj.name = type.name;
		this.type.script = gObj.GetComponent<CBlock>();
	}
	public void Passive() {
		this.type.script.Passive();
	}
	public void Active() {
		this.type.script.Active();
	}
	public void Edit() {
		this.type.script.Edit();
	}
}

[Serializable]
public struct BlockType {
	public string name;
	public GameObject prefab;
	[HideInInspector] public CBlock script;
	public Texture2D icon;
}