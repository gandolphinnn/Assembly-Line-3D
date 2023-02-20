using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class BordersGenerator: MonoBehaviour
{
	[SerializeField] Material borderMaterial;

	Mesh mesh;
	Vector3[] vertices;
	int[] triangles;

	public void GenerateChunks(Vector3Int mapDim, bool[,] unlockedChunks) {
		Debug.Log(unlockedChunks.GetLength(0));
		Vector3Int chunkDim = new Vector3Int(mapDim.x/3, mapDim.y, mapDim.z/3);
		for (int xChunk = 0; xChunk < unlockedChunks.GetLength(0); xChunk++) {
			for (int zChunk = 0; zChunk < unlockedChunks.GetLength(1); zChunk++) {
				if (unlockedChunks[xChunk, zChunk]) {
					GameObject chunk = NewEmpty(gameObject, $"Chunk({xChunk},{zChunk})");
					GameObject ceiling = NewEmpty(chunk, "Ceiling");
					GameObject floor = NewEmpty(chunk, "Floor");
					GameObject walls = NewEmpty(chunk, "Walls");
					#region floor and ceiling
						for (int x = xChunk*chunkDim.x; x < (xChunk + 1)*chunkDim.x; x++) {
							for (int z = zChunk*chunkDim.z; z < (zChunk + 1)*chunkDim.z; z++) {
								createTile(ceiling, new Vector3(x+.5f, chunkDim.y, z+.5f), new Vector3(180, 0, 0), $"Ceiling Tile({x},{chunkDim.y},{z})").GetComponent<MeshRenderer>().enabled = false;
								createTile(floor, new Vector3(x+.5f, 0, z+.5f), new Vector3(), $"Floor Tile({x},0,{z})");
							}
						}
					#endregion
					#region walls
						if (zChunk == unlockedChunks.GetLength(1) - 1 || !unlockedChunks[xChunk, zChunk+1]) {
							GameObject wall = NewEmpty(walls, "North Wall");
							for (int y = 0; y < chunkDim.y; y++) {
								for (int x = xChunk*chunkDim.x; x < (xChunk + 1)*chunkDim.x; x++) {
									createTile(wall, new Vector3(x+.5f, y+.5f, (zChunk+1)*chunkDim.z), new Vector3(-90, 0, 0), $"North Wall Tile({x},{y},{(zChunk+1)*chunkDim.z})");
								}
							}
						}
						if (zChunk == 0 || !unlockedChunks[xChunk, zChunk-1]) {
							GameObject wall = NewEmpty(walls, "South Wall");
							for (int y = 0; y < chunkDim.y; y++) {
								for (int x = xChunk*chunkDim.x; x < (xChunk + 1)*chunkDim.x; x++) {
									createTile(wall, new Vector3(x+.5f, y+.5f, zChunk*chunkDim.z), new Vector3(90, 0, 0), $"South Wall Tile({x},{y},{zChunk*chunkDim.z})");
								}
							}
						}
						if (xChunk == unlockedChunks.GetLength(0) - 1 || !unlockedChunks[xChunk+1, zChunk]) {
							GameObject wall = NewEmpty(walls, "East Wall");
							for (int y = 0; y < chunkDim.y; y++) {
								for (int z = zChunk*chunkDim.z; z < (zChunk + 1)*chunkDim.z; z++) {
									createTile(wall, new Vector3((xChunk+1)*chunkDim.x, y+.5f, z+.5f), new Vector3(0, 0, 90), $"East Wall Tile({(xChunk+1)*chunkDim.x},{y},{z})");
								}
							}
						}
						if (xChunk == 0 || !unlockedChunks[xChunk-1, zChunk]) {
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
	}
	GameObject createTile(GameObject parent, Vector3 coord, Vector3 rotation, string name) {
		GameObject tile = NewEmpty(parent, name);
		tile.layer = LayerMask.NameToLayer("Map Borders");
		mesh = new Mesh();
		tile.transform.position = coord;
		tile.AddComponent<MeshFilter>();
		tile.AddComponent<MeshRenderer>();
		tile.AddComponent<MeshCollider>();
		tile.GetComponent<MeshFilter>().mesh = mesh;
		tile.GetComponent<MeshRenderer>().material = borderMaterial;
		vertices = new Vector3[]
		{
			new Vector3(-.5f, 0, -.5f),
			new Vector3(-.5f, 0, .5f),
			new Vector3(.5f, 0, -.5f),
			new Vector3(.5f, 0, .5f),

		};
		mesh.vertices = vertices;
		triangles = new int[]
		{
			0, 1, 2,
			1, 3, 2
		};
		mesh.triangles = triangles;
		Vector3[] normals = new Vector3[]
		{
			-Vector3.forward,
			-Vector3.forward,
			-Vector3.forward,
			-Vector3.forward
		};
		mesh.normals = normals;
		Vector2[] uv = new Vector2[4]
		{
			new Vector2(0, 0),
			new Vector2(1, 0),
			new Vector2(0, 1),
			new Vector2(1, 1)
		};
		mesh.uv = uv;

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
