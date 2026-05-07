using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador
{
    public class AnalizadorSintactico
    {
        private List<Token> _tokens = new List<Token>();
        private int _posicionActual;
        private Token _tokenActual;
        public List<string> Errores = new List<string>();

        // Constantes para tokens usando PalabrasReservadas y AnalizadorLexico
        private const int TKN_ID = 101;
        private const int TKN_PROGRAM = 102;
        private const int TKN_VAR = 103;
        private const int TKN_PROCEDURE = 104;
        private const int TKN_BEGIN = 105;
        private const int TKN_END = 106;
        private const int TKN_INT = 107;
        private const int TKN_FLOAT = 108;
        private const int TKN_IF = 109;
        private const int TKN_THEN = 110;
        private const int TKN_ELSE = 111;
        private const int TKN_WHILE = 112;
        private const int TKN_DO = 113;
        private const int TKN_PRINT = 114;
        private const int TKN_WRITELN = 115;
        private const int TKN_WRITE = 116;
        private const int TKN_ENTERO = 200;
        //private const int TKN_RETURN = 201;
        private const int TKN_REAL = 201;
        private const int TKN_COMENTARIO = 400;

        public void Parse(List<Token> tokensTotales) 
        {
            _tokens = tokensTotales.Where(t => t.Tipo != TKN_COMENTARIO).ToList();

            Errores.Clear();
            _posicionActual = 0;

            if (_tokens.Count == 0)
            {
                Errores.Add("El código fuente está vacío o no generó tokens válidos.");
                return;
            }

            _tokenActual = _tokens[_posicionActual];

            try
            {
                ParserPrograma();

                if (_posicionActual < _tokens.Count && _tokenActual.Tipo != -1)
                {
                    Errores.Add($"Error en línea {_tokenActual.Linea}: Tokens inesperados después del fin del programa ('{_tokenActual.Lexema}').");
                }
            }
            catch (Exception ex)
            {
                Errores.Add(ex.Message);
            }
        }

        private void ParserPrograma()
        {
            MatchTipo(TKN_PROGRAM, "Se esperaba 'PROGRAM'");
            MatchTipo(TKN_ID, "Se esperaba identificador del programa");
            MatchLexema(";", "Falta ';' después del identificador del programa");

            ParserBloque();

            MatchLexema(".", "Falta '.' al finalizar el programa");
        }

        private void Avanzar() 
        {
            _posicionActual++;
            if (_posicionActual < _tokens.Count)
            {
                _tokenActual = _tokens[_posicionActual];
            }
            else
            {
                _tokenActual = new Token(-1, "EOF", _tokenActual?.Linea ?? 0);
            }
        }

        private void MatchTipo(int tipoEsperado, string mensajeError) 
        {
            if (_tokenActual.Tipo == tipoEsperado)
            {
                Avanzar();
            }
            else
            {
                throw new Exception($"Error sintáctico en línea {_tokenActual.Linea}: {mensajeError}. Encontrado '{_tokenActual.Lexema}'.");
            }
        }

        private void MatchLexema(string lexemaEsperado, string mensajeError) 
        {
            if (_tokenActual.Lexema == lexemaEsperado)
            {
                Avanzar();
            }
            else
            {
                throw new Exception($"Error sintáctico en línea {_tokenActual.Linea}: {mensajeError}. Encontrado '{_tokenActual.Lexema}'.");
            }
        }

        private bool CheckLexema(string lexemaEsperado) 
        {
            if (_tokenActual == null)
                return false;

            return _tokenActual.Lexema.Trim().ToUpper() == lexemaEsperado.ToUpper();
        }

        private void ParserBloque() 
        {
            // Opcional: Sección de variables
            if (_tokenActual.Tipo == TKN_VAR)
            {
                ParserDeclaracionesVariables();
            }

            // Opcional: Procedimientos
            while (_tokenActual.Tipo == TKN_PROCEDURE)
            {
                ParserDeclaracionProcedimiento();
            }

            MatchTipo(TKN_BEGIN, "Se esperaba 'BEGIN'");
            ParserInstrucciones();
            MatchTipo(TKN_END, "Se esperaba 'END'");
        }

        private void ParserDeclaracionesVariables() 
        {
            MatchTipo(TKN_VAR, "Se esperaba 'VAR'");

            // Puede haber múltiples variables declaradas
            while (_tokenActual.Tipo == TKN_ID)
            {
                MatchTipo(TKN_ID, "Se esperaba identificador de variable");

                while (CheckLexema(","))
                {
                    Avanzar(); // consumir ','
                    MatchTipo(TKN_ID, "Se esperaba identificador después de la ','");
                }

                MatchLexema(":", "Falta ':' en la declaración de variable");
                ParserTipo();
                MatchLexema(";", "Falta ';' al final de la declaración de variable");
            }

        }

        private void ParserTipo() 
        {
            if (_tokenActual.Tipo == TKN_INT)
            {
                Avanzar(); // consumir INT
            }
            else if (_tokenActual.Tipo == TKN_FLOAT)
            {
                Avanzar(); // consumir FLOAT
            }
            else
            {
                throw new Exception($"Error sintáctico en línea {_tokenActual.Linea}: Se esperaba un tipo de dato válido (INT o FLOAT).");
            }
        }

        private void ParserDeclaracionProcedimiento() 
        {
            MatchTipo(TKN_PROCEDURE, "Se esperaba 'PROCEDURE'");
            MatchTipo(TKN_ID, "Se esperaba nombre del procedimiento");

            if (CheckLexema("("))
            {
                Avanzar(); // Consumir '('
                           // Suponiendo parámetros variables simples
                if (_tokenActual.Tipo == TKN_ID)
                {
                    MatchTipo(TKN_ID, "Identificador de parámetro");
                    MatchLexema(":", "Falta ':'");
                    ParserTipo();
                }
                MatchLexema(")", "Falta ')'");
            }
            MatchLexema(";", "Falta ';' después de cabecera de procedimiento");

            ParserBloque();
            MatchLexema(";", "Falta ';' después de cuerpo de procedimiento");
        }

        private void ParserInstrucciones() 
        {
            ParserInstruccion();
            while (CheckLexema(";"))
            {
                Avanzar(); // consumir ';'
                if (_tokenActual.Tipo != TKN_END) // En pascal antes del END el ';' puede existir o no, validamos
                {
                    ParserInstruccion();
                }
            }
        }

        private void ParserInstruccion() 
        {
            // Asignación (ID := ...)
            if (_tokenActual.Tipo == TKN_ID)
            {
                Avanzar(); // consumir ID
                MatchLexema(":=", "Se esperaba operador de asignación ':='");
                ParserExpresion();
            }
            // Condicional IF
            else if (_tokenActual.Tipo == TKN_IF)
            {
                Avanzar(); // consumir IF
                ParserExpresion();
                MatchTipo(TKN_THEN, "Se esperaba 'THEN'");
                ParserInstruccion();

                if (_tokenActual.Tipo == TKN_ELSE)
                {
                    Avanzar(); // consumir ELSE
                    ParserInstruccion();
                }
            }
            // Bucle WHILE
            else if (_tokenActual.Tipo == TKN_WHILE)
            {
                Avanzar(); // consumir WHILE
                ParserExpresion();
                MatchTipo(TKN_DO, "Se esperaba 'DO'");
                ParserInstruccion();
            }
            // Bloque anidado BEGIN ... END
            else if (_tokenActual.Tipo == TKN_BEGIN)
            {
                Avanzar(); // consumir BEGIN
                ParserInstrucciones();
                MatchTipo(TKN_END, "Se esperaba 'END' en bloque anidado");
            }
            // Imprimir (WRITE / WRITELN / PRINT)
            else if (_tokenActual.Tipo == TKN_WRITE || _tokenActual.Tipo == TKN_WRITELN || _tokenActual.Tipo == TKN_PRINT)
            {
                Avanzar(); // consumir palabra de salida
                MatchLexema("(", "Falta '(' para función de impresión");
                ParserExpresion();
                while (CheckLexema(","))
                {
                    Avanzar();
                    ParserExpresion();
                }
                MatchLexema(")", "Falta ')' en la función de impresión");
            }
        }

        private void ParserExpresion() 
        {
            ParserTermino();
            while (CheckLexema("+") || CheckLexema("-") || CheckLexema("==") || CheckLexema(">") || CheckLexema("<") || CheckLexema(">=") || CheckLexema("<=") || CheckLexema("<>"))
            {
                Avanzar(); // consumir operador
                ParserTermino();
            }
        }

        private void ParserTermino() 
        {
            ParserFactor();
            while (CheckLexema("*") || CheckLexema("/"))
            {
                Avanzar(); // consumir operador
                ParserFactor();
            }
        }

        private void ParserFactor() 
        {
            if (_tokenActual.Tipo == TKN_ID)
            {
                Avanzar(); // Variable
            }
            else if (_tokenActual.Tipo == TKN_ENTERO || _tokenActual.Tipo == TKN_REAL)
            {
                Avanzar(); // Número
            }
            else if (CheckLexema("("))
            {
                Avanzar(); // Parentesis abre
                ParserExpresion();
                MatchLexema(")", "Se esperaba ')' tras expresión");
            }
            else
            {
                throw new Exception($"Error sintáctico en línea {_tokenActual.Linea}: Se esperaba un FACTOR (Identificador, Número o '('), pero se encontró '{_tokenActual.Lexema}'");
            }
        }

    }

}
