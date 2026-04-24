using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador
{
    public class PalabrasReservadas
    {
        private readonly Dictionary<string, int> _mapa = new Dictionary<string, int>
        {
    {"PROGRAM", 102 },
    {"VAR", 103 },
    {"PROCEDURE", 104 },
    {"BEGIN", 105 },
    {"END", 106 },
    {"INT", 107 },
    {"FLOAT", 108 },
    {"IF", 109 },
    {"THEN", 110 },
    {"ELSE", 111 },
    {"WHILE", 112 },
    {"DO", 113 },
    {"PRINT", 114 },
        };
        // 101 sera el token para ID, palabras, identificadores, variables
        public int ObtenerToken(string lexema)
        {
            if (string.IsNullOrEmpty(lexema))
                return 101;
            var clave = lexema.ToUpperInvariant();
            return _mapa.TryGetValue(clave, out int token)
                ? token
                : 101; //identificador
        }
    }
}
