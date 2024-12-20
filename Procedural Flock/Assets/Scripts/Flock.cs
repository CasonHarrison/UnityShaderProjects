using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public FlockAgent agentPrefab;
    List<FlockAgent> agents = new List<FlockAgent>();
    public FlockBehavior behavior;
    public bool CenteringBehavior;
    public bool AvoidanceBehavior;
    public bool VelocityMatchingBehavior;
    public bool WanderingBehavior;
    public bool Trails;
    private float squareMaxSpeed;
    private float squareNeighborRadius;
    float squareAvoidanceRadius;
    

    private const float density = 0.08f;
    [Range(10, 500)] public int startCount = 250;
    [Range(1f, 10f)] public float neighborRadius = 1.5f;
    [Range(0f, 1f)] public float avoidanceRadiusFactor = 0.5f;
    [Range(1f, 100f)] public float driveFactor = 10f;
    [Range(1f, 100f)] public float maxSpeed = 5f;
    
    
    // Start is called before the first frame update
    void Start()
    {
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighborRadius = neighborRadius * neighborRadius;
        squareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusFactor * avoidanceRadiusFactor;

        for (int i = 0; i < startCount; i++)
        {
            FlockAgent agent = Instantiate(
                agentPrefab, Random.insideUnitCircle * startCount * density,
                Quaternion.Euler(Vector3.forward * Random.Range(0f, 360f)),
                transform
                );
            agent.name = "Agent" + i;
            agents.Add(agent);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ScatterAgents();
        }
        
        foreach (FlockAgent agent in agents)
        {
            List<Transform> context = GetNearbyAgents(agent);
            Vector2 move = behavior.CalcMove(agent, context, this);
            move *= driveFactor;
            if (move.sqrMagnitude > squareMaxSpeed)
            {
                move = move.normalized * maxSpeed;
            }
            agent.Move(move);
            agent.EnableTrail(Trails);
        }
    }

    List<Transform> GetNearbyAgents(FlockAgent agent)
    {
        List<Transform> context = new List<Transform>();
        Collider2D[] contextColliders = Physics2D.OverlapCircleAll(agent.transform.position, neighborRadius);
        foreach (Collider2D c in contextColliders)
        {
            if (c != agent.AgentCollider)
            {
                context.Add(c.transform);
            }
        }
        return context;
    }
    
    private void ScatterAgents()
    {
        foreach (FlockAgent agent in agents)
        {
            Vector2 randomPosition = Random.insideUnitCircle * (startCount * density);
            agent.transform.position = randomPosition;
        }
    }
    
    public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }
}
