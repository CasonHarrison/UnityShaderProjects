using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Flock/Behavior/Velocity Matching")]
public class VelocityMatchingBehavior : FlockBehavior
{
    public override Vector2 CalcMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        if (context.Count == 0)
        {
            return agent.transform.up;
        }

        if (!flock.VelocityMatchingBehavior) return Vector2.zero;
        
        Vector2 velocityMatchMove = Vector2.zero;
        foreach (Transform item in context)
        {
            velocityMatchMove += (Vector2) item.transform.up;
        }
        
        velocityMatchMove /= context.Count;
        
        return velocityMatchMove;
    } 
}