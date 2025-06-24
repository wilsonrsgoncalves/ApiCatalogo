namespace APICatalogo.DTOs;

using System.ComponentModel.DataAnnotations;

public class ProdutoDTO
{
    public int ProdutoId { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(80, MinimumLength = 5, ErrorMessage = "O nome deve ter entre 5 e 80 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "A descrição é obrigatória")]
    [StringLength(300)]
    public string Descricao { get; set; } = string.Empty;

    [Required(ErrorMessage = "O preço é obrigatório")]
    [Range(1, 10000, ErrorMessage = "O preço deve estar entre 1 e 10000")]
    public decimal Preco { get; set; }

    [Required(ErrorMessage = "A imagem é obrigatória")]
    [StringLength(300, MinimumLength = 10)]
    public string ImagemUrl { get; set; } = string.Empty;

    public int CategoriaId { get; set; }
}
    
