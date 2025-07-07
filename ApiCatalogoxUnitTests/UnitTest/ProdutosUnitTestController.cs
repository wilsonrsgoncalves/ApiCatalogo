using APICatalogo.Context;
using APICatalogo.Mappings;
using APICatalogo.Repositories;
using APICatalogo.Repositories.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogoxUnitTests.UnitTest;

public class ProdutosUnitTestController
{
    public IUnitOfWork repository;
    public IMapper mapper;
    public static DbContextOptions<AppDbContext> DbContextOptions { get; }
    public static readonly string connectionString = ConfigurationHelper.GetConnectionString("DefaultConnection");

    static ProdutosUnitTestController()
    {
        DbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
           .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
           .Options;
    }
   public ProdutosUnitTestController()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new DTOMappingProfile());
        });

        mapper = config.CreateMapper();

        var context = new AppDbContext(DbContextOptions);
        repository = new UnitOfWork(context);
    }
}
