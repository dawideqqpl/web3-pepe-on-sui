using UnityEngine;
using UnityEngine.UI;
using OPS;
using OPS.AntiCheat;
using OPS.AntiCheat.Prefs;
using OPS.AntiCheat.Field;


public class Pipes : MonoBehaviour
{
    public Transform top;
    public Transform bottom;
    public ProtectedFloat speed;
   
    public ProtectedFloat leftEdge;

    private void Start()
    {

        leftEdge = Camera.main.ScreenToWorldPoint(Vector3.zero).x - 1f;
    }

    private void Update()
    {
        transform.position += speed * Time.deltaTime * Vector3.left;

        if (transform.position.x < leftEdge) {
            Destroy(gameObject);
        }
    }

}
