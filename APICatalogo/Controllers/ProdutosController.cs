﻿using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace APICatalogo.Controllers;

[Route("[controller]")]
[ApiController]
public class ProdutosController(IUnitOfWork uof, IMapper mapper) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = uof;
    private readonly IMapper _mapper = mapper;

    [HttpGet("produtos/{id}")]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosCategoria(int id)
    {
        var produtos = await _unitOfWork.ProdutoRepository.GetProdutosPorCategoriaAsync(id);

        if (produtos is null)
            return NotFound();
        
        var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

        return Ok(produtosDto);
    }

    [HttpGet("pagination")]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get([FromQuery]
                                   ProdutosParameters produtosParameters)
    {
        var produtos = await _unitOfWork.ProdutoRepository.GetProdutosAsync(produtosParameters);

        return ObterProdutos(produtos);
    }

    [HttpGet("filter/preco/pagination")]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosFilterPreco([FromQuery] ProdutosFiltroPreco
                                                                                    produtosFilterParameters)
    {
        var produtos = await _unitOfWork.ProdutoRepository.GetProdutosFiltroPrecoAsync(produtosFilterParameters);
        return ObterProdutos(produtos);
    }
    private ActionResult<IEnumerable<ProdutoDTO>> ObterProdutos(PagedList<Produto> produtos)
    {
        var metadata = new
        {
            produtos.TotalCount,
            produtos.PageSize,
            produtos.CurrentPage,
            produtos.TotalPages,
            produtos.HasNext,
            produtos.HasPrevious
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
        var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);
        return Ok(produtosDto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get()
    {
        var produtos = await _unitOfWork.ProdutoRepository.GetAllAsync();
        if (produtos is null)
            return NotFound();

        var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);
        return Ok(produtosDto);
    }
    [HttpGet("{id}", Name = "ObterProduto")]
    public async Task<ActionResult<ProdutoDTO>> Get(int id)
    {
        var produto = await _unitOfWork.ProdutoRepository.GetAsync(c => c.ProdutoId == id);
        if (produto is null)
        {
            return NotFound("Produto não encontrado...");
        }
        var produtoDto = _mapper.Map<ProdutoDTO>(produto);
        return Ok(produtoDto);
    }

    [HttpPost]
    public async Task<ActionResult<ProdutoDTO>> Post(ProdutoDTO produtoDto)
    {
        if (produtoDto is null)
            return BadRequest();

        var produto = _mapper.Map<Produto>(produtoDto);

        var novoProduto = _unitOfWork.ProdutoRepository.Create(produto);
        await _unitOfWork.CommitAsync();

        var novoProdutoDto = _mapper.Map<ProdutoDTO>(novoProduto);

        return new CreatedAtRouteResult("ObterProduto",
            new { id = novoProdutoDto.ProdutoId }, novoProdutoDto);
    }

    [HttpPatch("{id}/UpdatePartial")]
    public async Task<ActionResult<ProdutoDTOUpdateResponse>> Patch(int id,
        JsonPatchDocument<ProdutoDTOUpdateRequest> patchProdutoDto)
    {
        if (patchProdutoDto == null || id <= 0)
            return BadRequest();

        var produto = await _unitOfWork.ProdutoRepository.GetAsync(c => c.ProdutoId == id);

        if (produto == null)
            return NotFound();

        var produtoUpdateRequest = _mapper.Map<ProdutoDTOUpdateRequest>(produto);

        patchProdutoDto.ApplyTo(produtoUpdateRequest, ModelState);

        if (!ModelState.IsValid || !TryValidateModel(produtoUpdateRequest))
            return BadRequest(ModelState);

        _mapper.Map(produtoUpdateRequest, produto);

        _unitOfWork.ProdutoRepository.Update(produto);
        await _unitOfWork.CommitAsync();

        return Ok(_mapper.Map<ProdutoDTOUpdateResponse>(produto));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProdutoDTO>> Put(int id, ProdutoDTO produtoDto)
    {
        if (id != produtoDto.ProdutoId)
            return BadRequest();//400

        var produto = _mapper.Map<Produto>(produtoDto);

        var produtoAtualizado = _unitOfWork.ProdutoRepository.Update(produto);
        await _unitOfWork.CommitAsync();

        var produtoAtualizadoDto = _mapper.Map<ProdutoDTO>(produtoAtualizado);

        return Ok(produtoAtualizadoDto);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ProdutoDTO>> Delete(int id)
    {
        var produto = await _unitOfWork.ProdutoRepository.GetAsync(p => p.ProdutoId == id);
        if (produto is null)
        {
            return NotFound("Produto não encontrado...");
        }

        var produtoDeletado = _unitOfWork.ProdutoRepository.Delete(produto);
        await _unitOfWork.CommitAsync();

        var produtoDeletadoDto = _mapper.Map<ProdutoDTO>(produtoDeletado);

        return Ok(produtoDeletadoDto);
    }
}