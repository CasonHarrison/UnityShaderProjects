
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class CreatureGenerator : MonoBehaviour
{
    public int mainSeed = 12345;
    public int numberOfCreatures = 5;
    public GameObject wing;
    public GameObject wing2;
    public GameObject wing3;
    void Start()
    {
        Random.InitState(mainSeed);
        GenerateCreatures();
    }

    void GenerateCreatures()
    {
        for (int i = 0; i < numberOfCreatures; i++)
        {
            int creatureSeed = mainSeed + i;
            Random.InitState(creatureSeed);
            float pos = i * 10;
            GenerateCreature(pos);
        }
    }

    void GenerateCreature(float pos)
    {
        GameObject creature = new GameObject("Creature_" + (pos % 10) );
        Vector3 bodyPosition = new Vector3(pos, 0, 0);
        Color creatureColor = Random.ColorHSV();
        Material creatureMat = new Material(Shader.Find("Standard"));
        creatureMat.color = creatureColor;
        (float bodyRadius, int bodyLength) = GenerateBody(creature, bodyPosition, creatureMat);
        GenerateLegs(creature, bodyPosition, bodyRadius, bodyLength, creatureMat);
        GenerateEyes(creature, bodyPosition, bodyRadius, bodyLength);
        GenerateWings(creature, bodyPosition, bodyRadius, bodyLength, creatureMat);
    }

    (float bodyRadius, int bodyLength) GenerateBody(GameObject creature, Vector3 pos, Material creatureMat)
    {
        float randRadius = Random.Range(1.0f, 2.5f);
        List<Vector3> bodyControlPoints = new List<Vector3>();
        int randLength = Random.Range(5, 8);
        for (int i = 0; i < randLength; i++)
        {
            bodyControlPoints.Add(new Vector3(pos.x + i + 2, pos.y, pos.z));
        }
        CatmullRomCurve bodyCurve = new CatmullRomCurve(bodyControlPoints, false);
        Mesh bodyMesh = Tube.Build(bodyCurve, 20, randRadius, 8, false);

        GameObject body = new GameObject("Body");
        body.transform.parent = creature.transform;
        MeshFilter bodyMf = body.AddComponent<MeshFilter>();
        MeshRenderer bodyMr = body.AddComponent<MeshRenderer>();
        bodyMf.mesh = bodyMesh;
        bodyMr.material = creatureMat;
  
        return (randRadius, randLength);
    }
    
    void GenerateEyes(GameObject creature, Vector3 bodyPosition, float bodyRadius, int bodyLength)
    {
        CatmullRomCurve bodyCurve = new CatmullRomCurve(
            new List<Vector3>() {
                bodyPosition, new Vector3(bodyPosition.x + bodyLength, bodyPosition.y, bodyPosition.z)
            }, 
            false
        );
        
        List<FrenetFrame> frames = bodyCurve.ComputeFrenetFrames(20, false);
        FrenetFrame endFrame = frames[frames.Count - 1];
        
        Vector3 endPosition = bodyCurve.GetPoint(1.0f); 
        endPosition += endFrame.Tangent * bodyRadius + new Vector3(0.75f, 0f, 0f) ;

        Material eyeMat = new Material(Shader.Find("Standard"));
        eyeMat.color = Color.black;

        CreateEye(creature, eyeMat, endPosition + endFrame.Normal * bodyRadius / 2 , bodyRadius * 0.3f);
        CreateEye(creature, eyeMat, endPosition - endFrame.Normal * bodyRadius / 2 , bodyRadius * 0.3f);
        
    }

    void CreateEye(GameObject creature, Material eyeMat, Vector3 position, float scale)
    {
        GameObject eye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        eye.transform.parent = creature.transform;
        eye.transform.localScale = new Vector3(scale, scale, scale);
        eye.transform.position = position;
        eye.GetComponent<Renderer>().material = eyeMat;
    }

    void GenerateLegs(GameObject creature, Vector3 bodyPosition, float bodyRadius, int bodyLength, Material creatureMat)
    {
        int legType = Random.Range(0, 3);
        if (legType == 0) GenerateLegsType1(creature, bodyPosition, bodyRadius, bodyLength, creatureMat);
        else if (legType == 1) GenerateLegsType2(creature, bodyPosition, bodyRadius, bodyLength ,creatureMat);
        else GenerateLegsType3(creature, bodyPosition, bodyRadius, bodyLength, creatureMat);
    }
    void GenerateLegsType1(GameObject creature, Vector3 bodyPosition, float bodyRadius, int bodyLength, Material creatureMat)
    {
        int legCount = Random.Range(2, 5) * 2;
        float legRadius = Random.Range(0.1f, 0.3f);
        float offset = (float) bodyLength / (float) legCount; 
        for (int i = 0; i < legCount; i++)
        {
            List<Vector3> legControlPoints = new List<Vector3>();
            GameObject leg = new GameObject("Leg_" + i);
            float offset3 = ((float) bodyLength / 4f) + 2;
            if (i  < legCount / 2)
            {
                legControlPoints.Add(new Vector3(bodyPosition.x + (offset * i) + offset3, bodyPosition.y, bodyPosition.z));
                legControlPoints.Add(new Vector3(bodyPosition.x + (offset * i) + offset3, bodyPosition.y + 1, bodyPosition.z + bodyRadius + 1));
                legControlPoints.Add(new Vector3(bodyPosition.x + (offset * i) + offset3, bodyPosition.y + 2, bodyPosition.z + bodyRadius + 2));
                legControlPoints.Add(new Vector3(bodyPosition.x + (offset * i) + offset3, bodyPosition.y - 2, bodyPosition.z + bodyRadius + 2));
                legControlPoints.Add(new Vector3(bodyPosition.x + (offset * i) + offset3, bodyPosition.y - 5, bodyPosition.z + bodyRadius + 1));
            }
            else
            {
                float offset2 = i % (legCount / 2);
                legControlPoints.Add(new Vector3(bodyPosition.x + (offset * offset2) + offset3, bodyPosition.y, bodyPosition.z));
                legControlPoints.Add(new Vector3(bodyPosition.x + (offset * offset2) + offset3, bodyPosition.y + 1, bodyPosition.z - bodyRadius - 1));
                legControlPoints.Add(new Vector3(bodyPosition.x + (offset * offset2) + offset3, bodyPosition.y + 2, bodyPosition.z - bodyRadius - 2));
                legControlPoints.Add(new Vector3(bodyPosition.x + (offset * offset2) + offset3, bodyPosition.y - 2, bodyPosition.z - bodyRadius - 2));
                legControlPoints.Add(new Vector3(bodyPosition.x + (offset * offset2) + offset3, bodyPosition.y - 5, bodyPosition.z - bodyRadius - 1));
            }
            
            CatmullRomCurve legCurve = new CatmullRomCurve(legControlPoints, false);
            Mesh legMesh = Tube.Build(legCurve, 20, legRadius, 8, false);
            leg.transform.parent = creature.transform;
            MeshFilter legMf = leg.AddComponent<MeshFilter>();
            MeshRenderer legMr = leg.AddComponent<MeshRenderer>();
            legMf.mesh = legMesh;
            legMr.material = creatureMat;
        }
    }
    void GenerateLegsType2(GameObject creature, Vector3 bodyPosition, float bodyRadius, int bodyLength, Material creatureMat)
    {
        int legCount = Random.Range(2, 5) * 2;
        float legRadius = Random.Range(0.1f, 0.3f);
        float offset = (float) bodyLength / (float) legCount; 
        for (int i = 0; i < legCount; i++)
        {
            List<Vector3> legControlPoints = new List<Vector3>();
            GameObject leg = new GameObject("Leg_" + i);
            float offset3 = ((float) bodyLength / 4f) + 2;
            if (i  < legCount / 2)
            {
                legControlPoints.Add(new Vector3(bodyPosition.x + (offset * i) + offset3, bodyPosition.y, bodyPosition.z));
                legControlPoints.Add(new Vector3(bodyPosition.x + (offset * i) + offset3, bodyPosition.y, bodyPosition.z + bodyRadius));
                legControlPoints.Add(new Vector3(bodyPosition.x + (offset * i) + offset3, bodyPosition.y - 1, bodyPosition.z + bodyRadius * 1.5f));
                legControlPoints.Add(new Vector3(bodyPosition.x + (offset * i) + offset3, bodyPosition.y - 2, bodyPosition.z + bodyRadius * 2.5f));
                legControlPoints.Add(new Vector3(bodyPosition.x + (offset * i) + offset3, bodyPosition.y - 3, bodyPosition.z + bodyRadius * 3.5f));
            }
            else
            {
                float offset2 = i % (legCount / 2);
                legControlPoints.Add(new Vector3(bodyPosition.x + (offset * offset2) + offset3, bodyPosition.y, bodyPosition.z));
                legControlPoints.Add(new Vector3(bodyPosition.x + (offset * offset2) + offset3, bodyPosition.y, bodyPosition.z - bodyRadius));
                legControlPoints.Add(new Vector3(bodyPosition.x + (offset * offset2) + offset3, bodyPosition.y - 1, bodyPosition.z - bodyRadius * 1.5f));
                legControlPoints.Add(new Vector3(bodyPosition.x + (offset * offset2) + offset3, bodyPosition.y - 2, bodyPosition.z - bodyRadius * 2.5f));
                legControlPoints.Add(new Vector3(bodyPosition.x + (offset * offset2) + offset3, bodyPosition.y - 3, bodyPosition.z - bodyRadius * 3.5f));
            }
            
            CatmullRomCurve legCurve = new CatmullRomCurve(legControlPoints, false);
            Mesh legMesh = Tube.Build(legCurve, 20, legRadius, 8, false);
            leg.transform.parent = creature.transform;
            MeshFilter legMf = leg.AddComponent<MeshFilter>();
            MeshRenderer legMr = leg.AddComponent<MeshRenderer>();
            legMf.mesh = legMesh;
            legMr.material = creatureMat;
        }
    }
    void GenerateLegsType3(GameObject creature, Vector3 bodyPosition, float bodyRadius, int bodyLength, Material creatureMat)
    {
        int legCount = Random.Range(2, 5) * 2;
        float legRadius = Random.Range(0.1f, 0.3f);
        float offset = (float) bodyLength / (float) legCount; 
        for (int i = 0; i < legCount; i++)
        {
            List<Vector3> legControlPoints = new List<Vector3>();
            GameObject leg = new GameObject("Leg_" + i);
            float offset3 = ((float) bodyLength / 4f) + 2;
            if (i  < legCount / 2)
            {
                legControlPoints.Add(new Vector3(bodyPosition.x + (offset * i) + offset3, bodyPosition.y - bodyRadius, bodyPosition.z + 0.5f));
                legControlPoints.Add(new Vector3(bodyPosition.x + (offset * i) + offset3, bodyPosition.y - bodyRadius * 1.5f, bodyPosition.z + 0.5f));
                legControlPoints.Add(new Vector3(bodyPosition.x + (offset * i) + offset3, bodyPosition.y - bodyRadius * 2.0f, bodyPosition.z + 0.5f));
            }
            else
            {
                float offset2 = i % (legCount / 2);
                legControlPoints.Add(new Vector3(bodyPosition.x + (offset * offset2) + offset3, bodyPosition.y, bodyPosition.z - 0.5f));
                legControlPoints.Add(new Vector3(bodyPosition.x + (offset * offset2) + offset3, bodyPosition.y - bodyRadius * 1.5f, bodyPosition.z - 0.5f));
                legControlPoints.Add(new Vector3(bodyPosition.x + (offset * offset2) + offset3, bodyPosition.y - bodyRadius * 2.0f, bodyPosition.z - 0.5f));
            }
            
            CatmullRomCurve legCurve = new CatmullRomCurve(legControlPoints, false);
            Mesh legMesh = Tube.Build(legCurve, 20, legRadius, 8, false);
            leg.transform.parent = creature.transform;
            MeshFilter legMf = leg.AddComponent<MeshFilter>();
            MeshRenderer legMr = leg.AddComponent<MeshRenderer>();
            legMf.mesh = legMesh;
            legMr.material = creatureMat;
        }
    }

    void GenerateWings(GameObject creature, Vector3 bodyPosition, float bodyRadius, int bodyLength, Material creatureMat)
    {
        int wingType = Random.Range(0, 3);
        switch (wingType)
        {
            case 0: GenerateWingType1(creature, bodyPosition, bodyRadius, bodyLength, creatureMat);
                break;
            case 1: GenerateWingType2(creature, bodyPosition, bodyRadius, bodyLength, creatureMat);
                break;
            case 2: GenerateWingType3(creature, bodyPosition, bodyRadius, bodyLength, creatureMat);
                break;
        }
    }

    void GenerateWingType1(GameObject creature, Vector3 bodyPosition, float bodyRadius, int bodyLength, Material creatureMat)
    {
        GameObject wing1 = Instantiate(wing, bodyPosition, Quaternion.identity);
        wing1.transform.parent = creature.transform;
        wing1.transform.position = bodyPosition + new Vector3(bodyLength, bodyRadius * 1.7f, 0.2f);
        wing1.transform.rotation = Quaternion.Euler(20f, 0, 0);
        ApplyMaterialToWings(wing1, creatureMat);
        GameObject wingx = Instantiate(wing, bodyPosition, Quaternion.identity);
        wingx.transform.parent = creature.transform;
        wingx.transform.rotation = Quaternion.Euler(-20, 0, 0);
        wingx.transform.position = bodyPosition + new Vector3(bodyLength, bodyRadius * 1.7f - .1f, -0.9f);
        ApplyMaterialToWings(wingx, creatureMat);
    }
    
    void GenerateWingType2(GameObject creature, Vector3 bodyPosition, float bodyRadius, int bodyLength, Material creatureMat)
    {
        GameObject wing1 = Instantiate(wing2, bodyPosition, Quaternion.identity);
        wing1.transform.parent = creature.transform;
        wing1.transform.position = bodyPosition + new Vector3(bodyLength, bodyRadius * 1.5f, 0.4f);
        wing1.transform.rotation = Quaternion.Euler(20f, 0, 0);
        ApplyMaterialToWings(wing1, creatureMat);
        GameObject wingx = Instantiate(wing2, bodyPosition, Quaternion.identity);
        wingx.transform.parent = creature.transform;
        wingx.transform.rotation = Quaternion.Euler(-20, 0, 0);
        wingx.transform.position = bodyPosition + new Vector3(bodyLength, bodyRadius * 1.5f, -0.4f);
        ApplyMaterialToWings(wingx, creatureMat);
    }
    
    void GenerateWingType3(GameObject creature, Vector3 bodyPosition, float bodyRadius, int bodyLength, Material creatureMat)
    {
        GameObject wing1 = Instantiate(wing3, bodyPosition, Quaternion.identity);
        wing1.transform.parent = creature.transform;
        wing1.transform.position = bodyPosition + new Vector3(bodyLength, bodyRadius * 1.5f, 0.4f);
        wing1.transform.rotation = Quaternion.Euler(20f, 0, 0);
        wing1.GetComponent<MeshRenderer>().material = creatureMat;
        GameObject wingx = Instantiate(wing3, bodyPosition, Quaternion.identity);
        wingx.transform.parent = creature.transform;
        wingx.transform.rotation = Quaternion.Euler(-20, 0, 0);
        wingx.transform.position = bodyPosition + new Vector3(bodyLength, bodyRadius * 1.5f, -0.4f);
        wingx.GetComponent<MeshRenderer>().material = creatureMat;
    }
    
    void ApplyMaterialToWings(GameObject wing, Material creatureMat)
    {
        MeshRenderer[] renderers = wing.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            renderer.material = creatureMat;
        }
    }
}
