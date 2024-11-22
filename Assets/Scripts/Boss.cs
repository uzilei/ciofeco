using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Boss : MonoBehaviour {
    public static Boss Instance { get; private set; }
    private int bossHealth = 40;
    private bool invulnerable = false;
    private int phase = 0;
    Animator anim;

    [Header("Blast")]
    [SerializeField] private GameObject blastPrefab;
    [SerializeField] private Transform blastSpawnPoint;
    [SerializeField] private float blastSpawnPointRadius = 0.5f;

    [Header("Blast2")]
    [SerializeField] private Transform blastSpawnPoint2;

    [Header("Vortex")]
    [SerializeField] private GameObject vortexPrefab;

    [Header("Beam")]
    [SerializeField] private GameObject beamPrefab;
    [SerializeField] private float beamYPosition;
    Transform player;

    private void Awake() {
        AssignPlayer();
    }
    
    private void Start() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
        AssignPlayer();
        anim = GetComponent<Animator>();
        StartCoroutine(Phase1());
    }

    private void FixedUpdate() {
        AssignPlayer();
    }

    private void AssignPlayer() {
        if (player == null) {
            PlayerController foundPlayer = FindFirstObjectByType<PlayerController>();
            if (foundPlayer != null) {
                player = foundPlayer.transform;
            }
        }
    }

    private IEnumerator SpawnAttack(GameObject prefab, Vector3 spawnPosition, float interval, int count) {
        for (int i = 0; i < count; i++) {
            if (prefab != null) {
                Instantiate(prefab, spawnPosition, Quaternion.identity);
            }
            yield return new WaitForSeconds(interval);
        }
    }

    private IEnumerator Blast(float spawnInterval, int numberOfBlasts) {
        yield return SpawnAttack(blastPrefab, blastSpawnPoint.position, spawnInterval, numberOfBlasts);
    }

    private IEnumerator Blast2(float spawnInterval, int numberOfBlasts) {
        yield return SpawnAttack(blastPrefab, blastSpawnPoint2.position, spawnInterval, numberOfBlasts);
    }

    private IEnumerator Vortex(float spawnInterval, int numberOfVortexes) {
        for (int i = 0; i < numberOfVortexes; i++) {
            if (vortexPrefab != null) {
                Vector3 currentPlayerPosition = player.position;
                Instantiate(vortexPrefab, currentPlayerPosition, Quaternion.identity);
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }


    private IEnumerator Beam(float spawnInterval, int numberOfBeams) {
        for (int i = 0; i < numberOfBeams; i++) {
            Vector3 beamSpawnPosition = new Vector3(player.position.x, beamYPosition, 0);
            yield return SpawnAttack(beamPrefab, beamSpawnPosition, spawnInterval, 1);
        }
    }

    private IEnumerator Phase1() {
        while (player == null) {
            yield return null;
        }

        if (phase >= 1) yield break;
        phase = 1;
        Debug.Log("Starting phase 1");
        invulnerable = true;

        yield return new WaitForSeconds(2);
        StartCoroutine(Beam(1f, 1));
        yield return new WaitForSeconds(10);
        StartCoroutine(Blast(1f, 10));
        yield return Blast2(1f, 10);

        invulnerable = false;
        anim.SetTrigger("Open");
        Debug.Log("Phase 1 complete");
    }

    private IEnumerator Phase2() {
        if (phase >= 2) yield break;
        phase = 2;
        Debug.Log("Starting phase 2");
        invulnerable = true;
        anim.SetTrigger("Close");

        yield return new WaitForSeconds(3);
        StartCoroutine(Blast(0.5f, 40));
        StartCoroutine(Blast2(0.5f, 40));
        yield return Vortex(2f, 10);

        invulnerable = false;
        anim.SetTrigger("Open");
        Debug.Log("Phase 2 complete");
    }

    private IEnumerator Phase3() {
        if (phase >= 3) yield break;
        phase = 3;
        Debug.Log("Starting phase 3");
        invulnerable = true;
        anim.SetTrigger("Close");

        yield return new WaitForSeconds(3);
        StartCoroutine(Blast(2f, 10000));
        StartCoroutine(Blast2(3f, 10000));
        StartCoroutine(Beam(8f, 10000));
        StartCoroutine(Vortex(4f, 10000));

        yield return new WaitForSeconds(10);
        invulnerable = false;
        anim.SetTrigger("Open");
    }

    public void BossHit(int damage) {
        if (invulnerable) {
            Debug.Log("Boss is attacking and can't take damage");
            return;
        }

        bossHealth -= damage;
        Debug.Log($"Boss took {damage} damage! Current health: {bossHealth}");

        if (bossHealth <= 0) {
            StartCoroutine(BossDeath());
        }
        else if (bossHealth <= 20 && phase < 3) {
            StartCoroutine(Phase3());
        }
        else if (bossHealth <= 30 && phase < 2) {
            StartCoroutine(Phase2());
        }
    }

    private IEnumerator BossDeath() {
        Debug.Log("GG");
        anim.SetTrigger("Death");
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
        SceneManager.LoadScene("FinalScene");
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(blastSpawnPoint.position, blastSpawnPointRadius);
        Gizmos.DrawWireSphere(blastSpawnPoint2.position, blastSpawnPointRadius);
    }
}