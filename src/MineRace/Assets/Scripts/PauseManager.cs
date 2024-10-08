using MineRace.Infrastructure;
using UnityEngine;
using VContainer;

public class PauseManager : MonoBehaviour
{
    [Inject] private readonly IPublisher<PauseStateChangedMessage> pauseStatePublisher;

    private bool isPaused;

    [SerializeField] private PlayerInputReader inputReader;

    private void Awake()
    {
        inputReader.OnPauseHook += OnPause;
        inputReader.OnUnpauseHook += OnUnpause;
    }

    private void OnDestroy()
    {
        inputReader.OnPauseHook -= OnPause;
        inputReader.OnUnpauseHook -= OnUnpause;
    }

    public void Unpause()
    {
        SetPauseState(paused: false);
    }

    private void OnPause()
    {
        SetPauseState(paused: true);
    }

    private void OnUnpause()
    {
        SetPauseState(paused: false);
    }

    private void SetPauseState(bool paused)
    {
        if (isPaused == paused)
        {
            return;
        }

        isPaused = paused;
        pauseStatePublisher.Publish(new PauseStateChangedMessage(paused));
    }
}
