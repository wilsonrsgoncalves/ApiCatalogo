using APICatalogo.DTOs;
using APICatalogo.Mappings;
using APICatalogo.Models;
using APICatalogo.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers;
[Route("[controller]")]
[ApiController]
public class CategoriasController(IUnitOfWork unitOfWork,
    ILogger<CategoriasController> logger) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<CategoriasController> _logger = logger;

    [HttpGet]
    public ActionResult<IEnumerable<CategoriaDTO >> Get()
    {
        var categorias = _unitOfWork.CategoriaRepository.GetAll();

        if (categorias is null)
            return NotFound("Não existem categorias...");

        var categoriasDto = categorias.ToCategoriaDTOList();

        return Ok(categoriasDto);
    }

    [HttpGet("{id:int}", Name = "ObterCategoria")]
    public ActionResult<Categoria> Get(int id)
    {
        var categoria = _unitOfWork.CategoriaRepository.Get(c => c.CategoriaId == id);

        if (categoria is null)
        {
            _logger.LogWarning("Categoria com id= {id} não encontrada...",id);
            return NotFound($"Categoria com id= {id} não encontrada...");
        }

        var categoriaDto = categoria.ToCategoriaDTO();

        return Ok(categoriaDto);
    }

    [HttpPost]
    public ActionResult Post(CategoriaDTO categoriaDto)
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
            return BadRequest("Falha ao converter dados para categoria");
        }

        var categoriaCriada = _unitOfWork.CategoriaRepository.Create(categoria);
        _unitOfWork.Commit();

        var novaCategoriaDto = categoriaCriada.ToCategoriaDTO();
        if (novaCategoriaDto is null)
        {
            _logger.LogWarning($"Falha ao converter Categoria para CategoriaDTO...");
            return BadRequest("Falha ao converter dados para categoria");
        }
        else
        {
            return new CreatedAtRouteResult("ObterCategoria",
                new { id = novaCategoriaDto.CategoriaId },
                novaCategoriaDto);
        }
    }

    [HttpPut("{id:int}")]
    public ActionResult Put(int id, Categoria categoria)
    {
        if (id != categoria.CategoriaId)
        {
            _logger.LogWarning($"Dados inválidos...");
            return BadRequest("Dados inválidos");
        }

        _unitOfWork.CategoriaRepository.Update(categoria);
        _unitOfWork.Commit();

        return Ok(categoria);
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var categoria = _unitOfWork.CategoriaRepository.Get(c => c.CategoriaId == id);

        if (categoria is null)
        {
            _logger.LogWarning("Categoria com id={id} não encontrada...",id);
            return NotFound($"Categoria com id={id} não encontrada...");
        }

        var categoriaExcluida = _unitOfWork.CategoriaRepository.Delete(categoria);
        _unitOfWork.Commit();

        return Ok(categoriaExcluida);

    }
}