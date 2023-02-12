using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator: MonoBehaviour
{
	public Vector3 p1;
	public Vector3 p2;
	public Vector3 p3;

	Mesh mesh;
	Vector3[] vertices;
	int[] triangles;

	public void Start()
	{
		mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
	}
	public void Update() {
		vertices = new Vector3[]
		{
			new Vector3(0, 0, 0),
			p1,
			p2,
			p3
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
	}
}
