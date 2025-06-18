using APICatalogo.Context;
using APICatalogo.Repositories.Interfaces;

namespace APICatalogo.Repositories
{
    public class UnitOfWork(AppDbContext context) : IUnitOfWork
    {
        private IProdutoRepository? _produtoRepo;
        private ICategoriaRepository? _categoriaRepo;

        public AppDbContext _context = context;

        public IProdutoRepository ProdutoRepository
        {
            get
            {
                _produtoRepo ??= new ProdutoRepository(_context);
                return _produtoRepo;
            }
        }
        public ICategoriaRepository CategoriaRepository
        {
            get
            {
                _categoriaRepo ??= new CategoriaRepository(_context);
                return _categoriaRepo;
            }
        }
        public void Commit()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }

}
