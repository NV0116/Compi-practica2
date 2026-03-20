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
            string linea = (lineaOriginal ?? string.Empty) + " "; // Espacio centinela

            for (int i = 0; i < linea.Length; i++)
            {
                char c = linea[i];

                // ===== DETECCIÓN DE COMENTARIO // =====
                if (c == '/' && i + 1 < linea.Length && linea[i + 1] == '/')
                {
                    string comentario = lineaOriginal.Substring(i).Trim();
                    resultado.AgregarAviso($"[Comentario] Línea {numLinea}: {comentario}");
                    break; // se deja de analizar la línea
                }

                int columna = _matriz.ObtenerColumna(c);
                int valorMatriz = _matriz.SiguienteEstado(estado, columna);

                // --- GESTIÓN DE ERRORES ---
                if (valorMatriz >= 500)
                {
                    resultado.AgregarAviso($"[Error {valorMatriz}] Línea {numLinea}: Símbolo '{c}' no válido.");
                    estado = 0;
                    lexema.Clear();
                    continue;
                }

                // --- ESTADO INICIAL / ESPACIOS ---
                if (valorMatriz == 0)
                {
                    estado = 0;
                    lexema.Clear();
                }
                // --- ESTADOS TRANSITORIOS ---
                else if (valorMatriz < 100)
                {
                    estado = valorMatriz;
                    lexema.Append(c);
                }
                // --- ESTADOS DE ACEPTACIÓN ---
                else
                {
                    int tokenFinal = valorMatriz;
                    string lexFinal;

                    // Operadores compuestos (==, >=, <=)
                    if (valorMatriz == 305 || valorMatriz == 307 || valorMatriz == 309)
                    {
                        lexema.Append(c);
                        lexFinal = lexema.ToString();
                    }
                    else
                    {
                        lexFinal = lexema.ToString();

                        if (string.IsNullOrEmpty(lexFinal))
                        {
                            lexFinal = c.ToString();
                            tokenFinal = ObtenerCodigoSimboloEspecial(c);
                        }
                        else
                        {
                            i--;
                            if (tokenFinal == 100 || tokenFinal == 101)
                                tokenFinal = _palabrasReservadas.ObtenerToken(lexFinal);
                        }
                    }

                    resultado.Tokens.Add(new Token(tokenFinal, lexFinal, numLinea));
                    lexema.Clear();
                    estado = 0;
                }
            }
        }


        public static int ObtenerCodigoSimboloEspecial(char c)
        {
            switch (c)
            {
                case ';': return 300;
                case '+': return 302;
                case '-': return 303;
                case '=': return 304;
                case '>': return 306;
                case '<': return 308;
                case '(': return 311; 
                case ')': return 312;
                case '{': return 313;
                case '}': return 314;
                case ',': return 315;
                case '\'': return 310; 
                default: return 300;
            }
        }




    }

}
