using System;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

public sealed class TrackedLobby
{
    private const string RelayJoinCodeKey = "RelayJoinCode";
    internal readonly Lobby lobby;

    public string LobbyId => lobby.Id;

    public string RelayJoinCode
    {
        get => lobby.Data.TryGetValue(RelayJoinCodeKey, out DataObject joinCodeData) ? joinCodeData.Value : null;
        set => lobby.Data[RelayJoinCode] = new DataObject(DataObject.VisibilityOptions.Member, value);
    }

    internal Dictionary<string, DataObject> Data => lobby.Data;

    internal TrackedLobby(Lobby lobby)
    {
        this.lobby = lobby ?? throw new ArgumentNullException(nameof(lobby));
    }

    public bool IsLocalPlayerHost() =>
        lobby.HostId == AuthenticationService.Instance.PlayerId;

    internal void ApplyChanges(ILobbyChanges changes) =>
        changes.ApplyToLobby(lobby);
}
