namespace MineRace.Utils.Netcode
{
    public struct NetworkVariableChangedEvent<T>
    {
        public T previousValue;
        public T newValue;

        internal NetworkVariableChangedEvent(T previousValue, T newValue)
        {
            this.previousValue = previousValue;
            this.newValue = newValue;
        }
    }
}
