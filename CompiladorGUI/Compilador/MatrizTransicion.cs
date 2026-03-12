using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador
{
    public class MatrizTransicion
    {
        // Columnas:
        // 0 = Letra o '_'
        // 1 = Dígito
        // 2 = Espacio en blanco
        // 3 = Otro símbolo (por ejemplo: operadores, delimitadores, etc.)

        // Estados o RENGLONES:
        // 0 = inicial
        // 1 = leyendo ID / palabra reservada
        // 2 = leyendo número entero

        // Valores ACEPTADOS:
        // 100 = aceptar ID/palabra reservada   
        // 200 = aceptar número entero
        // >500 = error léxico

        private readonly int[,] _matriz =
        {
    //      0:L  1:D  2:.  3:ESP 4:SIMB 5:ERROR
    /*0*/ { 1,   2,  501,  0,   300,   501 }, 
    /*1*/ { 1,   1,  100, 100,  100,   100 },
    /*2*/ { 501, 2,   3,  200,  200,   200 },
    /*3*/ { 501, 4,  501, 501,  501,   501 },
    /*4*/ { 501, 4,  501, 201,  201,   201 }
};

        public int ObtenerColumna(char c)
        {
            if (char.IsLetter(c) || c == '_') return 0;
            if (char.IsDigit(c)) return 1;
            if (c == '.') return 2;
            if (char.IsWhiteSpace(c)) return 3;

            // Símbolos válidos del lenguaje
            if (";=/+-*><:(){},".Contains(c)) return 4;

            // CUALQUIER OTRA COSA es un error real
            return 5;
        }

        public int SiguienteEstado(int estado, int columna)
        {
            return _matriz[estado, columna];
        }
    }
}

