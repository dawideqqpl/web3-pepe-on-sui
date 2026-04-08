using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OPS;
using OPS.AntiCheat;
using OPS.AntiCheat.Field;
using OPS.AntiCheat.Prefs;


public class MemesScript : MonoBehaviour
{
    // Start is called before the first frame update
    public ProtectedFloat speed;

    public ProtectedFloat leftEdge;


    private void Start()
    {

        leftEdge = Camera.main.ScreenToWorldPoint(Vector3.zero).x - 3f;
    }

    private void Update()
    {
        transform.position += speed * Time.deltaTime * Vector3.left;

        if (transform.position.x < leftEdge)
        {
            Destroy(gameObject);
        }
    }
}
