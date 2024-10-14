
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMesh : MonoBehaviour {
	public float innerRadius = 1f;
	public float outerRadius = 5f;
    public float radius = 5f;
    public int seed = 4488;
	private Vector3[] verts;  // the vertices of the mesh
	private int[] tris;       // the triangles of the mesh (triplets of integer references to vertices)
	private int ntris = 0;    // the number of triangles that have been created so far

	// Create the mesh and put several copies of it into the scene.

	void Start() {
		Random.InitState(seed);
		// call the routine that makes a cube (the mesh) from scratch
		Mesh my_mesh = CreateMyMesh();

		// make several copies of this mesh and place these copies in the scene

		int num_objects = 100;


		for (int i = 0; i < num_objects; i++) {

			// create a new GameObject and give it a MeshFilter and a MeshRenderer
			GameObject s = new GameObject(i.ToString("Object 0"));
			s.AddComponent<MeshFilter>();
			s.AddComponent<MeshRenderer>();

			Vector3 randPos = GetRandomPos(radius);
			s.transform.position = new Vector3 (randPos.x, randPos.y, randPos.z);
			s.transform.localScale = new Vector3 (0.25f, 0.25f, 0.25f);  // shrink the object

			// associate the mesh with this object
			s.GetComponent<MeshFilter>().mesh = my_mesh;

			// change the color of the object
			Renderer rend = s.GetComponent<Renderer>();
			rend.material.color = new Color (Random.value, Random.value, Random.value, 1.0f);
		}
	
	}

	Vector3 GetRandomPos (float radius)
	{
		Vector3 pos;
		float len;

		do
		{
			pos = Random.insideUnitSphere * radius;
			len = pos.sqrMagnitude;
		} while (len > radius * radius);

		return pos;
	}

	// Create a cube that is centered at the origin (0, 0, 0) with sides of length = 2.
	//
	// Although the faces of a cube share corners, we cannot share these vertices
	// because that would mess up the surface normals at the vertices.

	Mesh CreateMyMesh() {
		
		// create a mesh object
		Mesh mesh = new Mesh();
		int slices = 15;
		int loops = 15;
		verts = new Vector3[(slices+1) * (loops+1)];
		tris = new int[(slices+1) * (loops+1) * 6];
		int vertIndex = 0;
		float theta = 0, phi = 0;
		float vertAngle = (Mathf.PI * 2.0f) / (float)loops;
		float horzAngle = (Mathf.PI * 2.0f) / (float)slices;

		for (int i = 0; i <= slices; i++)
		{
			theta = i * vertAngle;
			for (int j = 0; j <= loops; j++)
			{
				phi = j * horzAngle;
				
				float x = Mathf.Cos(theta) * (outerRadius + innerRadius * Mathf.Cos(phi));
				float y = Mathf.Sin(theta) * (outerRadius + innerRadius * Mathf.Cos(phi));
                float z = innerRadius * Mathf.Sin(phi);

				verts[vertIndex] = new Vector3(x, y, z);
                vertIndex++;
			}
		}
		int triIndex = 0;
        for (int i = 0; i < slices; i++)
        {
            for (int j = 0; j < loops; j++)
            {
			
                int i1 = i * (loops + 1) + j;
                int i2 = i1 + 1;
                int i3 = (i + 1) * (loops + 1) + j;
                int i4 = i3 + 1;

                tris[triIndex] = i1;
                tris[triIndex + 1] = i3;
                tris[triIndex + 2] = i2;
                triIndex += 3;


                tris[triIndex] = i2;
                tris[triIndex + 1] = i3;
                tris[triIndex + 2] = i4;
                triIndex += 3;
            }
        }
		// save the vertices and the triangles in the mesh object
		mesh.vertices = verts;
		mesh.triangles = tris;

		mesh.RecalculateNormals();  // automatically calculate the vertex normals

		return (mesh);
	}

	// make a triangle from three vertex indices (clockwise order)
	void MakeTri(int i1, int i2, int i3) {
		int index = ntris * 3;  // figure out the base index for storing triangle indices
		ntris++;

		tris[index]     = i1;
		tris[index + 1] = i2;
		tris[index + 2] = i3;
	}

	// make a quadrilateral from four vertex indices (clockwise order)
	void MakeQuad(int i1, int i2, int i3, int i4) {
		MakeTri (i1, i2, i4);
		MakeTri (i1, i4, i3);
	}

	// Update is called once per frame (in this case we don't need to do anything)
	void Update () {
	}
}
