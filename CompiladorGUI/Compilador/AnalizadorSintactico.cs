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
        private const int TKN_RETURN = 201;
        private const int TKN_REAL = 202;
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

            //ParserBloque();

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
            // consumir VAR si existe
            if (CheckLexema("VAR"))
            {
                Avanzar();
                // aquí podrías ignorar lo demás por ahora
                while (!CheckLexema("BEGIN") && _tokenActual.Tipo != -1)
                    Avanzar();
            }

            if (CheckLexema("BEGIN"))
            {
                Avanzar();

                while (!CheckLexema("END") && _tokenActual.Tipo != -1)
                    Avanzar();

                MatchLexema("END", "Se esperaba END");
            }
        }

    }

}
