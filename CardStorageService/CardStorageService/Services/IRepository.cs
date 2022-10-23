namespace CardStorageService.Services
{
    public interface IRepository<T, TId>
    {
        IList<T> GetAll();

        T GetById(TId id);

        TId Create(T data);

        void Update(T data);

        void Delete(TId id);

    }
}
