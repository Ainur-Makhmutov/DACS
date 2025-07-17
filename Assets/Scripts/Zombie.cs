using UnityEngine;

public class Zombie : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackTimer = 0f;

    private Transform player;
    private Hero hero;

    void Start()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            hero = playerObject.GetComponent<Hero>();
        }
    }

    void Update()
    {
        Attack();
    }

    private void Attack()
    {
        if (player == null || hero == null) return;

        attackTimer -= Time.deltaTime;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attackRange && attackTimer <= 0f)
        {
            attackTimer = attackCooldown;
            hero.GetDamage();
        }
    }

}
