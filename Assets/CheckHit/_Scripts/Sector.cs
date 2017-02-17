using UnityEngine;

public class Sector : MonoBehaviour
{
    public float _radius;
    public float _angleDegree;
    public int _segments;
    private Mesh sectorMesh;

    void Awake()
    {
        sectorMesh = CreateMesh(_radius, _angleDegree, _segments);

        GetComponent<MeshFilter>().mesh = sectorMesh;
    }

    /// <summary>
    /// 得到扇形坐标，0:圆点，1:左顶点，2:右顶点
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public Vector3 GetVertex(int i)
    {
        Vector3 v = gameObject.transform.localPosition;

        float currAngle = Mathf.Deg2Rad * _angleDegree / 2;

        switch (i)
        {
            case 0:
                break;
            case 1:
                v += new Vector3(Mathf.Cos(currAngle) * _radius, 0, Mathf.Sin(currAngle) * _radius);
                break;
            case 2:
                v += new Vector3(Mathf.Cos(-currAngle) * _radius, 0, Mathf.Sin(-currAngle) * _radius);
                break;
        }

        return v;
    }


    /// <summary>
    /// 创建扇形的Mesh
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="angleDegree"></param>
    /// <param name="segments"></param>
    /// <returns></returns>
    private Mesh CreateMesh(float radius,float angleDegree,int segments)
    {
        Mesh mesh = new Mesh();
        mesh.name = "sector";

        Vector3[] vertices = new Vector3[segments + 2];
        //vertices[0] = sector.transform.localPosition;
        vertices[0] = new Vector3(0, 0, 0);

        Debug.Log("扇形圆点：" + vertices[0]);

        float angle = Mathf.Deg2Rad * angleDegree;
        float currAngle = angle / 2;
        float deltaAngle = angle / segments;
        for (int i = 1; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(Mathf.Cos(currAngle) * radius, 0, Mathf.Sin(currAngle) * radius) + vertices[0];
            currAngle -= deltaAngle;
        }

        Debug.Log("顶点坐标：" + vertices[1] + "," + vertices[segments + 1]);

        int[] triangles = new int[segments * 3];
        for (int i = 0, vi = 1; i < triangles.Length; i += 3, vi++)
        {
            triangles[i] = 0;
            triangles[i + 1] = vi;
            triangles[i + 2] = vi + 1;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        return mesh;
    }
}