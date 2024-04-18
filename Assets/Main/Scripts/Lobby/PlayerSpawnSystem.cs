using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSpawnSystem : MonoBehaviour
{
    private static List<Transform> spawnPoints = new List<Transform>();


    public static void AddSpawnPoint(Transform transform)
    {
        spawnPoints.Add(transform);

        spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
    }

    public static void RemoveSpawnPoint(Transform transform) => spawnPoints.Remove(transform);

    public static Vector3 GetRandomPoint()
    {
        if (spawnPoints.Count == 0)
        {
            return Vector3.zero;
        }

        return spawnPoints[Random.Range(0, spawnPoints.Count)].position;
    }
}