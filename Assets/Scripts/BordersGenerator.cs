using System;
using System.Collections.Generic;
using UnityEngine;

public class BordersGenerator: MonoBehaviour {
	[SerializeField] GameObject tilePrefab;
	[SerializeField] GameObject invertPrefab;
	[SerializeField] Material borderMaterial;
	[SerializeField] Material terrainMaterial;
	bool[,] chunks;
	Vector3Int chunkDim;
	public void Init(Vector3Int mapDim, bool[,] unlockedChunks) {
		chunkDim = new Vector3Int(mapDim.x/unlockedChunks.GetLength(0), mapDim.y, mapDim.z/unlockedChunks.GetLength(1));
		GenerateChunks(unlockedChunks);
	}
	public bool GenerateChunks(bool[,] unlockedChunks) {
		if (chunks == unlockedChunks)
			return false;
		chunks = unlockedChunks;
		foreach (Transform child in transform) {
			GameObject.Destroy(child.gameObject);
		}
		for (int xChunk = 0; xChunk < chunks.GetLength(0); xChunk++) {
			for (int zChunk = 0; zChunk < chunks.GetLength(1); zChunk++) {
				if (chunks[xChunk, zChunk]) {
					GameObject chunk = NewEmpty(gameObject, $"Chunk({xChunk},{zChunk})");
					GameObject ceiling = NewEmpty(chunk, "Ceiling");
					GameObject floor = NewEmpty(chunk, "Floor");
					GameObject walls = NewEmpty(chunk, "Walls");
					#region floor and ceiling
						for (int x = xChunk*chunkDim.x; x < (xChunk + 1)*chunkDim.x; x++) {
							for (int z = zChunk*chunkDim.z; z < (zChunk + 1)*chunkDim.z; z++) {
								createTile(ceiling, new Vector3(x+.5f, chunkDim.y+.5f, z+.5f), new Vector3(0, -90, 0), true).GetComponent<MeshRenderer>().enabled = false;
								createTile(floor, new Vector3(x+.5f, -.5f, z+.5f), new Vector3(0, 90, 0)).GetComponent<MeshRenderer>().material = terrainMaterial;
							}
						}
					#endregion
					#region walls
						if (zChunk == chunks.GetLength(1) - 1 || !chunks[xChunk, zChunk+1]) {
							GameObject wall = NewEmpty(walls, "North Wall");
							for (int y = 0; y < chunkDim.y; y++) {
								for (int x = xChunk*chunkDim.x; x < (xChunk + 1)*chunkDim.x; x++) {
									createTile(wall, new Vector3(x+.5f, y+.5f, (zChunk+1)*chunkDim.z+.5f), new Vector3(-90, 0, 0));
								}
							}
						}
						if (xChunk == chunks.GetLength(0) - 1 || !chunks[xChunk+1, zChunk]) {
							GameObject wall = NewEmpty(walls, "East Wall");
							for (int y = 0; y < chunkDim.y; y++) {
								for (int z = zChunk*chunkDim.z; z < (zChunk + 1)*chunkDim.z; z++) {
									createTile(wall, new Vector3((xChunk+1)*chunkDim.x+.5f, y+.5f, z+.5f), new Vector3(0, 0, -90), true);
								}
							}
						}
						if (zChunk == 0 || !chunks[xChunk, zChunk-1]) {
							GameObject wall = NewEmpty(walls, "South Wall");
							for (int y = 0; y < chunkDim.y; y++) {
								for (int x = xChunk*chunkDim.x; x < (xChunk + 1)*chunkDim.x; x++) {
									createTile(wall, new Vector3(x+.5f, y+.5f, zChunk*chunkDim.z-.5f), new Vector3(90, 0, 0));
								}
							}
						}
						if (xChunk == 0 || !chunks[xChunk-1, zChunk]) {
							GameObject wall = NewEmpty(walls, "West Wall");
							for (int y = 0; y < chunkDim.y; y++) {
								for (int z = zChunk*chunkDim.z; z < (zChunk + 1)*chunkDim.z; z++) {
									createTile(wall, new Vector3(xChunk*chunkDim.x-.5f, y+.5f, z+.5f), new Vector3(0, 0, 90), true);
								}
							}
						}
						if (walls.transform.childCount == 0)
							Destroy(walls);
					#endregion
				}
			}
		}
		return true;
	}
	GameObject createTile(GameObject parent, Vector3 coord, Vector3 rotation, bool invert = false) {
		GameObject tile;
		if (invert)
			tile = Instantiate(invertPrefab, coord, Quaternion.Euler(rotation), parent.transform);
		else
			tile = Instantiate(tilePrefab, coord, Quaternion.Euler(rotation), parent.transform);
		tile.layer = LayerMask.NameToLayer("Borders");
		tile.AddComponent<MeshCollider>();
		tile.GetComponent<MeshRenderer>().material = borderMaterial;
		tile.GetComponent<MeshCollider>();
		tile.name = $"{parent.name} Tile({Mathf.FloorToInt(coord.x)},{Mathf.FloorToInt(coord.y)},{Mathf.FloorToInt(coord.z)})";
		return tile;
	}
	GameObject NewEmpty(GameObject parent, string name) {
		GameObject go = new GameObject(name);
		go.transform.parent = parent.transform;
		return go;
	}
}