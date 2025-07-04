using APICatalogo.Controllers;
using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Repositories.Interfaces;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Linq.Expressions;


namespace ApiCatalogoxUnitTests.UnitTest
{
    public class GetProdutoUnitTests
    {
        private readonly Mock<IUnitOfWork> _uofMock;
        private readonly Mock<IProdutoRepository> _produtoRepoMock;
        private readonly IMapper _mapper;
        private readonly ProdutosController _controller;

        public GetProdutoUnitTests()
        {            
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Produto, ProdutoDTO>().ReverseMap();
            });

            _mapper = config.CreateMapper();

            _produtoRepoMock = new Mock<IProdutoRepository>();
            _uofMock = new Mock<IUnitOfWork>();

            _uofMock.Setup(u => u.ProdutoRepository).Returns(_produtoRepoMock.Object);

            _controller = new ProdutosController(_uofMock.Object, _mapper);
        }

        [Fact]
        public async Task GetProdutoById_DeveRetornarOkResult()
        {
            // Arrange
            int produtoId = 1;
            var produto = new Produto
            {
                ProdutoId = produtoId,
                Nome = "Produto Teste",
                Preco = 12.5m,
                CategoriaId = 1
            };

            _produtoRepoMock
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Produto, bool>>>()))
                .ReturnsAsync(produto);

            // Act
            var result = await _controller.Get(produtoId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>()
                  .Which.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetProdutoById_DeveRetornarNotFound()
        {
            // Arrange
            int produtoId = 999;

            _produtoRepoMock
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Produto, bool>>>()))
                .ReturnsAsync((Produto?)null);

            // Act
            var result = await _controller.Get(produtoId);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>()
                  .Which.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task GetProdutos_DeveRetornarListaDeProdutos()
        {
            // Arrange
            var produtos = new List<Produto>
        {
            new() { ProdutoId = 1, Nome = "Produto 1", Preco = 10, CategoriaId = 1 },
            new() { ProdutoId = 2, Nome = "Produto 2", Preco = 20, CategoriaId = 2 },
            new() { ProdutoId = 3, Nome = "Produto 2", Preco = 20, CategoriaId = 3 },
            new() { ProdutoId = 4, Nome = "Produto 2", Preco = 20, CategoriaId = 4 }
        };

            _produtoRepoMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(produtos);

            // Act
            var result = await _controller.Get();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeAssignableTo<IEnumerable<ProdutoDTO>>()
                .And.NotBeNull();
        }

        [Fact]
        public async Task GetProdutosDeveRetornarNotFoundQuandoListaVazia()
        {
            // Arrange
            _produtoRepoMock
                .Setup(static res => res.GetAllAsync())
                 .ReturnsAsync([]);

            // Act
            var result = await _controller.Get();

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }


        // ---------- [Fact] (um caso fixo)
        [Fact]
        public async Task GetProdutosPorCategoria_DeveRetornarNotFound_QuandoListaVazia()
        {
            // Arrange
            int categoriaId = 1233;
            _produtoRepoMock.Setup(r => r.GetProdutosPorCategoriaAsync(categoriaId))
                .ReturnsAsync(([]));

            // Act
            var result = await _controller.GetPorCategoria(categoriaId);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        // ---------- [Theory] com diferentes categorias
        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        public async Task GetProdutosPorCategoria_DeveRetornarOk(int categoriaId)
        {
            // Arrange
            var produtos = new List<Produto>
            {
                new() { ProdutoId = 1, Nome = "Produto A", Preco = 10, CategoriaId = categoriaId },
                new() { ProdutoId = 2, Nome = "Produto B", Preco = 20, CategoriaId = categoriaId }
            };

            _produtoRepoMock
                .Setup(r => r.GetProdutosPorCategoriaAsync(categoriaId))
                .ReturnsAsync(produtos);

            // Act
            var result = await _controller.GetPorCategoria(categoriaId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeAssignableTo<IEnumerable<ProdutoDTO>>()
                .And.NotBeNull();
        }
    }
}