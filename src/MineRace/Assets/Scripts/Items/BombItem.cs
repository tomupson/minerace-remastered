using UnityEngine;

[CreateAssetMenu(fileName = "Bomb", menuName = "MineRace/Items/Bomb")]
public class BombItem : HeldItem
{
    public float ignitionTime;
    public float explosionRadius;
    public float throwPower;

    public override void Use(Player player)
    {
        GameObject bombObject = Instantiate(prefab, player.transform.position, Quaternion.identity);
        Rigidbody2D bombRigidbody = bombObject.GetComponent<Rigidbody2D>();
        bombRigidbody.AddForce(player.transform.right * throwPower, ForceMode2D.Impulse);
    }
}
