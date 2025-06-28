using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [System.Serializable]
    public class EnemySpawnData
    {
        public GameObject prefab;
        public Vector3 position;
        public Quaternion rotation;
    }

    public Transform enemyParent; // Drag "Enemies" object here
    private List<EnemySpawnData> spawnDataList = new List<EnemySpawnData>();
    private List<GameObject> currentEnemies = new List<GameObject>();
    private GameObject[] enemyPrefabs;

    void Start()
    {
        LoadEnemyPrefabs();
        RegisterEnemiesAtStart();
    }

    void LoadEnemyPrefabs()
    {
        // Load specific enemy prefabs
        enemyPrefabs = Resources.LoadAll<GameObject>("Objects/Enemies");
        if (enemyPrefabs.Length == 0)
        {
            Debug.LogError("No enemy prefabs found in Resources/Objects/Enemies/");
        }
    }

    void RegisterEnemiesAtStart()
    {
        spawnDataList.Clear();
        currentEnemies.Clear();

        if (enemyParent == null)
        {
            Debug.LogError("EnemyManager: 'enemyParent' is not assigned.");
            return;
        }

        // Loop through all children under the enemy parent
        foreach (Transform child in enemyParent)
        {
            GameObject instance = child.gameObject;
            GameObject matchingPrefab = FindMatchingPrefab(instance);

            if (matchingPrefab != null)
            {
                spawnDataList.Add(new EnemySpawnData
                {
                    prefab = matchingPrefab,
                    position = instance.transform.position,
                    rotation = instance.transform.rotation
                });

                currentEnemies.Add(instance);
            }
            else
            {
                Debug.LogWarning($"No matching prefab found for: {instance.name}");
            }
        }
    }

    GameObject FindMatchingPrefab(GameObject instance)
    {
        // Check for specific enemy names
        if (instance.name.Contains("Tank"))
        {
            return FindPrefabByName("Tank");
        }
        else if (instance.name.Contains("Drone"))
        {
            return FindPrefabByName("Drone");
        }

        return null;
    }

    GameObject FindPrefabByName(string name)
    {
        foreach (var prefab in enemyPrefabs)
        {
            if (prefab.name == name) // Exact match for "Tank" or "Drone"
            {
                return prefab;
            }
        }

        return null;
    }

    public void ResetEnemies()
    {
        // Destroy all current enemies in the scene
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemy);
        }

        currentEnemies.Clear();

        // Instantiate new enemies from the spawn data list
        foreach (var data in spawnDataList)
        {
            GameObject newEnemy = Instantiate(data.prefab, data.position, data.rotation, enemyParent);
            currentEnemies.Add(newEnemy);
        }
    }
}
