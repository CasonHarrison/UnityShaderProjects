
using UnityEngine;
using System.Collections.Generic;

public class TransformationGrid : MonoBehaviour
{
    public Transform prefab;
    public int gridResolution = 10;
    Transform[] grid;
    List<Transformation> transformations;
    Matrix4x4 transformation;

    void Awake () { 
        grid = new Transform[gridResolution * gridResolution * gridResolution]; // make 10x10x10 grid
        for (int i = 0, z = 0; z < gridResolution; z++)
        {
            for (int y = 0; y < gridResolution; y++)
            {
                for (int x = 0; x < gridResolution; x++, i++)
                {
                    grid[i] = CreateGridPoint(x, y, z);
                }
            }
        }
        transformations = new List<Transformation>();
    }
    Transform CreateGridPoint (int x, int y, int z) //color coordinates
    {
        Transform point = Instantiate<Transform>(prefab);
        point.localPosition = GetCoordinates(x, y, z);
        point.GetComponent<MeshRenderer>().material.color = new Color(
            (float)x / gridResolution,
            (float)y / gridResolution,
            (float)z / gridResolution
            );
        return point;
    }
    Vector3 GetCoordinates(int x, int y, int z)
    {
        return new Vector3(
            x - (gridResolution - 1) * .5f,
            y - (gridResolution - 1) * .5f,
            z - (gridResolution - 1) * .5f
        );
    }
    void Update() {
        UpdateTransformation(); 
        for (int i = 0, z = 0; z < gridResolution; z++) {
            for (int y = 0; y < gridResolution; y++) {
                for (int x = 0; x < gridResolution; x++, i++) {
                    grid[i].localPosition = TransformPoint(x, y, z);
                }
            } 
        }
    }

    void UpdateTransformation()
    {                                                  //grab during each update so works during playmode.
        GetComponents<Transformation>(transformations);//Good to use list for GetComponent Updates because GetComponents returns array with all components,
        if (transformations.Count > 0)
        {
            transformation = transformations[0].Matrix;
            for (int i = 1; i < transformations.Count; i++)
            {
                transformation = transformations[i].Matrix * transformation;
            }
        } //creating a new array each time. So better to use list when grabbing components often.
    }
    
    Vector3 TransformPoint(int x, int y, int z) {
        Vector3 coordinates = GetCoordinates(x, y, z);
        return transformation.MultiplyPoint(coordinates);
    }
}
