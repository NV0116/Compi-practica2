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
        // L(0) D(1) .(2) =(3) <(4) >(5) /(6) ESP(7) SYMB(8)
        /*0*/ { 1,   2,  501,  5,   6,   7,   8,   0,   300 }, 
        /*1*/ { 1,   1,  100, 100, 100, 100, 100, 100, 100 }, // Acepta ID
        /*2*/ { 200, 2,   3,  200, 200, 200, 200, 200, 200 }, // Acepta Entero
        /*3*/ { 501, 4,  501, 501, 501, 501, 501, 501, 501 }, // Punto decimal
        /*4*/ { 201, 4,  201, 201, 201, 201, 201, 201, 201 }, // Acepta Real
        /*5*/ { 304, 304, 304, 305, 304, 304, 304, 304, 304 }, // = o ==
        /*6*/ { 308, 308, 308, 309, 308, 308, 308, 308, 308 }, // < o <=
        /*7*/ { 306, 306, 306, 307, 306, 306, 306, 306, 306 }, // > o >=
        /*8*/ { 502, 502, 502, 502, 502, 502,  9,  502, 502 }, // Comentario //
        /*9*/ { 9,   9,   9,   9,   9,   9,   9,   9,   9   }  // Cuerpo comentario
    };

        public int ObtenerColumna(char c)
        {
            if (char.IsLetter(c) || c == '_') return 0;
            if (char.IsDigit(c)) return 1;
            if (c == '.') return 2;
            if (c == '=') return 3;
            if (c == '<') return 4;
            if (c == '>') return 5;
            if (c == '/') return 6;
            if (char.IsWhiteSpace(c)) return 7;
            return 8; // Cualquier otro símbolo (;, +, -, *, (, ), {, }, ,)
        }

        public int SiguienteEstado(int estado, int columna)
        {
            return _matriz[estado, columna];
        }
    }
}

