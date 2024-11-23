namespace Core;

public class Usuario(int id, string nome, string email)
{
    public int Id { get; set; } = id;
    public string Nome { get; set; } = nome;
    public string Email { get; set; } = email;
}