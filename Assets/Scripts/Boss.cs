using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour {
    public static Boss Instance { get; private set; }
    private int bossHealth = 50;
    private bool invulnerable = false;
    private int phase = 0;
    Animator anim;

    [Header("Blast")]
    [SerializeField] private GameObject blastPrefab;
    [SerializeField] private Transform blastSpawnPoint;
    [SerializeField] private float blastSpawnPointRadius = 0.5f;

    [Header("Vortex")]
    [SerializeField] private GameObject vortexPrefab;

    [Header("Beam")]
    [SerializeField] private GameObject beamPrefab;
    [SerializeField] private float beamYPosition;

    [Header("References")]
    [SerializeField] private Transform player;

    private void Start()
        {
            if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Ensure there's only one boss instance
        }
        player = FindFirstObjectByType<PlayerController>().transform;
        anim = GetComponent<Animator>();
        StartCoroutine(Phase1());
    }

    private IEnumerator SpawnAttack(GameObject prefab, Vector3 spawnPosition, float interval, int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (prefab != null)
            {
                Instantiate(prefab, spawnPosition, Quaternion.identity);
            }
            yield return new WaitForSeconds(interval);
        }
    }

    private IEnumerator Blast(float spawnInterval, int numberOfBlasts)
    {
        yield return SpawnAttack(blastPrefab, blastSpawnPoint.position, spawnInterval, numberOfBlasts);
    }

    private IEnumerator Vortex(float spawnInterval, int numberOfVortexes)
    {
        yield return SpawnAttack(vortexPrefab, player.position, spawnInterval, numberOfVortexes);
    }

    private IEnumerator Beam(float spawnInterval, int numberOfBeams)
    {
        for (int i = 0; i < numberOfBeams; i++)
        {
            Vector3 beamSpawnPosition = new Vector3(player.position.x, beamYPosition, 0);
            yield return SpawnAttack(beamPrefab, beamSpawnPosition, spawnInterval, 1);
        }
    }

    private IEnumerator Phase1()
    {
        if (phase >= 1) yield break; // Prevent re-entering Phase1
        phase = 1;
        Debug.Log("Starting phase 1");
        invulnerable = true;

        // Pass custom parameters for blast attack
        yield return new WaitForSeconds(3);
        yield return Blast(1f, 10);

        invulnerable = false;
        anim.SetTrigger("Open");
        Debug.Log("Phase 1 complete, boss is now vulnerable!");
    }

    private IEnumerator Phase2()
    {
        if (phase >= 2) yield break; // Prevent re-entering Phase2
        phase = 2;
        Debug.Log("Starting phase 2");
        invulnerable = true;
        anim.SetTrigger("Close");

        // Pass custom parameters for blast and vortex attacks
        yield return new WaitForSeconds(3);
        StartCoroutine(Blast(0.5f, 60));
        yield return Vortex(3f, 10);

        invulnerable = false;
        anim.SetTrigger("Open");
        Debug.Log("Phase 2 complete, boss is now vulnerable!");
    }

    private IEnumerator Phase3()
    {
        if (phase >= 3) yield break; // Prevent re-entering Phase3
        phase = 3;
        Debug.Log("Starting phase 3");
        invulnerable = true;
        anim.SetTrigger("Close");

        // Pass custom parameters for all attacks
        yield return new WaitForSeconds(3);
        StartCoroutine(Blast(3f, 10000));
        StartCoroutine(Beam(12f, 10000));

        yield return new WaitForSeconds(10);
        invulnerable = false;
        anim.SetTrigger("Open");
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(blastSpawnPoint.position, blastSpawnPointRadius);
    }
}