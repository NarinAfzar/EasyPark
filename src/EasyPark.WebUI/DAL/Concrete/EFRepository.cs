using EasyPark.WebUI.DAL.Abstract;
using EasyPark.WebUI.DAL.Entities;
using System.Data.Entity;
using System.Linq;

namespace EasyPark.WebUI.DAL.Concrete
{
    public class EFRepository<T>
        : IRepository<T> where T : class, IEntity
    {
        private readonly IDbContext _context;

        public EFRepository(IDbContext context)
        {
            _context = context;
        }

        public void CommitChanges()
        {
            _context.SaveChanges();
        }

        public void DeleteOnCommit(T entity)
        {
            _context.Set<T>()
                .Remove(entity);
        }

        public T Get(long key)
        {
            return _context.Set<T>().Find(key);
        }

        public IQueryable<T> GetAll()
        {
            return _context.Set<T>();
        }

        public long InsertOnCommit(T entity)
        {
            _context.Set<T>()
                .Add(entity);

            return entity.ID;
        }

        public void UpdateOnCommit(T entity)
        {
            _context.ChangeState(entity, EntityState.Modified);
        }
    }
}