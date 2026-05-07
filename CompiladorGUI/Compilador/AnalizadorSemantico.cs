using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador
{
    public class AnalizadorSemantico
    {
        private Dictionary<string, string> tablaSimbolos =
           new Dictionary<string, string>();

        public List<string> Errores = new List<string>();

        public List<Simbolo> TablaSimbolos = new List<Simbolo>();

        public void Analizar(List<Token> tokens)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                // Buscar declaraciones:
                // x : INT;
                if (tokens[i].Tipo == 101) // ID
                {
                    if (i + 2 < tokens.Count &&
                        tokens[i + 1].Lexema == ":" &&
                        (tokens[i + 2].Lexema.ToUpper() == "INT" ||
                         tokens[i + 2].Lexema.ToUpper() == "FLOAT"))
                    {
                        string nombre = tokens[i].Lexema;
                        string tipo = tokens[i + 2].Lexema.ToUpper();

                        if (!tablaSimbolos.ContainsKey(nombre))
                        {
                            tablaSimbolos.Add(nombre, tipo);

                            TablaSimbolos.Add(new Simbolo
                            {
                                Nombre = nombre,
                                Tipo = tipo
                            });
                        }
                        else
                        {
                            Errores.Add($"Error semántico: La variable '{nombre}' ya fue declarada.");
                        }
                    }
                }
            }
        }
    }
}
