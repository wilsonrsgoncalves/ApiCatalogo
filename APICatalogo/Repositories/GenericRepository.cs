using APICatalogo.Context;
using APICatalogo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace APICatalogo.Repositories;

public class GenericRepository<T>(AppDbContext context) : IRepository<T> where T : class
{
    protected AppDbContext _context = context;

    public IEnumerable<T> GetAll()
    {
        return _context.Set<T>().AsNoTracking();
    }
    public T? Get(Expression<Func<T, bool>> predicate)
    {
        return _context.Set<T>().AsNoTracking().FirstOrDefault(predicate);
    }
    public T Create(T entity)
    {
        _context.Set<T>().Add(entity);
        _context.SaveChanges();
        return entity;
    }
    public T Update(T entity)
    {
        _context.Set<T>().Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        _context.SaveChanges();
        return entity;
    }
    public T Delete(T entity)
    {
        _context.Set<T>().Attach(entity);
        _context.Set<T>().Remove(entity);
        _context.SaveChanges();
        return entity;
    }
}
