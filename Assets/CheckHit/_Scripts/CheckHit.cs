using UnityEngine;
using UnityEngine.UI;

public class CheckHit : MonoBehaviour
{
    public GameObject _sphere;
    public GameObject _sector;
    public float _radius;
    public Button checkButton;
    public Text checkText;

    void Awake()
    {
        _sphere.transform.localScale *= _radius;

        checkButton.onClick.AddListener(Check);
    }

    public void Check()
    {
        bool result = _CheckHit(_sphere, _sector);

        if (result)
        {
            checkText.text = "相交";
        }
        else
        {
            checkText.text = "不相交";
        }
    }

    /// <summary>
    /// 返回V向量在 V1和V2平面上的投影向量
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    public Vector3 GetProjection(Vector3 v1,Vector3 v2,Vector3 v)
    {
        Vector3 n = Vector3.Cross(v1, v2).normalized;//V1,V2平面的单位法向量

        return v - Vector3.Dot(v, n) * n;
    }

    public bool _CheckHit(GameObject sphere, GameObject sector)
    {
        bool check = false;

        float sphereRadius = _radius / 2;//球半径
        Vector3 o = sphere.transform.localPosition;//球心

        Sector sectorObj = sector.GetComponent<Sector>();
        //扇形的3个顶点
        Vector3 a = sectorObj.GetVertex(0);
        Vector3 b = sectorObj.GetVertex(1);
        Vector3 c = sectorObj.GetVertex(2);
        float angleDegree = sectorObj._angleDegree;//扇形夹角
        float angle = Mathf.Deg2Rad * angleDegree / 2;//半角弧度
        float sectorRadius = sectorObj._radius;//扇形长度

        //Debug.Log("A:" + a + ",B:" + b + ",C:" + c);

        //扇形平面
        Plane sectorPlane = new Plane(a, b, c);
        float distancePointToPlane = Mathf.Abs(sectorPlane.GetDistanceToPoint(o));
        Debug.Log("球心到扇形平面距离：" + distancePointToPlane);

        if(distancePointToPlane > sphereRadius)
        {
            check = false;
        }
        else
        {
            Vector3 ab = b - a;
            Vector3 ac = c - a;
            Vector3 ao = o - a;
            Vector3 u = ( ab + ac ).normalized;//扇形方向上的单位向量
            Vector3 aoo = GetProjection(ab, ac, ao);//扇形原点到球心向量在扇形平面上的投影向量
            Debug.Log("扇形到球心向量在扇形平面的投影：" + aoo);

            float sqrRadius = Mathf.Sqrt(( sphereRadius * sphereRadius ) - ( ao - aoo ).sqrMagnitude);//扇形平面横切球体出来的圆的半径
            Debug.Log("横切圆的半径:" + sqrRadius);

            float distancePointToPoint = aoo.sqrMagnitude;//扇形原点到球心投影的距离平方
            Debug.Log("球心到扇形原点距离：" + Mathf.Sqrt(distancePointToPoint));

            if (distancePointToPoint > ( sectorRadius + sqrRadius ) * ( sectorRadius + sqrRadius ))
            {
                check = false;
            }
            else
            {
                float px = Vector3.Dot(aoo, u);

                if (px > ( aoo.magnitude * Mathf.Cos(angle) ))
                {
                    check = true;
                }
                else
                {
                    Vector3 oo = new Vector3(aoo.x + a.x, aoo.y + a.y, aoo.z + a.z);

                    Debug.Log("横切圆圆心：" + oo);

                    float disAB = SegmentPointSqrDistance(a, b, oo);
                    float disAC = SegmentPointSqrDistance(a, c, oo);
                    Debug.Log("横切圆圆心到ab线段距离：" + disAB);
                    Debug.Log("横切圆圆心到ac线段距离：" + disAC);

                    if (( disAB > ( sqrRadius * sqrRadius ) ) && ( disAC > ( sqrRadius * sqrRadius ) ))
                    {
                        check = false;
                    }
                    else
                    {
                        check = true;
                    }
                }
            }   
        }

        return check;
    }

    /// <summary>
    /// 返回x点到xo,u线段的最短距离
    /// </summary>
    /// <param name="x0"></param>
    /// <param name="u"></param>
    /// <param name="x"></param>
    /// <returns></returns>
    public float SegmentPointSqrDistance(Vector3 x0, Vector3 u, Vector3 x)
    {
        float t = Vector3.Dot(x - x0, u) / u.sqrMagnitude;
        return ( x - ( x0 + Mathf.Clamp(t, 0, 1) * u ) ).sqrMagnitude;
    }
}