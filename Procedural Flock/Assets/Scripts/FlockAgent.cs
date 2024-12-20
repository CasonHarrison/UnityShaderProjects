using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]

public class FlockAgent : MonoBehaviour
{
    Collider2D agentCollider;
    private TrailRenderer trailRenderer;

    public float wanderTheta;
    // Start is called before the first frame update
    void Start()
    {
        agentCollider = GetComponent<Collider2D>();
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.enabled = false;
        wanderTheta = Random.Range(0, Mathf.PI * 2);
    }
    
    public void Move (Vector2 velocity) {
        transform.up = velocity;
        transform.position += (Vector3) velocity * Time.deltaTime;
    }
    
    public Collider2D AgentCollider { get { return agentCollider; } }

    public void EnableTrail(bool enable)
    {
        trailRenderer.enabled = enable;
    }
}
