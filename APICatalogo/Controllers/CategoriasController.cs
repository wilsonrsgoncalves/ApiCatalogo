using APICatalogo.Context;
using APICatalogo.Filters;
using APICatalogo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace APICatalogo.Controllers;

[Route("[controller]")]
[ApiController]
public class CategoriasController(AppDbContext context, ILogger<CategoriasController> logger) : ControllerBase
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<CategoriasController> _logger = logger;

    [HttpGet("produtos")]
    public async Task<ActionResult<IEnumerable<Categoria>>> GetCategoriasProdutos()
    {
        try
        {
            _logger.LogInformation("============ Get/Categorias/produtos==============");
            
            if (_context.Categorias == null)
            {
                _logger.LogInformation("============ Nenhuma categoria encontrada. ==============");
                return NotFound("Nenhuma categoria encontrada.");
            }

            return await _context.Categorias.Include(p => p.Produtos).ToListAsync();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Ocorreu um problema ao tratar a sua solicitação.");
        }
    }

    [HttpGet]
    [ServiceFilter(typeof(ApiLoggingFilter))]
    public ActionResult<IEnumerable<Categoria>> Get()
    {
        try
        {   var categorias = _context.Categorias?.ToList();
            if (categorias == null)
            {
                return NotFound("Nenhuma categoria encontrada.");
            }
            return Ok(categorias);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Ocorreu um problema ao tratar a sua solicitação.");
        }

    }

    [HttpGet("{id:int}", Name = "ObterCategoria")]
    public ActionResult<Categoria> Get(int id)
    {
        try
        {
            var categoria = _context.Categorias.FirstOrDefault(p => p.CategoriaId == id);

            if (categoria == null)
            {
                _logger.LogWarning("=======================================");
                _logger.LogWarning($"Categoria com id= ${id} não encontrada...");
                _logger.LogWarning("=======================================");
                return NotFound($"Categoria com id= {id} não encontrada...");
            }
            return Ok(categoria);
        }
        catch (Exception)
        {
            _logger.LogError("=======================================================================");
            _logger.LogError($"{StatusCodes.Status500InternalServerError} - Ocorreu um problema ao tratar a sua solicitação");
            _logger.LogError("=======================================================================");

            return StatusCode(StatusCodes.Status500InternalServerError,
                       "Ocorreu um problema ao tratar a sua solicitação.");
        }
    }

    [HttpPost]
    public ActionResult Post(Categoria categoria)
    {
        try
        {
            if (categoria is null)
                return BadRequest("Dados inválidos");

            _context.Categorias?.Add(categoria);
            _context.SaveChanges();

            return new CreatedAtRouteResult("ObterCategoria",
                new { id = categoria.CategoriaId }, categoria);

        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                      "Ocorreu um problema ao tratar a sua solicitação.");
        }
    }

    [HttpPut("{id:int}")]
    public ActionResult Put(int id, Categoria categoria)
    {
        try
        {
            if (id != categoria.CategoriaId)
            {
                return BadRequest("Dados inválidos");
            }
            _context.Entry(categoria).State = EntityState.Modified;
            _context.SaveChanges();
            return Ok(categoria);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                   "Ocorreu um problema ao tratar a sua solicitação.");
        }
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        try
        {
            var categoria = _context.Categorias?.FirstOrDefault(p => p.CategoriaId == id);

            if (categoria == null)
            {
                return NotFound($"Categoria com id={id} não encontrada...");
            }
            _context.Categorias?.Remove(categoria);
            _context.SaveChanges();
            return Ok(categoria);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                           "Ocorreu um problema ao tratar a sua solicitação.");
        }
    }
}