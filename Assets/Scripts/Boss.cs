using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour
{
    private int bossHealth = 50;
    private bool invulnerable = false;
    private int phase = 0;

    [Header("Blast")]
    [SerializeField] private GameObject blastPrefab;
    [SerializeField] private Transform blastSpawnPoint;
    [SerializeField] private float blastSpawnPointRadius = 0.5f;
    [SerializeField] private float blastSpawnInterval;
    [SerializeField] private int numberOfBlasts;

    [Header("Vortex")]
    [SerializeField] private GameObject vortexPrefab;
    [SerializeField] private float vortexSpawnInterval;
    [SerializeField] private int numberOfVortexes;

    [Header("Beam")]
    [SerializeField] private GameObject beamPrefab;
    [SerializeField] private float beamYPosition;
    [SerializeField] private float beamSpawnInterval;
    [SerializeField] private int numberOfBeams;

    [Header("References")]
    [SerializeField] private Transform bossHitBox;
    [SerializeField] private Transform player;

    // Optional: Allow setting hit box size in inspector
    [Header("Hitbox Settings")]
    [SerializeField] private Vector3 bossHitBoxSize = new Vector3(1f, 1f, 1f); // Set default size

    private void Start()
    {
        StartCoroutine(Phase1());
    }

    private IEnumerator SpawnAttack(GameObject prefab, Vector3 spawnPosition, float interval, int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (prefab != null)
            {
                Instantiate(prefab, spawnPosition, Quaternion.identity);
                Debug.Log($"Spawned prefab {i + 1} of {count}");
            }
            yield return new WaitForSeconds(interval);
        }
    }

    private IEnumerator Blast()
    {
        return SpawnAttack(blastPrefab, blastSpawnPoint.position, blastSpawnInterval, numberOfBlasts);
    }

    private IEnumerator Vortex()
    {
        return SpawnAttack(vortexPrefab, player.position, vortexSpawnInterval, numberOfVortexes);
    }

    private IEnumerator Beam()
    {
        for (int i = 0; i < numberOfBeams; i++)
        {
            Vector3 beamSpawnPosition = new Vector3(player.position.x, beamYPosition, 0);
            yield return SpawnAttack(beamPrefab, beamSpawnPosition, beamSpawnInterval, 1);
        }
    }

    private IEnumerator Phase1()
    {
        if (phase >= 1) yield break; // Prevent re-entering Phase1
        phase = 1;
        Debug.Log("Starting phase 1");
        invulnerable = true;
        yield return Blast();
        invulnerable = false;
        Debug.Log("Phase 1 complete, boss is now vulnerable!");
    }

    private IEnumerator Phase2()
    {
        if (phase >= 2) yield break; // Prevent re-entering Phase2
        phase = 2;
        Debug.Log("Starting phase 2");
        invulnerable = true;

        StartCoroutine(Blast());
        yield return Vortex();
        invulnerable = false;
        Debug.Log("Phase 2 complete, boss is now vulnerable!");
    }

    private IEnumerator Phase3()
    {
        if (phase >= 3) yield break; // Prevent re-entering Phase3
        phase = 3;
        Debug.Log("Starting phase 3");

        StartCoroutine(Blast());
        StartCoroutine(Vortex());
        StartCoroutine(Beam());
    }

    public void BossHit(int damage)
    {
        if (invulnerable)
        {
            Debug.Log("Boss is attacking and can't take damage!");
            return;
        }

        bossHealth -= damage;
        Debug.Log($"Boss took {damage} damage! Current health: {bossHealth}");

        if (bossHealth <= 0)
        {
            StartCoroutine(BossDeath());
        }
        else if (bossHealth <= 30 && phase < 3)
        {
            StartCoroutine(Phase3());
        }
        else if (bossHealth <= 40 && phase < 2)
        {
            StartCoroutine(Phase2());
        }
    }

    private IEnumerator BossDeath()
    {
        Debug.Log("Boss defeated!");
        // Add death animation (will do myself)
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (bossHitBox != null)
        {
            Gizmos.color = Color.red;
            // Use custom hitbox size if defined in the inspector
            Gizmos.DrawWireCube(bossHitBox.position, bossHitBoxSize);
            Gizmos.DrawWireSphere(blastSpawnPoint.position, blastSpawnPointRadius);
        }
    }
}