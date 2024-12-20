using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName ="Flock/Behavior/Stay In World Behavior")]
public class StayInWorldBehavior : FlockBehavior
{
    public float radius = 20f;
    private Vector2 center = new Vector2(0f, 0f);
    public override Vector2 CalcMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        /* Circle boundary
        Vector2 offset = center - (Vector2) agent.transform.position;
        float x = offset.magnitude / radius;
        if (x < 0.9f)
        {
            return Vector2.zero;
        }

        return offset * x * x;
        */
        if (agent.transform.position.x >= radius || agent.transform.position.y >= radius ||
            agent.transform.position.x <= -radius || agent.transform.position.y <= -radius) {
            Vector2 offset = center - (Vector2) agent.transform.position;
            float x = offset.magnitude / radius;
            return offset * (x * x);
        }
        return Vector2.zero;
    }
}
