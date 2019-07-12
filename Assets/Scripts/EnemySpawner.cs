using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {

    // The radius of the spawn area
    public float radius = 250.0f;

    // The asteroids to spawn
    public Rigidbody enemyPrefab;

    // Wait spawnRate ± variance seconds between each asteroid
    public float spawnRate = 5.0f;
    public float variance = 1.0f;

    // The object to aim the asteriods at
    public Transform target;

    // If false, disable spawning
    public bool spawnEnemy = false;

    void Start()
    {
        // Start the coroutine that creates asteroids immediately
        StartCoroutine(CreateEnemies());
    }

    IEnumerator CreateEnemies()
    {

        // Loop forever
        while (true)
        {

            // Work out when the next asteroid should appear
            float nextSpawnTime = spawnRate + Random.Range(-variance, variance);

            // Wait that much time
            yield return new WaitForSeconds(nextSpawnTime);

            // Additionally, wait until physics is about to update
            yield return new WaitForFixedUpdate();

            // Create the asteroid
            CreateNewEnemy();
        }

    }

    void CreateNewEnemy()
    {

        // If we're not currently spawning asteroids, bail out
        if (spawnEnemy == false)
        {
            return;
        }

        // Randomly select a point on the surface of the sphere
        var enemyPosition = Random.onUnitSphere * radius;

        // Scale this by the object's scale
        enemyPosition.Scale(transform.lossyScale);

        // And offset it by the asteroid spawner's location
        enemyPosition += transform.position;

        // Create the new asteroid
        var newEnemy = Instantiate(enemyPrefab);

        // Place it at the spot we just calculated
        newEnemy.transform.position = enemyPosition;

        // Aim it at the target
        newEnemy.transform.LookAt(target);
    }

    // Called by the editor while the spawner object is selected.
    void OnDrawGizmosSelected()
    {

        // We want to draw yellow stuff
        Gizmos.color = Color.yellow;

        // Tell the Gizmos drawer to use our current position and scale
        Gizmos.matrix = transform.localToWorldMatrix;

        // Draw a sphere representing the spawn area
        Gizmos.DrawWireSphere(Vector3.zero, radius);
    }

    public void DestroyAllEnemies()
    {
        // Remove all asteroids in the game
        foreach (var enemy in FindObjectsOfType<Enemy>())
        {
            Destroy(enemy.gameObject);
        }
    }

  
}
