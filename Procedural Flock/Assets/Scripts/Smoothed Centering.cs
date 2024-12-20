using System.Collections.Generic;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

[CreateAssetMenu(menuName = "Flock/Behavior/Smoothed Centering")]
public class SmoothedCentering : FlockBehavior
{
    public float smoothTime = 0.5f;
    Vector2 currVelo;
    public override Vector2 CalcMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        if (context.Count == 0 || !flock.CenteringBehavior)
        {
            return Vector2.zero;
        }
        
        Vector2 centerMove = Vector2.zero;
        foreach (Transform item in context)
        {
            centerMove += (Vector2) item.position;
        }
        
        centerMove /= context.Count;
        centerMove -= (Vector2) agent.transform.position;
        centerMove = Vector2.SmoothDamp(agent.transform.up, centerMove, ref currVelo, smoothTime);
        
        return centerMove;
    }
}

