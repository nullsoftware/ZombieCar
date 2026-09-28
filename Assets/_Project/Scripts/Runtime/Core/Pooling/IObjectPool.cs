namespace ZombieCar.Core.Pooling
{
    public interface IObjectPool<T> where T : class
    {
        int CountActive { get; }
        int CountInactive { get; }

        T Get();

        void Release(T item);

        /// <summary>
        /// Returns every item handed out by this pool. Used when a level restarts.
        /// </summary>
        void ReleaseAll();
    }
}
