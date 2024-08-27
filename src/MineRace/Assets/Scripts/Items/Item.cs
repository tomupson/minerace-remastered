using UnityEngine;

public abstract class Item : ScriptableObject
{
    public GameObject prefab;

    public abstract void Use(Player player);
}
