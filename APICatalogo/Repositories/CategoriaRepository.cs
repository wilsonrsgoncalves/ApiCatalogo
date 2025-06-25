using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories.Interfaces;

namespace APICatalogo.Repositories;

public class CategoriaRepository(AppDbContext context) : Repository<Categoria>(context), ICategoriaRepository
{
    public async Task<PagedList<Categoria>> GetCategoriasAsync(CategoriasParameters categoriasParams)
    {
        var categorias = await GetAllAsync();
        var categoriasOrdenadas = categorias.OrderBy(p => p.CategoriaId).AsQueryable();
        var resultCategorias = PagedList<Categoria>.ToPagedList(categoriasOrdenadas,
            categoriasParams.PageNumber, categoriasParams.PageSize);

        return resultCategorias;
    }

    public async Task<PagedList<Categoria>> GetCategoriasFiltroNomeAsync(CategoriasFiltroNome categoriasParams)
    {
        var categorias = await GetAllAsync();

        if (!string.IsNullOrEmpty(categoriasParams.Nome))
        {
            categorias = categorias.Where(c => c.Nome != null && c.Nome.Contains(categoriasParams.Nome));
        }

        var categoriasFiltradas = PagedList<Categoria>.ToPagedList(categorias.AsQueryable(),
                                   categoriasParams.PageNumber, categoriasParams.PageSize);

        return categoriasFiltradas;
    }
}
