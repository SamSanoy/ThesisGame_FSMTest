using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] float attackCooldown = 0.3f;

    [Header("Hitbox")]
    [SerializeField] GameObject attackPrefab;
    [SerializeField] float attackOffset = 0.8f;
    [SerializeField] float attackDuration = 0.2f;
    [SerializeField] float attackDamage = 10f;

    [Header("References")]
    [SerializeField] Camera mainCamera;

    CharacterController2D characterController;
    float nextAttackTime;

    void Awake()
    {
        characterController =
            GetComponent<CharacterController2D>();

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Update()
    {
        if (characterController == null ||
            characterController.IsDashing)
            return;

        if (Mouse.current == null ||
            mainCamera == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame &&
            Time.time >= nextAttackTime)
        {
            Attack();

            nextAttackTime =
                Time.time + attackCooldown;
        }
    }

    void Attack()
    {
        Vector2 mouseScreenPosition =
            Mouse.current.position.ReadValue();

        float distanceFromCamera =
            Mathf.Abs(
                transform.position.z -
                mainCamera.transform.position.z
            );

        Vector3 mouseWorldPosition =
            mainCamera.ScreenToWorldPoint(
                new Vector3(
                    mouseScreenPosition.x,
                    mouseScreenPosition.y,
                    distanceFromCamera
                )
            );

        Vector2 direction =
            (Vector2)(mouseWorldPosition -
            transform.position);

        if (direction.sqrMagnitude < 0.0001f)
            return;

        direction.Normalize();

        Vector2 spawnPosition =
            (Vector2)transform.position +
            direction * attackOffset;

        float angle = Mathf.Atan2(
            direction.y,
            direction.x
        ) * Mathf.Rad2Deg;

        Quaternion rotation =
            Quaternion.Euler(0f, 0f, angle);

        GameObject hitbox = Instantiate(
            attackPrefab,
            spawnPosition,
            rotation
        );

        PlayerAttackHitbox hitboxScript =
            hitbox.GetComponent<PlayerAttackHitbox>();//EnemyAttackHitbox

        if (hitboxScript != null)
            hitboxScript.damage = attackDamage;

        Destroy(hitbox, attackDuration);
    }
}
