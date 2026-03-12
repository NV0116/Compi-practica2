using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador
{
    public class AnalizadorLexico
    {
        private readonly MatrizTransicion _matriz;
        private readonly PalabrasReservadas _palabrasReservadas;

        public AnalizadorLexico()
        {
            _matriz = new MatrizTransicion();
            _palabrasReservadas = new PalabrasReservadas();
        }

        public ResultadoLexico Analizar(CodigoFuente fuente)
        {
            var resultado = new ResultadoLexico();
            resultado.AgregarAviso("LEXICO INICIADO");

            try
            {
                for (int numLinea = 1; numLinea <= fuente.NumeroLineas; numLinea++)
                {
                    ProcesarLinea(fuente.ObtenerLinea(numLinea), numLinea, resultado);
                }
                resultado.AgregarAviso("LEXICO FINALIZADO EXITOSAMENTE");
            }
            catch (Exception ex) 
            {
                resultado.AgregarAviso("ERROR GRAVE EN LEXICO: " + ex.Message);
            }
            return resultado;
        }

        private void ProcesarLinea(string lineaOriginal, int numLinea, ResultadoLexico resultado)
        {
            int estado = 0;
            var lexema = new StringBuilder();
            int token = 0;
            // Agregar un espacio al final para forzar el cierre de lexemas
            string linea = (lineaOriginal ?? string.Empty) + " ";

            for (int i = 0; i < linea.Length; i++)
            {
                char c = linea[i];
                int columna = _matriz.ObtenerColumna(c);
                int valorMatriz = _matriz.SiguienteEstado(estado, columna);

                if (valorMatriz == 0)
                {
                    // Reset
                    estado = 0;
                    lexema.Clear();
                    token = 0;
                }

                else if (valorMatriz < 100)
                {
                    // Estado interno (leyendo ID o número)
                    estado = valorMatriz;
                    lexema.Append(c);
                    token = 0;
                }

                else if (valorMatriz == 100)
                {
                    // Fin de ID / palabra reservada
                    string lex = lexema.ToString();
                    token = _palabrasReservadas.ObtenerToken(lex);
                    resultado.Tokens.Add(new Token(token, lex, numLinea));
                    Console.WriteLine($"Token: {token} | Lexema: {lex} | Línea: {numLinea}");
                    lexema.Clear();
                    estado = 0;
                    i--; // reprocesar el mismo carácter en el estado 0
                }

                else if (valorMatriz == 200)
                {
                    // Fin de número entero
                   string lex = lexema.ToString();
                    token = 200; // código para número entero
                    resultado.Tokens.Add(new Token(token, lex, numLinea));
                    Console.WriteLine($"Token: {token} | Lexema: {lex} | Línea: {numLinea}");
                    lexema.Clear();
                    estado = 0;
                    i--; // reprocesar el mismo carácter en el estado 0
                }

                else if (valorMatriz == 300)
                {
                    token = ObtenerTokenSimbolo(c);
                    resultado.Tokens.Add(
                        new Token(token, c.ToString(), numLinea)
                    );

                    estado = 0;
                }

                else if (valorMatriz >= 500)
                {
                    // Esto captura el 501 de la matriz
                    resultado.AgregarAviso($"ERROR: En línea [{numLinea}] símbolo no reconocido: '{c}'");

                    // Reset para continuar con el siguiente carácter
                    estado = 0;
                    lexema.Clear();
                }

            }

        }

        private int ObtenerTokenSimbolo(char c)
        {
            switch (c)
            {
                case ';': return 300;
                case '=': return 301;
                case '+': return 302;
                case '-': return 303;
                case '*': return 304;
                case '>': return 305;
                case '<': return 306;
                case '(': return 307;
                case ')': return 308;
                case '{': return 309;
                case '}': return 310;
                case ',': return 311;
                default: return 501;
            }
        }



    }

}
