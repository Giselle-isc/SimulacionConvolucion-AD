using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions; 

// Necesario para la normal teórica

namespace Simulacion_AD_Convolucion.Logica
{
    public class Estadistica
    {
        public double CalcularAndersonDarling(List<double> datos, double media, double desv)
        {
            int n = datos.Count;
            // 1. Ordenar los datos (Paso fundamental según el libro)
            var Y = datos.OrderBy(x => x).ToList();

            double suma = 0;
            for (int i = 1; i <= n; i++)
            {
                // Calculamos la probabilidad acumulada teórica para cada dato
                // MathNet nos ahorra programar la integral de la normal
                double Fi = Normal.CDF(media, desv, Y[i - 1]);
                double F_ni = Normal.CDF(media, desv, Y[n - i]);

                // Evitar errores de logaritmo con valores extremos
                Fi = Clamp(Fi);
                F_ni = Clamp(F_ni);

                // Fórmula del estadístico A² del método de Anderson-Darling
                suma += (2 * i - 1) * (Math.Log(Fi) + Math.Log(1 - F_ni));
            }

            double aCuadrado = -n - (suma / n);

            // Ajuste para muestras finitas (factor de corrección)
            double aAjustado = aCuadrado * (1 + 0.75 / n + 2.25 / (Math.Pow(n, 2)));
            return Math.Round(aAjustado, 4);
        }

        private double Clamp(double value) => Math.Max(0.000001, Math.Min(0.999999, value));
    }
}