using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class ClientPlayerItemBag : NetworkBehaviour
{
    private readonly List<HeldItem> items = new();
    private Player player;
    private HeldItem selectedItem;

    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private NetworkPlayerState networkPlayerState;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsClient || !IsOwner)
        {
            enabled = false;
            return;
        }

        inputReader.OnUseHook += OnUse;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsClient || !IsOwner)
        {
            return;
        }

        inputReader.OnUseHook -= OnUse;
    }

    private void OnUse()
    {
        if (networkPlayerState.State.Value != PlayerState.Playing || selectedItem == null)
        {
            return;
        }

        selectedItem.Use(player);
    }

    public bool TryAddItem<T>(T item) where T : HeldItem
    {
        int existingItemCount = items.OfType<T>().Count();
        if (!item.stackable && existingItemCount > 0)
        {
            return false;
        }

        if (existingItemCount >= item.maxStackSize)
        {
            return false;
        }

        items.Add(item);
        if (selectedItem == null)
        {
            selectedItem = item;
        }

        return true;
    }

    public bool TryUseItem<T>() where T : HeldItem
    {
        T item = items.OfType<T>().FirstOrDefault();
        if (item == null)
        {
            return false;
        }

        item.Use(player);
        items.Remove(item);
        if (selectedItem == item)
        {
            selectedItem = null;
        }

        return true;
    }
}
