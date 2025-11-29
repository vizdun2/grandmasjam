using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Cable : MonoBehaviour
{
    public Vector2 from;
    public Vector2 to;

    // Update is called once per frame
    void Update()
    {
        // Vector3 to = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Debug.Log(to);
        Vector2 direction = to - from;
        Quaternion rotation = Quaternion.FromToRotation(Vector2.right, direction);
        transform.rotation = rotation;
        transform.localScale = new Vector3(Vector2.Distance(from, to), 1, 1);
        transform.position = (Vector3)from + new Vector3(0, 0, -0.15f);
    }
}