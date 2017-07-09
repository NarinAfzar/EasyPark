using EasyPark.WebUI.DAL.Entities;
using System.Data.Entity;

namespace EasyPark.WebUI.DAL.Abstract
{
    public interface IDbContext
    {
        int SaveChanges();

        DbSet<T> Set<T>() where T : class;

        void ChangeState<T>(T entity, EntityState state) where T : class, IEntity;
    }
}