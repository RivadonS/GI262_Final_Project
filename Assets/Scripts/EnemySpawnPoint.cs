using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] possibleEnemies; // ใส่ Prefab ของ Patrol หรือ Flying ตรงนี้

    private void Start()
    {
        if (possibleEnemies.Length == 0) return;

        // 1. สุ่มเลือกศัตรูจาก Array
        int randomIndex = Random.Range(0, possibleEnemies.Length);
        GameObject selectedEnemy = possibleEnemies[randomIndex];

        // 2. เสกศัตรูออกมาที่ตำแหน่งของ Node นี้
        Instantiate(selectedEnemy, transform.position, Quaternion.identity);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 1f, 0.5f); // สีฟ้าโปร่งแสง
        Gizmos.DrawCube(transform.position, new Vector3(1, 1, 1));
    }
}