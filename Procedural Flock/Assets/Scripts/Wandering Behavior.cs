
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
[CreateAssetMenu(menuName = "Flock/Behavior/Wandering")]
public class WanderingBehavior : FlockBehavior
{
    const float PI = Mathf.PI;
    public float wanderRadius = 1.0f;
    public float wanderDistance = 3.0f;
    float displaceRange = 2f;
    Vector2 currentVelo;
    float smoothTime = 0.5f;
    
    public override Vector2 CalcMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        if (!flock.WanderingBehavior) return Vector2.zero;
        
        Random.InitState(agent.transform.GetInstanceID());
        Vector2 wanderPoint = (Vector2) agent.transform.up * wanderDistance;
        float theta = Mathf.Atan2(agent.transform.up.y, agent.transform.up.x) + agent.wanderTheta;
        Vector2 target = new Vector2(wanderRadius * Mathf.Cos(theta), wanderRadius * Mathf.Sin(theta));
        Vector2 wanderForce = wanderPoint + target;

        Vector2 steer = wanderForce - (Vector2)agent.transform.position;

        steer.Normalize();
        steer *= wanderRadius;
        steer = Vector2.SmoothDamp(agent.transform.up, steer, ref currentVelo, smoothTime);
        agent.wanderTheta += Random.Range(-displaceRange, displaceRange);
        return steer;
        
    }
    
}
