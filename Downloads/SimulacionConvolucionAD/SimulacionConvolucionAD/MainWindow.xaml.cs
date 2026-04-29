using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Simulacion_AD_Convolucion.Logica;
using LiveCharts;
using LiveCharts.Wpf;

namespace Simulacion_AD_Convolucion
{
    public partial class MainWindow : Window
    {
        Generador gen = new Generador();
        Estadistica stats = new Estadistica();
        List<double> datosGenerados = new List<double>();
        public ChartValues<double> GraficoValores { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            GraficoValores = new ChartValues<double>();
            DataContext = this;
        }

        private void BtnGenerarAuto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double m = double.Parse(txtMedia.Text);
                double d = double.Parse(txtDesv.Text);
                int n = int.Parse(txtN.Text);

                datosGenerados = gen.GenerarNormal(m, d, n);
                ActualizarInterfaz();
            }
            catch { MessageBox.Show("Revisa los datos de Media, Desv y N."); }
        }

        private void BtnGenerarManual_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var lista = txt12Numeros.Text.Split(',').Select(n => double.Parse(n.Trim())).ToList();
                if (lista.Count != 12) { MessageBox.Show("¡Deben ser exactamente 12 números!"); return; }

                double m = double.Parse(txtMedia.Text);
                double d = double.Parse(txtDesv.Text);

                double resultado = gen.GenerarUnoManual(lista, m, d);
                datosGenerados.Add(resultado);
                ActualizarInterfaz();
            }
            catch { MessageBox.Show("Verifica los 12 números y los parámetros μ/σ."); }
        }

        private void BtnCalcularAD_Click(object sender, RoutedEventArgs e)
        {
            if (!datosGenerados.Any()) return;

            try
            {
                double m = double.Parse(txtMedia.Text);
                double d = double.Parse(txtDesv.Text);
                double ad = stats.CalcularAndersonDarling(datosGenerados, m, d);

                txtResultadoAD.Text = ad.ToString("F4");

                if (ad < 0.752)
                {
                    lblEstado.Text = "ACEPTADO"; // Corregido: .Text en lugar de .Content
                    borderVeredicto.Background = Brushes.DarkGreen;
                }
                else
                {
                    lblEstado.Text = "RECHAZADO"; // Corregido: .Text en lugar de .Content
                    borderVeredicto.Background = Brushes.DarkRed;
                }
            }
            catch { MessageBox.Show("Error al calcular. Revisa los datos."); }
        }

        private void ActualizarInterfaz()
        {
            dgDatos.ItemsSource = null;
            dgDatos.ItemsSource = datosGenerados.Select(x => new { Valor = Math.Round(x, 4) }).ToList();

            GraficoValores.Clear();
            var ordenados = datosGenerados.OrderBy(x => x).ToList();
            int paso = Math.Max(1, ordenados.Count / 50);
            for (int i = 0; i < ordenados.Count; i += paso) GraficoValores.Add(ordenados[i]);
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            datosGenerados.Clear();
            GraficoValores.Clear();
            dgDatos.ItemsSource = null;
            txtResultadoAD.Text = "---";
            lblEstado.Text = "SIN DATOS";
            borderVeredicto.Background = new SolidColorBrush(Color.FromRgb(74, 76, 79));
        }
    }
}