namespace MineRace.Infrastructure
{
    public interface IPublisher<T>
    {
        void Publish(T message);
    }
}
