using KristaEmp.Data;

namespace KristaEmp
{
    internal class Core
    {
        private static KristaEmpEntities _context;

        public static KristaEmpEntities GetContext()
        {
            if (_context == null)
                _context = new KristaEmpEntities();
            return _context;
        }
    }
}
