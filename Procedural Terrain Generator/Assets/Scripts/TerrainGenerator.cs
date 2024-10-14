
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TerrainGenerator : MonoBehaviour
{
    public int width = 85;
    public int height = 85;
    public float scale = 3f;
    public float heightMultiplier = 30f;
    public Vector3 offset;
    
    public int textureWidth = 64;
    public int textureHeight = 64;
    public GameObject plantPrefab;
    public int plantCount = 40;

    public float planeSize = 85f;
    private Dictionary<Vector2, GameObject> chunks = new Dictionary<Vector2, GameObject>();

    public float unloadDist = 300f;
    private HashSet<Vector2> loadedChunks = new HashSet<Vector2>();
    
    private Dictionary<Vector2, Vector3[]> chunkNormals = new Dictionary<Vector2, Vector3[]>();
    
    // Start is called before the first frame update
    void Start()
    {
        offset = new Vector3(Random.Range(0f, 1000f), 0, Random.Range(0f, 1000f));
        GenerateTerrainChunk(Vector2.zero);
    }

    void GenerateTerrainChunk(Vector2 coord)
    {
        if (chunks.ContainsKey(coord)) return;
        Vector3 chunkPos = new Vector3(coord.x * planeSize, 0, coord.y * planeSize);
        Mesh myMesh = CreateTerrainMesh(chunkPos, coord);
        GameObject s = new GameObject("Terrain Mesh {coord}");
        s.transform.position = chunkPos;
        s.AddComponent<MeshFilter>();
        s.AddComponent<MeshRenderer>();
        s.GetComponent<MeshFilter>().mesh = myMesh;
        Renderer rend = s.GetComponent<Renderer>();
        rend.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);

        // create a texture
        Texture2D texture = MakeTexture(chunkPos);

        // attach the texture to the mesh
        Renderer renderer = s.GetComponent<Renderer>();
        renderer.material.mainTexture = texture;
        chunks[coord] = s;
        PlacePlants(s.transform, chunkPos);
    }

    Texture2D MakeTexture(Vector3 chunkPos)
    {
        Texture2D texture = new Texture2D(textureWidth, textureHeight);
        Color[] colors = new Color[textureWidth * textureHeight];
        
        for (int i = 0; i < textureWidth; i++)
        {
            for (int j = 0; j < textureHeight; j++)
            {
                float x = (chunkPos.x + i) / textureWidth * scale;
                float z = (chunkPos.z + j) / textureHeight * scale;
                float y = Mathf.PerlinNoise(x, z);
                Color color = Color.Lerp(Color.green, new Color(0, 0.5f, 0.0f, 1.0f), y);
                colors[j * textureWidth + i] = color;
            }
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        return texture;
    }

    Mesh CreateTerrainMesh(Vector3 chunkPos, Vector2 coord)
    {
        Mesh m = new Mesh();
        Vector3[] vertices = new Vector3[(width + 1) * (height + 1)];
        int[] triangles = new int[width * height * 6];
        Vector2[] uv = new Vector2[vertices.Length];
        Vector3[] normals = new Vector3[vertices.Length];
        
        for (int i = 0, z = 0; z <= height; z++)
        {
            for (int x = 0; x <= width; x++, i++)
            {
                float y = CalcHeight(chunkPos.x + x, chunkPos.z + z);
                vertices[i] = new Vector3(x, y, z);
                uv[i] = new Vector2((float)x / width, (float)z / height);
            }
        }
        
        int triIndex = 0;

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                triangles[triIndex++] = z * (width + 1) + x;
                triangles[triIndex++] = (z + 1) * (width + 1) + x;
                triangles[triIndex++] = z * (width + 1) + x + 1;

                triangles[triIndex++] = z * (width + 1) + x + 1;
                triangles[triIndex++] = (z + 1) * (width + 1) + x;
                triangles[triIndex++] = (z + 1) * (width + 1) + x + 1;
            }
        }
        
        m.vertices = vertices;
        m.triangles = triangles;
        m.uv = uv;
        m.RecalculateNormals();
        chunkNormals[coord] = normals;
        return m;
    }
    

    float CalcHeight(float x, float z)
    {
        float xCoord = (x + offset.x) / width * scale;
        float zCoord = (z + offset.z) / height * scale;
        float y = Mathf.PerlinNoise(xCoord, zCoord) * heightMultiplier;
        y += Mathf.PerlinNoise(xCoord * 2, zCoord * 2) * (heightMultiplier / 2f);
        y += Mathf.PerlinNoise(xCoord * 4, zCoord * 4) * (heightMultiplier / 4f);
        return y;
    }

    void PlacePlants(Transform parent, Vector3 chunkPos)
    {
        for (int i = 0; i < plantCount; i++)
        {
            Vector3 pos = new Vector3(Random.Range(0, width), 0, Random.Range(0, height));
            Vector3 worldPos = pos + chunkPos;
            worldPos.y = CalcHeight(worldPos.x, worldPos.z);
            worldPos.y += 3.0f;
            Instantiate(plantPrefab, worldPos, Quaternion.identity, parent);
        }
    }
    void CalculateSeamNormals(Vector2 chunkCoord)
    {
        Vector2[] adjacentCoords = new Vector2[]
        {
            chunkCoord + Vector2.right,
            chunkCoord + Vector2.left,
            chunkCoord + Vector2.up,
            chunkCoord + Vector2.down
        };

        foreach (Vector2 adjCoord in adjacentCoords)
        {
            if (chunkNormals.ContainsKey(adjCoord))
            {
                Vector3[] normalsA = chunkNormals[chunkCoord];
                Vector3[] normalsB = chunkNormals[adjCoord];
                for (int i = 0; i <= width; i++)
                {
                    int indexA = -1;
                    int indexB = -1;

                    if (adjCoord == chunkCoord + Vector2.right)
                    {
                        indexA = (i + 1) * (width + 1) - 1;
                        indexB = i * (width + 1);
                    }
                    else if (adjCoord == chunkCoord + Vector2.left)
                    {
                        indexA = i * (width + 1);
                        indexB = (i + 1) * (width + 1) - 1;
                    }
                    else if (adjCoord == chunkCoord + Vector2.up)
                    {
                        indexA = (height * (width + 1)) + i;
                        indexB = i;
                    }
                    else if (adjCoord == chunkCoord + Vector2.down)
                    {
                        indexA = i;
                        indexB = (height * (width + 1)) + i;
                    }

                    if (indexA >= 0 && indexA < normalsA.Length && indexB >= 0 && indexB < normalsB.Length)
                    {
                        Vector3 averageNormal = (normalsA[indexA] + normalsB[indexB]).normalized;
                        normalsA[indexA] = averageNormal;
                        normalsB[indexB] = averageNormal;
                        
                    }
                }
            }
        }
    }
    void Update()
    {
        float dx = Input.GetAxis ("Horizontal");
        float dz = Input.GetAxis ("Vertical");

        // sensitivity factors for translate and rotate
        float translate_factor = 0.3f;
        float rotate_factor = 3.0f;
        
        if (Camera.main != null) {

            // translate forward or backwards
            Camera.main.transform.Translate (0, 0, dz * translate_factor);

            // rotate left or right
            Camera.main.transform.Rotate (0, dx * rotate_factor, 0);
        }

        Vector3 camPos = Camera.main.transform.position;
        
        Vector2 currentChunkPos = new Vector2(
            Mathf.FloorToInt(camPos.x / planeSize),
            Mathf.FloorToInt(camPos.z / planeSize));
        LoadVisibleChunks(currentChunkPos);
        UnloadDistanceChunks(camPos);
    }

    void LoadVisibleChunks(Vector2 currentChunkPos)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                Vector2 chunkCoord = new Vector2(currentChunkPos.x + x, currentChunkPos.y + z);
                if (!loadedChunks.Contains(chunkCoord))
                {
                    GenerateTerrainChunk(chunkCoord);
                    loadedChunks.Add(chunkCoord);
                    CalculateSeamNormals(chunkCoord);
                }
            }
        }
    }

    void UnloadDistanceChunks(Vector3 camPos)
    {
        List<Vector2> chunksToRemove = new List<Vector2>();
        foreach (var chunk in loadedChunks)
        {
            Vector3 chunkCenter = new Vector3(chunk.x * planeSize + width / 2f, 0, chunk.y * planeSize + height / 2f);
            float distance = Vector3.Distance(camPos, chunkCenter);
            if (distance > unloadDist || !IsChunkInView(chunk))
            {
                chunksToRemove.Add(chunk);
            }
        }

        foreach (var chunk in chunksToRemove)
        {
            Destroy(chunks[chunk]);
            chunks.Remove(chunk);
            loadedChunks.Remove(chunk);
            chunkNormals.Remove(chunk);
        }
    }

    bool IsChunkInView(Vector2 chunkPos)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        Vector3 chunkCenter = new Vector3(chunkPos.x * planeSize + width / 2f, 0, chunkPos.y * planeSize + height / 2f);
        Bounds bounds = new Bounds(chunkCenter, new Vector3(planeSize, 100, planeSize));
        return GeometryUtility.TestPlanesAABB(planes, bounds);
    }
    
}
