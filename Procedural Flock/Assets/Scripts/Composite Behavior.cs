using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Flock/Behavior/Composite")]
public class CompositeBehavior : FlockBehavior
{
    public float[] weights;
    public FlockBehavior[] behaviors;
    
    public override Vector2 CalcMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        if (weights.Length != behaviors.Length)
        {
            Debug.LogError("Weights and behaviors do not match");
            return Vector2.zero;
        }
        
        Vector2 compositeMove = Vector2.zero;
        for (int i = 0; i < behaviors.Length; i++)
        {
            Vector2 move = behaviors[i].CalcMove(agent, context, flock) * weights[i];
            if (move != Vector2.zero)
            {
                if (move.sqrMagnitude > weights[i] * weights[i])
                {
                    move.Normalize();
                    move *= weights[i];
                }
                compositeMove += move;
            }
        }

        return compositeMove;
    }
}
