using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProdutosController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Produto>>> Get()
    {
        if (_context.Produtos == null)
        {
            return NotFound();
        }

        var produtos = await _context.Produtos.ToListAsync();
        return produtos;
    }

    [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
    public async Task<ActionResult<Produto>> Get(int id)
    {
        if (_context.Produtos == null)
        {
            return NotFound("Produtos not found...");
        }

        var produto = await _context.Produtos.FirstOrDefaultAsync(p => p.ProdutoId == id);
        if (produto is null)
        {
            return NotFound("Produto não encontrado...");
        }
        return produto;
    }

    [HttpPost]
    public ActionResult Post(Produto produto)
    {
        if (produto is null)
            return BadRequest();

        if (_context.Produtos == null)
        {
            return Problem("Entity set 'AppDbContext.Produtos' is null.");
        }

        _context.Produtos.Add(produto);
        _context.SaveChanges();

        return new CreatedAtRouteResult("ObterProduto",
            new { id = produto.ProdutoId }, produto);
    }

    [HttpPut("{id:int}")]
    public ActionResult Put(int id, Produto produto)
    {
        if (id != produto.ProdutoId)
        {
            return BadRequest();
        }

        if (_context.Produtos == null)
        {
            return Problem("Entity set 'AppDbContext.Produtos' is null.");
        }

        _context.Entry(produto).State = EntityState.Modified;
        _context.SaveChanges();

        return Ok(produto);
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        if (_context.Produtos == null)
        {
            return NotFound("Produtos not found...");
        }

        var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);

        if (produto is null)
        {
            return NotFound("Produto não localizado...");
        }
        _context.Produtos.Remove(produto);
        _context.SaveChanges();

        return Ok(produto);
    }
}