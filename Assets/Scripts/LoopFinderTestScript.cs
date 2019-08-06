using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopFinderTestScript : MonoBehaviour
{
    [SerializeField] private List<GameObject> _points = new List<GameObject>();
    [SerializeField] private Material _mat;
    

    private void Start()
    {
        List<Vector2> actualPoints = new List<Vector2>();
        foreach (GameObject obj in _points)
        {
            actualPoints.Add(new Vector2(obj.transform.position.x, obj.transform.position.y));
        }

        List<int> loops = LoopFinder.FindLoops(actualPoints);

        // Loops work!
        foreach (int i in loops)
        {
            Debug.Log("Found a loop at index " + i); 
        }

        for (int i = 0; i < loops.Count; ++i)
        {
            List<GameObject> objs;
            LoopFinder.FindGameObjectsInLoop(i, "Barrel", out objs);
            foreach (GameObject obj in objs)
            {
                Destroy(obj);
            }

            var poly = LoopFinder.SpawnMeshOnLoop(i);
            poly.material = _mat;

            // List<Vector2> path = LoopFinder.GetLoop(i);
            // List<Vector3> v3path = new List<Vector3>();
            // foreach (Vector2 v2 in path)
            // {
            //     v3path.Add(new Vector3(v2.x, v2.y, 0));
            // }
            // List<Vector3> normals = new List<Vector3>();
            // Vector3 norm = new Vector3(0, 0, -1);
            // foreach (Vector2 v2 in path)
            // {
            //     normals.Add(norm);
            // }
            // 
            // int[] indices = Triangulator.StaticTrianglulate(path.ToArray());
            // 
            // MeshFilter meshFilter = poly.gameObject.AddComponent<MeshFilter>();
            // poly.gameObject.AddComponent<MeshRenderer>();
            // Mesh mesh = meshFilter.mesh;
            // 
            // mesh.Clear();
            // mesh.vertices = v3path.ToArray();
            // mesh.triangles = indices;
            // mesh.normals = normals.ToArray();
            // mesh.Optimize();
            // poly.GetComponent<Renderer>().material = _mat;
        }
    }
}
