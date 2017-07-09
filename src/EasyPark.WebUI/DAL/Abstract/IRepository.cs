using EasyPark.WebUI.DAL.Entities;
using System.Linq;

namespace EasyPark.WebUI.DAL.Abstract
{
    public interface IRepository<T>
            where T : class, IEntity
    {
        void CommitChanges();

        void DeleteOnCommit(T entity);

        T Get(long key);

        IQueryable<T> GetAll();

        long InsertOnCommit(T entity);

        void UpdateOnCommit(T entity);
    }
}