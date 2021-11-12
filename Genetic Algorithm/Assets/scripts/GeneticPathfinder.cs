using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticPathfinder : MonoBehaviour
{
    public float creatureSpeed;
    public float pathMultiplier;
    public float rotationSpeed;
    int pathIndex = 0;
    public DNA dna;
    public bool hasFinished = false;
    bool hasBeenInitialized = false;
    List<Vector2> travelledPath = new List<Vector2>();
    Vector2 target;
    Vector2 nextPoint;
    Quaternion targetRotation;
    LineRenderer lr;

   // private void Awake()
    //{
      // InitCreature(new DNA(), Vector2.zero);
    //}

    public void InitCreature(DNA newDna, Vector2 _target)
    {
        travelledPath.Add(transform.position);
        lr = GetComponent<LineRenderer>();
        dna = newDna;
        target = _target;
        nextPoint = transform.position;
        travelledPath.Add(nextPoint);
        hasBeenInitialized = true;
    }
    private void Update()
    {
        
        if (hasBeenInitialized && !hasFinished)
        {
            if (pathIndex == dna.genes.Count || Vector2.Distance(transform.position, target) < 0.5f)
            {
                hasFinished = true;
            }
            if ((Vector2)transform.position == nextPoint)
            {
                nextPoint = (Vector2)transform.position + (dna.genes[pathIndex] * pathMultiplier);
                travelledPath.Add(nextPoint);
                targetRotation = LookAt2D(nextPoint);
                pathIndex++;
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, nextPoint, creatureSpeed * Time.deltaTime);
            }
            if (transform.rotation != targetRotation)
            {
                transform.rotation=Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            //Slerp gives a straightest and shortest path between its quaternion end points, and maps to a rotation through an angle of 2Ω.
            //RenderLine();
        }
    }
   
    /*public void RenderLine()
    {
        List<Vector3> linePoints = new List<Vector3>();
        if(travelledPath.Count > 2)
        {
            for (int i = 0; i < travelledPath.Count - 1; i++)
            {
                linePoints.Add(travelledPath[i]);
            }
            linePoints.Add(transform.position);
        }
        else
        {
            linePoints.Add(travelledPath[0]);
            linePoints.Add(transform.position);
        }

        lr.positionCount = linePoints.Count;
        lr.SetPositions(linePoints.ToArray());
        
    }*/
    public float fitness
    {
        get{
            float dist = Vector2.Distance(transform.position, target);
            if (dist == 0)
            {
                dist = 0.0001f;
            }
            return 60 / dist;
        }
    }
    public Quaternion LookAt2D(Vector2 target, float angleOffset = -90)
    {
        Vector2 fromTo =(target - (Vector2)transform.position).normalized;
        float zRotation = Mathf.Atan2(fromTo.y, fromTo.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0, 0, zRotation + angleOffset);
        //Euler generates a Quaternion that represents the orientation or relative rotation specified by the Euler angles provided as inputs.
    }
}
