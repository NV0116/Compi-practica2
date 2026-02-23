
using System;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms;
using System.Windows.Forms;

namespace Compilador.UI.Forms
{
    public partial class FrmCompilador : Form
    {
        private RichTextBox txtEditor;
        private Panel pnlLineNumbers;
        private bool isDarkTheme = false;

        public FrmCompilador()
        {
            InitializeComponent();
            InicializarEditor();
            AplicarTemaClaro();

            btnTema.Click += BtnTema_Click;
        }


        private void InicializarEditor()
        {
            pnlLineNumbers = new Panel
            {
                Dock = DockStyle.Left,
                Width = 50
            };
            pnlLineNumbers.Paint += PnlLineNumbers_Paint;

            txtEditor = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 11F),
                WordWrap = false,
                BorderStyle = BorderStyle.None,
                ScrollBars = RichTextBoxScrollBars.Vertical
            };

            txtEditor.VScroll += (s, e) => pnlLineNumbers.Invalidate();
            txtEditor.TextChanged += (s, e) => pnlLineNumbers.Invalidate();
            txtEditor.Resize += (s, e) => pnlLineNumbers.Invalidate();

            splitEditor.Panel1.Controls.Add(txtEditor);
            splitEditor.Panel1.Controls.Add(pnlLineNumbers);
        }


        private void PnlLineNumbers_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(pnlLineNumbers.BackColor);

            int firstLine = txtEditor.GetLineFromCharIndex(
                txtEditor.GetCharIndexFromPosition(new Point(0, 0)));

            int lastLine = txtEditor.GetLineFromCharIndex(
                txtEditor.GetCharIndexFromPosition(
                    new Point(0, txtEditor.Height)));

            int lineHeight = txtEditor.Font.Height;
            int y = 2;

            for (int i = firstLine; i <= lastLine + 1; i++)
            {
                string line = (i + 1).ToString();
                SizeF size = e.Graphics.MeasureString(line, txtEditor.Font);

                e.Graphics.DrawString(
                    line,
                    txtEditor.Font,
                    Brushes.Gray,
                    pnlLineNumbers.Width - size.Width - 5,
                    y
                );

                y += lineHeight;
            }
        }


        private void BtnTema_Click(object sender, EventArgs e)
        {
            isDarkTheme = !isDarkTheme;

            if (isDarkTheme)
                AplicarTemaOscuro();
            else
                AplicarTemaClaro();
        }

        private void AplicarTemaClaro()
        {
            txtEditor.BackColor = Color.White;
            txtEditor.ForeColor = Color.Black;
            txtTokens.BackColor = Color.White;
            txtTokens.ForeColor = Color.Black;
            txtEstatus.BackColor = Color.White;
            txtEstatus.ForeColor = Color.Black;
            gridSimbolos.BackgroundColor = Color.White;
            gridSimbolos.DefaultCellStyle.BackColor = Color.White;
            gridSimbolos.DefaultCellStyle.ForeColor = Color.Black;
            pnlLineNumbers.BackColor = Color.FromArgb(245, 245, 245);
        }

        private void AplicarTemaOscuro()
        {
            txtEditor.BackColor = Color.FromArgb(30, 30, 30);
            txtEditor.ForeColor = Color.Gainsboro;
            txtTokens.BackColor = Color.FromArgb(30, 30, 30);
            txtTokens.ForeColor = Color.Gainsboro;
            txtEstatus.BackColor = Color.FromArgb(30, 30, 30);
            txtEstatus.ForeColor = Color.Gainsboro;
            gridSimbolos.BackgroundColor = Color.FromArgb(30, 30, 30);
            gridSimbolos.DefaultCellStyle.BackColor = Color.FromArgb(30, 30, 30);
            gridSimbolos.DefaultCellStyle.ForeColor = Color.Gainsboro;
            pnlLineNumbers.BackColor = Color.FromArgb(45, 45, 48);
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            limpiar();
        }
        private void limpiar()
        {
            try
            {
                txtEditor.Clear();
                txtTokens.Clear();
                txtEstatus.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al limpiar los campos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAbrir_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.Filter = "Archivos de texto (*.txt)|*.txt|Todos los archivos (*.*)|*.*";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    limpiar();
                    string filePath = openFileDialog1.FileName;
                    string fileContent = File.ReadAllText(filePath);
                    txtEditor.Text = fileContent;

                    txtEstatus.AppendText("Archivo abierto: " + filePath + Environment.NewLine);
                }
                else
                {
                    txtEstatus.AppendText("Operación de apertura cancelada." + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir el archivo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                saveFileDialog1.Filter = "Archivos de texto (*.txt)|*.txt|Todos los archivos (*.*)|*.*";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog1.FileName;
                    File.WriteAllText(filePath, txtEditor.Text);
                    txtEstatus.AppendText("Archivo guardado: " + filePath + Environment.NewLine);
                }
                else
                {
                    txtEstatus.AppendText("Operación de guardado cancelada." + Environment.NewLine);
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar el archivo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCompilar_Click(object sender, EventArgs e)
        {
            // 1. Validación de texto: si está vacío o solo tiene espacios
            if (string.IsNullOrWhiteSpace(txtEditor.Text))
            {
                MessageBox.Show("El editor de código está vacío. Ingrese texto para compilar.",
                                "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                txtEstatus.Clear();
                return;
            }

            // 2. Mensaje inicial en el estatus
            txtEstatus.Clear();
            txtEstatus.AppendText("Analizador léxico iniciado..." + Environment.NewLine);

            // 3. Llamada al proceso de análisis
            EjecutarAnalisisLexico(txtEditor.Text);
        }
        //__________________________________________________________________________________________________________//
        private void EjecutarAnalisisLexico(string codigo)
        {
            txtTokens.Clear();
            txtEstatus.AppendText("Analizando tokens..." + Environment.NewLine);

            // Definición de patrones sin espacios en los nombres de grupo
            var definiciones = new (string Patron, string Tipo)[]
            {
        (@"\b(if|else|while|for|int|float|string|return)\b", "PalabraReservada"),
        (@"[a-zA-Z_][a-zA-Z0-9_]*", "Identificador"),
        (@"\d+(\.\d+)?", "Numero"),
        (@"[\+\-\*/=]", "Operador"),
        (@"[;(){}\[\]]", "Delimitador"),
        (@"\s+", "Espacio")
            };

            try
            {
                // Unimos los patrones en una sola expresión regular
                string patronGlobal = string.Join("|", definiciones.Select(d => $"(?<{d.Tipo}>{d.Patron})"));
                Regex regex = new Regex(patronGlobal);
                MatchCollection coincidencias = regex.Matches(codigo);

                foreach (Match match in coincidencias)
                {
                    foreach (var def in definiciones)
                    {
                        if (match.Groups[def.Tipo].Success)
                        {
                            if (def.Tipo != "Espacio") // No mostramos los espacios en la lista
                            {
                                txtTokens.AppendText($"<{def.Tipo}> : {match.Value}" + Environment.NewLine);
                            }
                            break;
                        }
                    }
                }
                txtEstatus.AppendText("--- Análisis léxico finalizado con éxito ---" + Environment.NewLine);
            }
            catch (Exception ex)
            {
                txtEstatus.AppendText("Error crítico en el análisis: " + ex.Message + Environment.NewLine);
            }
        }

        // Estructura para almacenar los tokens
        
    }
    public class Token
    {
        public string Tipo { get; set; }
        public string Valor { get; set; }
    }
}
