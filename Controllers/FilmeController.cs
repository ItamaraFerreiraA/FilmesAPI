using AutoMapper;
using FilmesAPI.Data;
using FilmesAPI.Data.Dtos;
using FilmesAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FilmesAPI.Controllers;

[ApiController]
[Route("[controller]")]
 
public class FilmeController : ControllerBase
{
    private FilmeContext _context;
    private IMapper _mapper;

    public FilmeController(FilmeContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper; 
    }

    [HttpPost] ///adiciona um filme ao banco de dados
    public IActionResult AdicionaFilme([FromBody] CreateFilmeDto filmeDto)
    {
        Filme filme = _mapper.Map<Filme>(filmeDto);
        _context.Filmes.Add(filme);
        _context.SaveChanges();   
        return CreatedAtAction(nameof(RecuperaFilmePorId), 
            new { id = filme.Id }, filme);

    } 
     
    [HttpGet] ///captura um filme do banco de dados
    public IEnumerable<ReadFilmeDto> RecuperaFilmes(
        [FromQuery] int skip = 0, 
        [FromQuery] int take = 10 )  
    {
        return _mapper.Map<List<ReadFilmeDto>>(_context.Filmes.Skip(skip).Take(take));  
    }

    [HttpGet("{id}")] ///captura um filme específico do banco de dados, buscando pelo id
    public IActionResult RecuperaFilmePorId(int id)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
        if (filme == null) return NotFound();  
        var filmeDto = _mapper.Map<ReadFilmeDto>(filme);
        return Ok(filmeDto);
    }


    /*[HttpGet("{titulo}")]
    public IActionResult RecuperaFilmePorTitulo(string titulo)
    {
        var filme = _context.Filmes.SingleOrDefault(filme => filme.Titulo == titulo);
        if (filme == null) return NotFound();
        return Ok(filme); 
    }*/

    [HttpPut ("{id}")]  ///atualiza um filme que já esteja salvo no banco de dados
    public IActionResult AtualizaFilme( 
        int id, 
        [FromBody] UpdateFilmeDto filmeDto )
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
        if (filme == null) return NotFound();
        _mapper.Map(filmeDto, filme); 
        _context.SaveChanges();
        return NoContent();
    }


    ///atualiza um filme que esteja salvo no banco, sem precisar passar todos os dados cadastrados neste filme
    ///passando apenas: 
    /// [
    ///     {
    ///         "op": "replace",
    ///         "path": "/titulo", (campo que quer mudar)
    ///         "value": "Rapunzel" (a informação que deseja colocar)
    ///     }
    /// ] 
    [HttpPatch("{id}")] 
    public IActionResult AtualizaFilmeParcial(
        int id,
        JsonPatchDocument<UpdateFilmeDto> patch) 
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
        if (filme == null) return NotFound();

        var filmeParaAtualizar = _mapper.Map<UpdateFilmeDto>(filme);

        patch.ApplyTo(filmeParaAtualizar, ModelState);

        if(!TryValidateModel(filmeParaAtualizar))
        {
            return ValidationProblem(ModelState);
        }

        _mapper.Map(filmeParaAtualizar, filme); 
        _context.SaveChanges();
        return NoContent(); 
    }

    [HttpDelete("{id}")] ///deleta um filme do banco de dados
    public IActionResult DeletaFilme(int id)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
        if (filme == null) return NotFound();
        _context.Remove(filme);
        _context.SaveChanges(); 
        return NoContent();
    }

}
