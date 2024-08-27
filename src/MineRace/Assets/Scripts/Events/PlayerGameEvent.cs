using MineRace.Infrastructure;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerEvent", menuName = "Events/Player")]
public sealed class PlayerGameEvent : GameEvent<Player>
{
}
