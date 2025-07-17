using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VisionCone2D : MonoBehaviour
{
    [Header("VisionCone Settings")]
    [SerializeField] private float viewDistance = 5f;       // дистанция зоны видимости
    [SerializeField] private float viewAngle = 90f;         // Угол конуса зоны видимости
    [SerializeField] private int rayCount = 50;             // лучи в конусе
    [SerializeField] private LayerMask obstacleMask;        // маска, которая загораживает зону видимости

    private Mesh mesh;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void LateUpdate()
    {
        GenerateViewMesh();
    }

    private void GenerateViewMesh()
    {
        float angleStep = viewAngle / rayCount;
        float startAngle = -viewAngle / 2f;
        List<Vector3> vertices = new List<Vector3> { Vector3.zero };
        List<int> triangles = new List<int>();

        for (int i = 0; i <= rayCount; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector3 dir = DirFromAngle(angle);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, viewDistance, obstacleMask);
            Vector3 vertex = hit ? (Vector3)(hit.point - (Vector2)transform.position) : dir * viewDistance;

            vertices.Add(vertex);

            if (i > 0)
            {
                triangles.Add(0);
                triangles.Add(i);
                triangles.Add(i + 1);
            }
        }


        //Debug.Log("Vertices count: " + mesh.vertexCount);

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    private Vector3 DirFromAngle(float angle)
    {
        float rad = (angle + transform.eulerAngles.z) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad));
    }
}
