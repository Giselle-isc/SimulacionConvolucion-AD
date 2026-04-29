using System;
using System.Collections.Generic;
using System.Linq;

namespace Simulacion_AD_Convolucion.Logica
{
    public class Generador
    {
        private Random _rnd = new Random();

        // Método Base: Convolución Normal
        public List<double> GenerarNormal(double media, double desv, int n)
        {
            List<double> lista = new List<double>();
            for (int i = 0; i < n; i++)
            {
                double suma = 0;
                for (int j = 0; j < 12; j++) suma += _rnd.NextDouble();
                lista.Add(desv * (suma - 6) + media);
            }
            return lista;
        }

        // --- ESTE ES EL MÉTODO QUE TE FALTABA Y CAUSA EL ERROR ---
        public double GenerarUnoManual(List<double> mis12Rnd, double media, double desv)
        {
            // La suma de los 12 números aleatorios manuales
            double suma = mis12Rnd.Sum();
            // Aplicamos la fórmula de convolución: X = σ * (ΣR - 6) + μ
            return desv * (suma - 6) + media;
        }
        // -------------------------------------------------------

        // Derivado 1: Convolución de Exponenciales (Erlang)
        public List<double> GenerarErlang(double media, int k, int n)
        {
            List<double> resultados = new List<double>();
            double lambda = 1.0 / media;
            for (int i = 0; i < n; i++)
            {
                double producto = 1.0;
                for (int j = 0; j < k; j++) producto *= _rnd.NextDouble();
                resultados.Add(-(1.0 / lambda) * Math.Log(producto));
            }
            return resultados;
        }

        // Derivado 2: Suma de Bernoulli (Binomial)
        public List<double> GenerarBinomial(int n, double p, int total)
        {
            List<double> resultados = new List<double>();
            for (int i = 0; i < total; i++)
            {
                int exitos = 0;
                for (int j = 0; j < n; j++)
                {
                    if (_rnd.NextDouble() < p) exitos++;
                }
                resultados.Add((double)exitos);
            }
            return resultados;
        }
    }
}
