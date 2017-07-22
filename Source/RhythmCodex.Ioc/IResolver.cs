namespace RhythmCodex.Ioc
{
    public interface IResolver
    {
        T Resolve<T>() where T : class;
    }
}
