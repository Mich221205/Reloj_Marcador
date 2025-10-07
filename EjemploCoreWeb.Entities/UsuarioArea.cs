namespace EjemploCoreWeb.Entities;

public record class UsuarioArea
{
    public int ID_Usuario { get; set; }
    public int ID_Area { get; set; }
    public string Nombre_Area { get; set; } = "";
}

