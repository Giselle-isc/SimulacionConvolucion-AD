using System;
using System.Collections.Generic;
using System.Linq;

namespace Simulacion_AD_Convolucion.Logica
{
    public class Generador
    {
        private Random _rnd = new Random();

        // MODO AUTOMÁTICO: Genera N datos usando aleatorios del sistema
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

        // MODO MANUAL: Transforma 12 números específicos en 1 dato normal
        public double GenerarUnoManual(List<double> mis12Rnd, double media, double desv)
        {
            double suma = mis12Rnd.Sum();
            return desv * (suma - 6) + media;
        }
    }
}