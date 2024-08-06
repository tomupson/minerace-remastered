using VContainer.Unity;

public class GameStateBehaviour : LifetimeScope
{
    protected override void Awake()
    {
        base.Awake();

        if (Parent != null)
        {
            Parent.Container.Inject(this);
        }
    }
}
