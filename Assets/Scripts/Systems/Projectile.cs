using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] bool initOfStart = false;
    [Header("Projectile Settings")]
    public float speed = 10f;
    public float damage = 5f;
    public float lifeTime = 3f;

    [Header("Collision Settings")]
    public bool destroyOnAnyCollision = true; // destroy even if not hitting a damageable

    private bool initialized = false;
    private float timer = 0f;

    void Start()
    {
        if (initOfStart)
        {
            initialized = true;
            Destroy(this.gameObject,lifeTime);
        }
    }

    // Call this after spawning
    [NaughtyAttributes.Button]
    public void Init(float speed = -1f, float damage = -1f, float lifeTime = -1f, Vector2? direction = null)
    {
        if (speed > 0) this.speed = speed;
        if (damage >= 0) this.damage = damage;
        if (lifeTime > 0) this.lifeTime = lifeTime;
        if (direction.HasValue)
            transform.right = direction.Value.normalized;  // forward direction for 2D

        initialized = true;
        Destroy(this.gameObject,lifeTime);
    }

    void FixedUpdate()
    {
        if (!initialized) return;

        // Move in facing direction
        transform.Translate(Vector2.up * speed * Time.deltaTime);

        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        HandleHit(other.gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        HandleHit(collision.gameObject);
    }

    void HandleHit(GameObject hitObj)
    {
        if (hitObj == gameObject) return;

        IDamageable damageable = hitObj.GetComponent<IDamageable>();
        if (damageable == null)
            damageable = hitObj.GetComponentInParent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            Destroy(Instantiate(_GameAssets.Instance.explosionEffect, transform.position, Quaternion.identity),0.35f);
            Destroy(gameObject);
        }
        else if (destroyOnAnyCollision)
        {
            Destroy(gameObject);
        }
    }
}
