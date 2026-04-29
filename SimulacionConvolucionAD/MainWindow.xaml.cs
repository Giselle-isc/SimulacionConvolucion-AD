using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Simulacion_AD_Convolucion.Logica;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Controls;

namespace Simulacion_AD_Convolucion
{
    public partial class MainWindow : Window
    {
        Generador gen = new Generador();
        Estadistica stats = new Estadistica();
        List<double> datosGenerados = new List<double>();
        int contadorManual = 1;

        public ChartValues<double> GraficoValores { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            GraficoValores = new ChartValues<double>();
            DataContext = this;
        }

        public class ResultadoFila
        {
            public int Id { get; set; }
            public double Valor { get; set; }
        }

        // --- LÓGICA DE GENERACIÓN ---

        private void BtnGenerarAuto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Reset visual
                txtResultadoAD.Text = "---";
                lblEstado.Text = "ESPERANDO PRUEBA";
                borderVeredicto.Background = (Brush)new BrushConverter().ConvertFrom("#4A4C4F");

                int n = int.Parse(txtN.Text);
                datosGenerados.Clear();
                dgDatos.Items.Clear();

                // Switch para decidir qué método usar según el ComboBox
                switch (CmbMetodo.SelectedIndex)
                {
                    case 0: // Normal
                        double m = double.Parse(txtMedia.Text);
                        double d = double.Parse(txtDesv.Text);
                        datosGenerados = gen.GenerarNormal(m, d, n);
                        break;
                    case 1: // Erlang
                        double me = double.Parse(txtMediaE.Text);
                        int k = int.Parse(txtK.Text);
                        datosGenerados = gen.GenerarErlang(me, k, n);
                        break;
                    case 2: // Binomial
                        int nb = int.Parse(txtN_Bin.Text);
                        double p = double.Parse(txtP_Bin.Text);
                        datosGenerados = gen.GenerarBinomial(nb, p, n);
                        break;
                }

                // Llenar tabla
                for (int i = 0; i < datosGenerados.Count; i++)
                {
                    dgDatos.Items.Add(new ResultadoFila { Id = i + 1, Valor = Math.Round(datosGenerados[i], 4) });
                }

                ActualizarInterfaz();
            }
            catch
            {
                MessageBox.Show("Verifica que todos los parámetros del método seleccionado sean correctos.");
            }
        }

        private void BtnGenerarManual_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CmbMetodo.SelectedIndex != 0)
                {
                    MessageBox.Show("El modo manual solo está configurado para la distribución Normal (12 números).");
                    return;
                }

                string[] partes = txt12Numeros.Text.Split(',');
                if (partes.Length != 12)
                {
                    MessageBox.Show("Debes ingresar exactamente 12 números.");
                    return;
                }

                List<double> mis12 = partes.Select(p => double.Parse(p.Trim())).ToList();
                double resultado = gen.GenerarUnoManual(mis12, double.Parse(txtMedia.Text), double.Parse(txtDesv.Text));

                datosGenerados.Add(resultado);
                dgDatos.Items.Add(new ResultadoFila { Id = contadorManual++, Valor = Math.Round(resultado, 4) });

                ActualizarInterfaz();
            }
            catch { MessageBox.Show("Error en los números manuales. Revisa el formato."); }
        }

        // --- PRUEBA ESTADÍSTICA ---

        private void BtnCalcularAD_Click(object sender, RoutedEventArgs e)
        {
            if (datosGenerados.Count < 2)
            {
                MessageBox.Show("Necesitas generar datos primero.");
                return;
            }

            try
            {
                double m = double.Parse(txtMedia.Text);
                double d = double.Parse(txtDesv.Text);

                double resultadoAD = stats.CalcularAndersonDarling(datosGenerados, m, d);
                txtResultadoAD.Text = resultadoAD.ToString("F4");

                // Veredicto (Límite para Normalidad es 0.752)
                if (resultadoAD < 0.752)
                {
                    lblEstado.Text = "ACEPTADO (Es Normal)";
                    borderVeredicto.Background = Brushes.Green;
                }
                else
                {
                    lblEstado.Text = "RECHAZADO";
                    borderVeredicto.Background = Brushes.DarkRed;
                }
            }
            catch { MessageBox.Show("Asegúrate de tener Media y Desviación para la prueba."); }
        }

        // --- INTERFAZ Y CONTROLES ---

        private void CmbMetodo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InputsNormal == null || InputsErlang == null || InputsBinomial == null) return;

            // Mostrar/Ocultar paneles
            InputsNormal.Visibility = (CmbMetodo.SelectedIndex == 0) ? Visibility.Visible : Visibility.Collapsed;
            InputsErlang.Visibility = (CmbMetodo.SelectedIndex == 1) ? Visibility.Visible : Visibility.Collapsed;
            InputsBinomial.Visibility = (CmbMetodo.SelectedIndex == 2) ? Visibility.Visible : Visibility.Collapsed;

            // Deshabilitar botón de Anderson-Darling si no es Normal
            if (BtnCalcularAD != null)
            {
                BtnCalcularAD.IsEnabled = (CmbMetodo.SelectedIndex == 0);
            }
        }

        private void ActualizarInterfaz()
        {
            GraficoValores.Clear();
            var ordenados = datosGenerados.OrderBy(x => x).ToList();
            int paso = Math.Max(1, ordenados.Count / 50);
            for (int i = 0; i < ordenados.Count; i += paso)
            {
                GraficoValores.Add(ordenados[i]);
            }
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            dgDatos.Items.Clear();
            datosGenerados.Clear();
            GraficoValores.Clear();
            contadorManual = 1;
            txtResultadoAD.Text = "---";
            lblEstado.Text = "SIN DATOS";
            borderVeredicto.Background = (Brush)new BrushConverter().ConvertFrom("#4A4C4F");
            txt12Numeros.Clear();
        }
    }
}