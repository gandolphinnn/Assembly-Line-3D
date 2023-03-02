using System;
using System.Collections.Generic;
using UnityEngine;

public class BordersGenerator: MonoBehaviour {
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
								createTile(ceiling, new Vector3(x+.5f, chunkDim.y, z+.5f), new Vector3(180, 0, 0), $"Ceiling Tile({x},{chunkDim.y},{z})").GetComponent<MeshRenderer>().enabled = false;
								createTile(floor, new Vector3(x+.5f, 0, z+.5f), new Vector3(), $"Floor Tile({x},0,{z})").GetComponent<MeshRenderer>().material = terrainMaterial;
							}
						}
					#endregion
					#region walls
						if (zChunk == chunks.GetLength(1) - 1 || !chunks[xChunk, zChunk+1]) {
							GameObject wall = NewEmpty(walls, "North Wall");
							for (int y = 0; y < chunkDim.y; y++) {
								for (int x = xChunk*chunkDim.x; x < (xChunk + 1)*chunkDim.x; x++) {
									createTile(wall, new Vector3(x+.5f, y+.5f, (zChunk+1)*chunkDim.z), new Vector3(-90, 0, 0), $"North Wall Tile({x},{y},{(zChunk+1)*chunkDim.z})");
								}
							}
						}
						if (zChunk == 0 || !chunks[xChunk, zChunk-1]) {
							GameObject wall = NewEmpty(walls, "South Wall");
							for (int y = 0; y < chunkDim.y; y++) {
								for (int x = xChunk*chunkDim.x; x < (xChunk + 1)*chunkDim.x; x++) {
									createTile(wall, new Vector3(x+.5f, y+.5f, zChunk*chunkDim.z), new Vector3(90, 0, 0), $"South Wall Tile({x},{y},{zChunk*chunkDim.z})");
								}
							}
						}
						if (xChunk == chunks.GetLength(0) - 1 || !chunks[xChunk+1, zChunk]) {
							GameObject wall = NewEmpty(walls, "East Wall");
							for (int y = 0; y < chunkDim.y; y++) {
								for (int z = zChunk*chunkDim.z; z < (zChunk + 1)*chunkDim.z; z++) {
									createTile(wall, new Vector3((xChunk+1)*chunkDim.x, y+.5f, z+.5f), new Vector3(0, 0, 90), $"East Wall Tile({(xChunk+1)*chunkDim.x},{y},{z})");
								}
							}
						}
						if (xChunk == 0 || !chunks[xChunk-1, zChunk]) {
							GameObject wall = NewEmpty(walls, "West Wall");
							for (int y = 0; y < chunkDim.y; y++) {
								for (int z = zChunk*chunkDim.z; z < (zChunk + 1)*chunkDim.z; z++) {
									createTile(wall, new Vector3(xChunk*chunkDim.x, y+.5f, z+.5f), new Vector3(0, 0, -90), $"West Wall Tile({xChunk*chunkDim.x},{y},{z})");
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
	GameObject createTile(GameObject parent, Vector3 coord, Vector3 rotation, string name) {
		#region mesh definition
			Mesh mesh = new Mesh();
			Vector3[] vertices = new Vector3[] {
				new Vector3(-.5f, 0, -.5f),
				new Vector3(-.5f, 0, .5f),
				new Vector3(.5f, 0, -.5f),
				new Vector3(.5f, 0, .5f),
			};
			mesh.vertices = vertices;
			int[] triangles = new int[] {
				0, 1, 2,
				1, 3, 2
			};
			mesh.triangles = triangles;
		#endregion
		GameObject tile = NewEmpty(parent, name);
		tile.layer = LayerMask.NameToLayer("Map Borders");
		tile.transform.position = coord;
		tile.AddComponent<MeshFilter>();
		tile.AddComponent<MeshRenderer>();
		tile.AddComponent<MeshCollider>();
		tile.GetComponent<MeshFilter>().mesh = mesh;
		tile.GetComponent<MeshRenderer>().material = borderMaterial;
		tile.GetComponent<MeshRenderer>().enabled = true;
		tile.GetComponent<MeshCollider>().sharedMesh = mesh;
		tile.transform.Rotate(rotation);
		return tile;
	}
	GameObject NewEmpty(GameObject parent, string name) {
		GameObject go = new GameObject(name);
		go.transform.parent = parent.transform;
		return go;
	}
}
