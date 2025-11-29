using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Cable : MonoBehaviour
{
    public Vector2 from;
    public Vector2 to;

    Transform middle;
    Transform start;
    Transform end;

    void Start()
    {
        middle = transform.Find("Mid");
        start = transform.Find("Start");
        end = transform.Find("End");
    }

    void Update()
    {

        Vector2 direction = to - from;
        Quaternion rotation = Quaternion.FromToRotation(Vector2.right, direction);

        middle.rotation = rotation;
        middle.localScale = new Vector3(Vector2.Distance(from, to), 1, 1);
        middle.position = (Vector3)from + new Vector3(0, 0, -0.15f);

        start.position = (Vector3)from + new Vector3(0, 0, -0.35f);
        start.rotation = rotation;
        end.position = (Vector3)to + new Vector3(0, 0, -0.35f);
        end.rotation = rotation;
    }
}