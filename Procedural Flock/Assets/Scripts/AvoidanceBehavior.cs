using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Flock/Behavior/Avoidance")]
public class AvoidanceBehavior : FlockBehavior
{
    public override Vector2 CalcMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        if (context.Count == 0 || !flock.AvoidanceBehavior)
        {
            return Vector2.zero;
        }
        
        Vector2 avoidMove = Vector2.zero;
        int avoidCount = 0;
        foreach (Transform item in context)
        {
            if (Vector2.SqrMagnitude(item.position - agent.transform.position) < flock.SquareAvoidanceRadius)
            {
                avoidCount++;
                avoidMove += (Vector2) (agent.transform.position - item.position);
            }
        }

        if (avoidCount > 0)
        {
            avoidMove /= avoidCount;
        }
        return avoidMove;
    }
}