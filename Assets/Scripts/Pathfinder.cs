using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public float agentSpeed;
    public float pathMultiplier;
    public float rotationSpeed;
    public bool hasFinished = false;
    public bool hasCrashed = false;
    public DNA agentDNA;

    public LayerMask obstacleLayer;

    private int pathIndex = 0;
    private bool initialized = false;

    List<Vector2> travelledPath = new List<Vector2>();

    private Vector2 target;
    private Vector2 nextPoint;

    Quaternion targetRotation;
    LineRenderer lr;

    public void InitAgent(DNA agentDNA, Vector2 target)
    {
        travelledPath.Add(transform.position);
        lr = GetComponent<LineRenderer>();
        this.agentDNA = agentDNA;
        this.target = target;
        nextPoint = transform.position;
        travelledPath.Add(nextPoint);
        initialized = true;
    }

    public void Update()
    {
        if (initialized && !hasFinished)
        {
            if (pathIndex == agentDNA.genome.Count || Vector2.Distance(transform.position, target) < 0.5f)
            {
                hasFinished = true;
            }

            if ((Vector2)transform.position == nextPoint)
            {
                nextPoint = (Vector2)transform.position + agentDNA.genome[pathIndex] * pathMultiplier;
                travelledPath.Add(nextPoint);
                targetRotation = LookAt2D(nextPoint);
                pathIndex++;
            }

            else
            {
                transform.position = Vector2.MoveTowards(transform.position, nextPoint, agentSpeed * Time.deltaTime);
            }

            if(transform.rotation != targetRotation)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            RenderLine();
        }
    }

    public void RenderLine()
    {
        List<Vector3> linePointers = new List<Vector3>();

        if(travelledPath.Count > 2)
        {
            for (int i = 0; i < travelledPath.Count - 1; i++)
            {
                linePointers.Add(travelledPath[i]);
            }
            linePointers.Add(transform.position);
        }
        else
        {
            linePointers.Add(travelledPath[0]);
            linePointers.Add(transform.position);
        }

        lr.positionCount = linePointers.Count;
        lr.SetPositions(linePointers.ToArray());
    }

    public float Fitness
    {
        get
        {
            float distanceFromTarget = Vector2.Distance(transform.position, target);

            if (distanceFromTarget == 0)
            {
                distanceFromTarget = 0.0001f;
            }

            RaycastHit2D[] obstacles = Physics2D.RaycastAll(transform.position, target, obstacleLayer);
            float obstacleMultiplier = 1f - (0.15f * obstacles.Length);
            return (60 / distanceFromTarget) * (hasCrashed ? 0.65f : 1f) * obstacleMultiplier;
        }
    }

    public Quaternion LookAt2D(Vector2 target, float angleOffset = -90)
    {
        Vector2 fromTo = (target - (Vector2)transform.position).normalized;
        float zRotation = Mathf.Atan2(fromTo.y, fromTo.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0, 0, zRotation + angleOffset);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 8)
        {
            hasFinished = true;
            hasCrashed = true;
        }
    }

}
