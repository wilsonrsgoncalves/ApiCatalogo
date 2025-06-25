using APICatalogo.DTOs;
using APICatalogo.Mappings;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace APICatalogo.Controllers;

[Route("[controller]")]
[ApiController]
public class CategoriasController(IUnitOfWork uof,
    ILogger<CategoriasController> logger) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = uof;
    private readonly ILogger<CategoriasController> _logger = logger;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get()
    {
        var categorias = await _unitOfWork.CategoriaRepository.GetAllAsync();

        if (categorias is null)
            return NotFound("Não existem categorias...");

        var categoriasDto = categorias.ToCategoriaDTOList();

        return Ok(categoriasDto);
    }

    [HttpGet("pagination")]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get([FromQuery]
                               CategoriasParameters categoriasParameters)
    {
        var categorias = await _unitOfWork.CategoriaRepository.GetCategoriasAsync(categoriasParameters);

        return ObterCategorias(categorias);
    }

    [HttpGet("filter/nome/pagination")]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasFiltradas(
                                   [FromQuery] CategoriasFiltroNome categoriasFiltro)
    {
        var categoriasFiltradas = await _unitOfWork.CategoriaRepository
                                     .GetCategoriasFiltroNomeAsync(categoriasFiltro);

        return ObterCategorias(categoriasFiltradas);

    }

    private ActionResult<IEnumerable<CategoriaDTO>> ObterCategorias(PagedList<Categoria> categorias)
    {
        var metadata = new
        {
            categorias.TotalCount,
            categorias.PageSize,
            categorias.CurrentPage,
            categorias.TotalPages,
            categorias.HasNext,
            categorias.HasPrevious
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
        var categoriasDto = categorias.ToCategoriaDTOList();
        return Ok(categoriasDto);
    }

    [HttpGet("{id:int}", Name = "ObterCategoria")]
    public async Task<ActionResult<CategoriaDTO>> Get(int id)
    {
        var categoria = await _unitOfWork.CategoriaRepository.GetAsync(c => c.CategoriaId == id);

        if (categoria is null)
        {
            _logger.LogWarning("Categoria com id= {id} não encontrada...",id);
            return NotFound($"Categoria com id= {id} não encontrada...");
        }

        var categoriaDto = categoria.ToCategoriaDTO();

        return Ok(categoriaDto);
    }

    [HttpPost]
    public async Task<ActionResult<CategoriaDTO>> Post(CategoriaDTO categoriaDto)
    {
        if (categoriaDto is null)
        {
            _logger.LogWarning($"Dados inválidos...");
            return BadRequest("Dados inválidos");
        }

        var categoria = categoriaDto.ToCategoria();

        if (categoria is null) 
        {
            _logger.LogWarning($"Falha ao converter CategoriaDTO para Categoria...");
            return BadRequest("Falha ao converter dados");
        }

        var categoriaCriada = _unitOfWork.CategoriaRepository.Create(categoria);

        if (categoriaCriada is null) 
        {
            _logger.LogWarning($"Falha ao criar nova categoria...");
            return StatusCode(500, "Erro interno ao criar categoria");
        }

        await _unitOfWork.CommitAsync();

        var novaCategoriaDto = categoriaCriada.ToCategoriaDTO();

        if (novaCategoriaDto is null) 
        {
            _logger.LogWarning($"Falha ao converter Categoria para CategoriaDTO...");
            return StatusCode(500, "Erro interno ao processar categoria criada");
        }

        return new CreatedAtRouteResult("ObterCategoria",
            new { id = novaCategoriaDto.CategoriaId },
            novaCategoriaDto);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoriaDTO>> Put(int id, CategoriaDTO categoriaDto)
    {
        if (id != categoriaDto.CategoriaId)
        {
            _logger.LogWarning($"Dados inválidos...");
            return BadRequest("Dados inválidos");
        }

        var categoria = categoriaDto.ToCategoria();
        if (categoria is null)
        {
            _logger.LogWarning($"Falha ao converter CategoriaDTO para Categoria..."); 
            return BadRequest("Falha ao converter dados");
        }
        var categoriaAtualizada = _unitOfWork.CategoriaRepository.Update(categoria);
        await _unitOfWork.CommitAsync();

        var categoriaAtualizadaDto = categoriaAtualizada.ToCategoriaDTO();

        return Ok(categoriaAtualizadaDto);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<CategoriaDTO>> Delete(int id)
    {
        var categoria = await _unitOfWork.CategoriaRepository.GetAsync(c => c.CategoriaId == id);

        if (categoria is null)
        {
            _logger.LogWarning("Categoria com id={id} não encontrada...",id);
            return NotFound($"Categoria com id={id} não encontrada...");
        }

        var categoriaExcluida = _unitOfWork.CategoriaRepository.Delete(categoria);
        await _unitOfWork.CommitAsync();

        var categoriaExcluidaDto = categoriaExcluida.ToCategoriaDTO();

        return Ok(categoriaExcluidaDto);
    }
}