using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LoopFinder
{
    // save the loops
    private static List<List<Vector2>> _loops = new List<List<Vector2>>();

    // return value is a list of loop start indices, that are the index of a point on which a loop is created
    static public List<int> FindLoops(List<Vector2> points, bool reverse = true)
    {
        if (reverse)
            points.Reverse();


        // --- SETUP ---

        // 1 Clear the current list
        _loops.Clear();

        // 2 Save all exploded indices
        List<int> explodedPoints = new List<int>();

        // 3 Create a list of ints to return
        List<int> result = new List<int>();


        // --- RUN --- 
        for (int i = 0; i < points.Count - 1; ++i)
        {
            // These are the points we check for intersection with, that are older
            for (int j = 0; j < (i - 1); ++j)
            {
                // Check if the curr J point is not already exploded
                if (explodedPoints.Contains(j + 1))
                    continue;

                // Only check for intersection with points that are already on fire (in the future) (so only older points)
                if (DoesIntersect(points[i], points[i + 1], points[j], points[j + 1]))
                {
                    // Intersection found.
                    // Add to result
                    if (reverse)
                        result.Add((points.Count - (i + 1)));
                    else
                        result.Add(i + 1);

                    // Add affected points (J + 1 until I + 1) to a new Loop
                    // Add affected points to exploded list
                    List<Vector2> newLoop = new List<Vector2>();
                    int tempIdx = j + 1;
                    while (tempIdx != i + 1)
                    {
                        if (!explodedPoints.Contains(tempIdx))
                        {
                            explodedPoints.Add(tempIdx);
                            newLoop.Add(points[tempIdx]);
                        }
                        tempIdx++;
                    }
                    _loops.Add(newLoop);
                }
            }
        }


        // --- END ---
        if (reverse)
            points.Reverse();

        return result;
    }

    // Input nr is the index of the loop we are at
    static public void FindGameObjectsInLoop(int loopNr, string tag, out List<GameObject> objects)
    {
        objects = new List<GameObject>();
        GameObject polyObj = new GameObject();
        var poly = polyObj.AddComponent<PolygonCollider2D>();
        // needs self closing list because else the poly looks funny
        List<Vector2> selfclosingList = new List<Vector2>();
        selfclosingList.AddRange(_loops[loopNr]);
        selfclosingList.Add(_loops[loopNr][0]);
        poly.SetPath(0, selfclosingList);

        // Debug.Log("Size: " + poly.bounds.size.magnitude);
        
        GameObject[] gameObjectsToCollide = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject obj in gameObjectsToCollide)
        {
            if (poly.OverlapPoint(new Vector2(obj.transform.position.x, obj.transform.position.y)))
            {
                objects.Add(obj);
            }
        }

        GameObject.Destroy(polyObj);
    }




    // Intersection math ref: https://www.geeksforgeeks.org/check-if-two-given-line-segments-intersect/
    private static bool DoesIntersect(Vector2 pointA, Vector2 pointB, Vector2 pointC, Vector2 pointD)
    {
        // Find the four orientations needed for general and 
        // special cases 
        int o1 = GetOrientation(pointA, pointB, pointC);
        int o2 = GetOrientation(pointA, pointB, pointD);
        int o3 = GetOrientation(pointC, pointD, pointA);
        int o4 = GetOrientation(pointC, pointD, pointB);

        // General case 
        if (o1 != o2 && o3 != o4)
            return true;

        // Special Cases 
        // p1, q1 and p2 are colinear and p2 lies on segment p1q1 
        if (o1 == 0 && IsOnSegment(pointA, pointC, pointB))
            return true;

        // p1, q1 and q2 are colinear and q2 lies on segment p1q1 
        if (o2 == 0 && IsOnSegment(pointA, pointD, pointB))
            return true;

        // p2, q2 and p1 are colinear and p1 lies on segment p2q2 
        if (o3 == 0 && IsOnSegment(pointC, pointA, pointD))
            return true;

        // p2, q2 and q1 are colinear and q1 lies on segment p2q2 
        if (o4 == 0 && IsOnSegment(pointC, pointB, pointD))
            return true;

        return false;
    }
    private static bool IsOnSegment(Vector2 pointA, Vector2 pointB, Vector2 pointC)
    {
        if (pointB.x <= Mathf.Max(pointA.x, pointC.x) && pointB.x >= Mathf.Min(pointA.x, pointC.x) &&
        pointB.y <= Mathf.Max(pointA.y, pointC.y) && pointB.y >= Mathf.Min(pointA.y, pointC.y))
            return true;

        return false;
    }
    private static int GetOrientation(Vector2 pointA, Vector2 pointB, Vector2 pointC)
    {
        float val = (pointB.y - pointA.y) * (pointC.x - pointB.x) - (pointB.x - pointA.x) * (pointC.y - pointB.y);

        if (Mathf.Abs(val) <= 0.001f) return 0;

        return (val > 0) ? 1 : 2;
    }




    // Utility
    static public List<Vector2> GetLoop(int idx)
    {
        if (idx >= _loops.Count)
        {
            Debug.Log("Tried to get loop vertices of a higher index than available");
            return new List<Vector2>();
        }
        return _loops[idx];
    }
    static public MeshRenderer SpawnMeshOnLoop(int idx)
    {
        if (idx >= _loops.Count)
        {
            Debug.Log("Tried to get loop vertices of a higher index than available in SpawnMeshOnLoop");
            return null;
        }

        GameObject polyObj = new GameObject();
        var poly = polyObj.AddComponent<PolygonCollider2D>();
        // needs self closing list because else the poly looks funny
        List<Vector2> selfclosingList = new List<Vector2>();
        selfclosingList.AddRange(_loops[idx]);
        selfclosingList.Add(_loops[idx][0]);
        poly.SetPath(0, selfclosingList);

        var path = _loops[idx];

        List<Vector3> v3path = new List<Vector3>();
        foreach (Vector2 v2 in path)
        {
            v3path.Add(new Vector3(v2.x, v2.y, 0));
        }
        List<Vector3> normals = new List<Vector3>();
        Vector3 norm = new Vector3(0, 0, -1);
        foreach (Vector2 v2 in path)
        {
            normals.Add(norm);
        }

        int[] indices = Triangulator.StaticTrianglulate(path.ToArray());

        MeshFilter meshFilter = poly.gameObject.AddComponent<MeshFilter>();
        var result = poly.gameObject.AddComponent<MeshRenderer>();
        Mesh mesh = meshFilter.mesh;

        mesh.Clear();
        mesh.vertices = v3path.ToArray();
        mesh.triangles = indices;
        mesh.normals = normals.ToArray();
        mesh.Optimize();

        return result;
    }
}
