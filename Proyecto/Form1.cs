using System;
using System.Drawing;
using System.Windows.Forms;

namespace Proyecto
{
    public partial class Form1 : Form
    {
        // Paneles para simular las múltiples vistas / pantallas
        private Panel pnlConfig;
        private Label lblTitulo, lblModo, lblJ1, lblJ2;
        private ComboBox cmbModo;
        private TextBox txtJ1, txtJ2;
        private Button btnIniciar;

        private Panel pnlJuego;
        private Label lblTurnoInfo, lblPregunta, lblEstadoJuego;
        private TextBox txtRespuesta;
        private Button btnResponder;

        private Panel pnlResultados;
        private Label lblResTitulo;
        private ListBox lstClasificacion;
        private Button btnReiniciar;

        // Arreglos obligatorios para el almacenamiento temporal de puntajes y nombres
        private string[] nombresArr = new string[20];
        private int[] puntajesArr = new int[20];
        private int totalRecords = 0;

        // Variables de control de estado del juego
        private string jugador1 = "";
        private string jugador2 = "";
        private int modoJuego = 0; // 0 = Individual, 1 = Dos Jugadores
        private int turnoActual = 1; // 1 o 2

        private float n1 = 0, n2 = 0, resultado = 0;
        private char operador = ' ';
        private int conterroes = 0, contadoraciertos = 0;
        private int limite = 11;

        public Form1()
        {
            InitializeComponent();
            ConfigurarInterfazGeneral();
        }

        private void ConfigurarInterfazGeneral()
        {
            this.Text = "Sistema Matemático - Aplicación de Escritorio";
            this.Size = new Size(460, 390);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(45, 45, 48);

            CrearPanelConfig();
            CrearPanelJuego();
            CrearPanelResultados();

            MostrarPanel(pnlConfig);
        }

        private void MostrarPanel(Panel panelMostrar)
        {
            pnlConfig.Visible = (panelMostrar == pnlConfig);
            pnlJuego.Visible = (panelMostrar == pnlJuego);
            pnlResultados.Visible = (panelMostrar == pnlResultados);
        }

        private void CrearPanelConfig()
        {
            pnlConfig = new Panel() { Dock = DockStyle.Fill, BackColor = Color.FromArgb(45, 45, 48) };

            lblTitulo = new Label() { Text = "SISTEMA MATEMÁTICO ESCRITORIO", Location = new Point(35, 20), Size = new Size(380, 30), Font = new Font("Arial", 13, FontStyle.Bold), ForeColor = Color.Yellow };
            lblModo = new Label() { Text = "Modo de Juego:", Location = new Point(35, 65), Size = new Size(150, 20), ForeColor = Color.White };

            cmbModo = new ComboBox() { Location = new Point(35, 90), Size = new Size(370, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbModo.Items.AddRange(new string[] { "Modo Individual", "Modo Acompañado (Dos Jugadores)" });
            cmbModo.SelectedIndex = 0;
            cmbModo.SelectedIndexChanged += (s, e) => {
                lblJ2.Visible = (cmbModo.SelectedIndex == 1);
                txtJ2.Visible = (cmbModo.SelectedIndex == 1);
            };

            lblJ1 = new Label() { Text = "Nombre del Jugador 1:", Location = new Point(35, 130), Size = new Size(200, 20), ForeColor = Color.White };
            txtJ1 = new TextBox() { Location = new Point(35, 155), Size = new Size(370, 25), Font = new Font("Arial", 11) };

            lblJ2 = new Label() { Text = "Nombre del Jugador 2:", Location = new Point(35, 195), Size = new Size(200, 20), ForeColor = Color.White, Visible = false };
            txtJ2 = new TextBox() { Location = new Point(35, 220), Size = new Size(370, 25), Font = new Font("Arial", 11), Visible = false };

            btnIniciar = new Button() { Text = "Iniciar Partida", Location = new Point(115, 275), Size = new Size(210, 40), BackColor = Color.Green, ForeColor = Color.White, Font = new Font("Arial", 10, FontStyle.Bold) };
            btnIniciar.Click += BtnIniciar_Click;

            pnlConfig.Controls.AddRange(new Control[] { lblTitulo, lblModo, cmbModo, lblJ1, txtJ1, lblJ2, txtJ2, btnIniciar });
            this.Controls.Add(pnlConfig);
        }

        private void CrearPanelJuego()
        {
            pnlJuego = new Panel() { Dock = DockStyle.Fill, BackColor = Color.FromArgb(45, 45, 48), Visible = false };

            lblTurnoInfo = new Label() { Text = "Turno de:", Location = new Point(30, 20), Size = new Size(390, 25), Font = new Font("Arial", 12, FontStyle.Bold), ForeColor = Color.Cyan };
            lblPregunta = new Label() { Text = "? + ? = ?", Location = new Point(30, 65), Size = new Size(390, 40), Font = new Font("Arial", 18, FontStyle.Bold), ForeColor = Color.Yellow };

            txtRespuesta = new TextBox() { Location = new Point(30, 130), Size = new Size(210, 30), Font = new Font("Arial", 14) };
            btnResponder = new Button() { Text = "Responder", Location = new Point(255, 128), Size = new Size(160, 35), BackColor = Color.DodgerBlue, ForeColor = Color.White, Font = new Font("Arial", 10, FontStyle.Bold) };
            btnResponder.Click += BtnResponder_Click;

            lblEstadoJuego = new Label() { Text = "Aciertos: 0 | Errores: 0/3", Location = new Point(30, 190), Size = new Size(390, 90), Font = new Font("Arial", 11), ForeColor = Color.White };

            pnlJuego.Controls.AddRange(new Control[] { lblTurnoInfo, lblPregunta, txtRespuesta, btnResponder, lblEstadoJuego });
            this.Controls.Add(pnlJuego);
        }

        private void CrearPanelResultados()
        {
            pnlResultados = new Panel() { Dock = DockStyle.Fill, BackColor = Color.FromArgb(45, 45, 48), Visible = false };

            lblResTitulo = new Label() { Text = "TABLA DE CLASIFICACIÓN LOCAL", Location = new Point(35, 20), Size = new Size(380, 30), Font = new Font("Arial", 13, FontStyle.Bold), ForeColor = Color.Yellow };
            lstClasificacion = new ListBox() { Location = new Point(35, 60), Size = new Size(380, 180), Font = new Font("Consolas", 10) };

            btnReiniciar = new Button() { Text = "Jugar de Nuevo", Location = new Point(115, 260), Size = new Size(210, 40), BackColor = Color.Green, ForeColor = Color.White, Font = new Font("Arial", 10, FontStyle.Bold) };
            btnReiniciar.Click += (s, e) => {
                conterroes = 0;
                contadoraciertos = 0;
                limite = 11;
                txtJ1.Clear();
                txtJ2.Clear();
                cmbModo.SelectedIndex = 0;
                MostrarPanel(pnlConfig);
            };

            pnlResultados.Controls.AddRange(new Control[] { lblResTitulo, lstClasificacion, btnReiniciar });
            this.Controls.Add(pnlResultados);
        }

        private void BtnIniciar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtJ1.Text))
            {
                MessageBox.Show("Por favor ingresa el nombre del Jugador 1.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            modoJuego = cmbModo.SelectedIndex;
            if (modoJuego == 1 && string.IsNullOrWhiteSpace(txtJ2.Text))
            {
                MessageBox.Show("Por favor ingresa el nombre del Jugador 2.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            jugador1 = txtJ1.Text.Trim();
            jugador2 = txtJ2.Text.Trim();
            turnoActual = 1;
            conterroes = 0;
            contadoraciertos = 0;
            limite = 11;

            ActualizarInfoTurno();
            GenerarPregunta();
            MostrarPanel(pnlJuego);
        }

        private void ActualizarInfoTurno()
        {
            string actual = (modoJuego == 1 && turnoActual == 2) ? jugador2 : jugador1;
            lblTurnoInfo.Text = $"Turno de: {actual} (Modo: {(modoJuego == 1 ? "Dos Jugadores" : "Individual")})";
            lblEstadoJuego.Text = $"Aciertos: {contadoraciertos} | Errores: {conterroes}/3 (Vidas restantes: {3 - conterroes})\nNivel actual (Límite numérico): {limite}";
        }

        private void GenerarPregunta()
        {
            n1 = Random.Shared.Next(1, limite);
            n2 = Random.Shared.Next(1, limite);
            int op = Random.Shared.Next(1, 5);

            switch (op)
            {
                case 1: operador = '+'; resultado = n1 + n2; break;
                case 2: operador = '-'; resultado = n1 - n2; break;
                case 3: operador = '*'; resultado = n1 * n2; break;
                case 4: operador = '/'; resultado = (float)Math.Round(n1 / n2, 2); break;
            }

            lblPregunta.Text = $"{n1} {operador} {n2} = ?";
            txtRespuesta.Clear();
            txtRespuesta.Focus();
        }

        private void BtnResponder_Click(object sender, EventArgs e)
        {
            if (float.TryParse(txtRespuesta.Text, out float resUsuario))
            {
                if (resUsuario == resultado)
                {
                    contadoraciertos++;
                    MessageBox.Show("¡Correcto!", "Acierto", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (contadoraciertos == 3)
                    {
                        limite = 101;
                        MessageBox.Show("¡3 aciertos alcanzados! La dificultad aumentó: números hasta 100.", "Nivel Dinámico", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    conterroes++;
                    MessageBox.Show($"Incorrecto. El resultado correcto era {resultado}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    if (conterroes >= 3)
                    {
                        MessageBox.Show("¡Fin del juego! Acumulaste 3 errores.", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        // Guardar en arreglos locales
                        RegistrarPuntajeArray((modoJuego == 1 && turnoActual == 2) ? jugador2 : jugador1, contadoraciertos);
                        MostrarTablaClasificacion();
                        return;
                    }
                }

                if (modoJuego == 1)
                {
                    turnoActual = (turnoActual == 1) ? 2 : 1;
                }

                ActualizarInfoTurno();
                GenerarPregunta();
            }
            else
            {
                MessageBox.Show("Por favor ingresa un valor numérico válido.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void RegistrarPuntajeArray(string nombre, int puntaje)
        {
            if (totalRecords < nombresArr.Length)
            {
                nombresArr[totalRecords] = nombre;
                puntajesArr[totalRecords] = puntaje;
                totalRecords++;
            }
        }

        private void MostrarTablaClasificacion()
        {
            lstClasificacion.Items.Clear();
            lstClasificacion.Items.Add(" N° | JUGADOR              | ACIERTOS");
            lstClasificacion.Items.Add("-------------------------------------");

            for (int i = 0; i < totalRecords; i++)
            {
                lstClasificacion.Items.Add($"  {i + 1} | {nombresArr[i].PadRight(20)} |   {puntajesArr[i]}");
            }

            MostrarPanel(pnlResultados);
        }
    }
}