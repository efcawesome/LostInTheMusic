using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour
{
    public List<Sprite> clouds;
    public float spawnTime;
    private float spawnTimer;
    public float spawnVariability;
    public float vertRange;
    public float cloudSpeed;
    public float cloudSpeedVariability;

    public GameObject cloudPrefab;

    private void Start()
    {
        spawnTimer = spawnTime;
    }
    // Update is called once per frame
    void Update()
    {
        if (spawnTimer > 0) spawnTimer -= Time.deltaTime;
        else
        {
            spawnTimer = spawnTime + Random.Range(-spawnVariability, spawnVariability);
            SpawnCloud();
        }
    }

    private void SpawnCloud()
    {
        GameObject cloud = Instantiate(cloudPrefab);
        cloud.transform.parent = transform;
        cloud.transform.localPosition = new Vector2(0, Random.Range(-vertRange, vertRange));
        cloud.GetComponent<CloudMovement>().speed = cloudSpeed + Random.Range(-cloudSpeedVariability, cloudSpeedVariability);
        cloud.GetComponent<SpriteRenderer>().sprite = clouds[Random.Range(0, clouds.Count)];
        cloud.SetActive(true);
    }
}
