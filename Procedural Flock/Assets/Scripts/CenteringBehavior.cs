using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Flock/Behavior/Centering")]
public class CenteringBehavior : FlockBehavior
{
    public override Vector2 CalcMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        if (context.Count == 0)
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
        
        return centerMove;
    }
}
