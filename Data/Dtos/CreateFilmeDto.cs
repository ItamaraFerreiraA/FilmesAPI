using System.ComponentModel.DataAnnotations;

namespace FilmesAPI.Data.Dtos;

public class CreateFilmeDto
{ 
    [Required(ErrorMessage = "O titulo é obrigatorio")]
    public string Titulo { get; set; }

    [Required(ErrorMessage = "O Genero é obrigatorio")]
    [StringLength(50, ErrorMessage = "O tamanho tem que ter no máximo 50 caracteres")]
    public string Genero { get; set; }

    [Required]
    [Range(70, 600, ErrorMessage = "A duração deve ter entre 70 e 600 minutos")]
    public int Duracao { get; set; }

}
