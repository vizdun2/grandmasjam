using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wobble : MonoBehaviour
{
    public float speed = 1;
    public Vector2 diff;

    Vector3 original;

    // Start is called before the first frame update
    void Start()
    {
        original = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position =
            (Vector3)Vector2.Lerp((Vector2)original - diff, (Vector2)original + diff,
                Math.Abs((Time.time * speed) % 2 - 1)) + new Vector3(0, 0, original.z);
    }
}