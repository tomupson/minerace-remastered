using Unity.Multiplayer.Tools.NetStatsMonitor;
using UnityEngine;

[RequireComponent(typeof(RuntimeNetStatsMonitor))]
public class NetStatsUI : MonoBehaviour
{
    private RuntimeNetStatsMonitor monitor;

    [SerializeField] private PlayerInputReader inputReader;

    private void Awake()
    {
        monitor = GetComponent<RuntimeNetStatsMonitor>();
        monitor.Visible = false;

        inputReader.OnToggleNetStatsHook += OnToggleNetStatsHook;
    }

    private void OnToggleNetStatsHook()
    {
        monitor.Visible = !monitor.Visible;
    }
}
