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
            _tokens = tokensTotales;
            _posicionActual = 0;

            if (_tokens.Count > 0)
                _tokenActual = _tokens[0];

          
          
        }

        private void Avanzar() 
        {
            _posicionActual++;

            if (_posicionActual < _tokens.Count)
                _tokenActual = _tokens[_posicionActual];
        }

        private void MatchTipo(int tipoEsperado, string mensajeError) 
        {

        }

        private void MatchLexema(string lexemaEsperado, string mensajeError) 
        {

        }

        private bool CheckLexema(string lexemaEsperado) 
        {
            if (_tokenActual == null)
                return false;

            return _tokenActual.Lexema == lexemaEsperado;   
        }

     


    }

}
