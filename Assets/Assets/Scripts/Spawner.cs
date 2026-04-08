using UnityEngine;

using OPS;
using OPS.AntiCheat;
using OPS.AntiCheat.Field;
using OPS.AntiCheat.Prefs;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    public GameObject prefabPizza;
    public ProtectedFloat spawnRate = 1f;
    public ProtectedFloat spawnRateMeme;
    public ProtectedFloat spawnRatePizza = 1f;
    public ProtectedFloat minHeight = -1f;
    public ProtectedFloat maxHeight = 2f;


    public Sprite[] memeSprites;
    public GameObject meme;

    private void OnEnable()
    {
        InvokeRepeating(nameof(Spawn), spawnRate, spawnRate);
        InvokeRepeating(nameof(SpawnMeme), spawnRateMeme, spawnRateMeme);
        InvokeRepeating(nameof(SpawnPizza), 0.5f, spawnRatePizza);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(Spawn));
    }

    private void Spawn()
    {

       
        GameObject pipes = Instantiate(prefab, transform.position, Quaternion.identity);
        pipes.transform.position += Vector3.up * Random.Range(minHeight, maxHeight);
    }

    private void SpawnMeme()
    {
        GameObject memes = Instantiate(meme, transform.position, Quaternion.identity);
        memes.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = memeSprites[Random.Range(0, memeSprites.Length)];
    }
    private void SpawnPizza()
    {
        GameObject pizza = Instantiate(prefabPizza, transform.position, Quaternion.identity);
        pizza.transform.position += Vector3.up * Random.Range(minHeight -2, maxHeight + 2);
    }
}
