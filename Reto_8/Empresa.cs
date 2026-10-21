using SQLite;

namespace Reto8
{
    public class Empresa
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Url { get; set; } = "";
        public string Telefono { get; set; } = "";
        public string Email { get; set; } = "";
        public string Productos { get; set; } = "";
        public string Clasificacion { get; set; } = "Consultoría";
    }
}
