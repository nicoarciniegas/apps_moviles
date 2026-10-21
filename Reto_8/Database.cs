using SQLite;

namespace Reto8
{
    public static class Database
    {
        static SQLiteConnection? _db;

        static SQLiteConnection Db
        {
            get
            {
                if (_db == null)
                {
                    _db = new SQLiteConnection(Path.Combine(FileSystem.AppDataDirectory, "empresas.db3"));
                    _db.CreateTable<Empresa>();
                }
                return _db;
            }
        }

        public static List<Empresa> Listar(string nombre, string clasificacion)
        {
            var q = Db.Table<Empresa>().ToList().AsEnumerable();
            if (!string.IsNullOrWhiteSpace(nombre))
                q = q.Where(e => e.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(clasificacion) && clasificacion != "Todas")
                q = q.Where(e => e.Clasificacion.Split(',').Contains(clasificacion));
            return q.OrderBy(e => e.Nombre).ToList();
        }

        public static void Guardar(Empresa e)
        {
            if (e.Id == 0) Db.Insert(e); else Db.Update(e);
        }

        public static void Eliminar(Empresa e) => Db.Delete(e);
    }
}
