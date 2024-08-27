using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [SerializeField] private BombItem item;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private LayerMask destructableLayerMask;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        float timeBetweenTicks = item.ignitionTime / (sprites.Length - 1);
        StartCoroutine(BombTick(timeBetweenTicks));
    }

    private IEnumerator BombTick(float timeBetweenTicks)
    {
        int currentTick = 0;
        while (currentTick < sprites.Length)
        {
            spriteRenderer.sprite = sprites[currentTick];
            currentTick++;
            yield return new WaitForSeconds(timeBetweenTicks);
        }

        Ignite();
        yield return null;
    }

    private void Ignite()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, item.explosionRadius, destructableLayerMask);
        foreach (Collider2D collider in colliders)
        {
            Destroy(collider.gameObject);
        }

        Destroy(gameObject);
    }
}
