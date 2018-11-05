using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GraficadorSeñales
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Variables Globales
        double amplitudMaxima = 1;
        Señal señal;
        Señal señal_2;
        Señal señalResultado;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BotonGraficar_Click(object sender, RoutedEventArgs e)
        {
            double tiempoInicial = double.Parse(txt_TiempoInicial.Text);
            double tiempoFinal = double.Parse(txt_TiempoFinal.Text);
            double frecuenciaMuestreo = double.Parse(txt_FrecuenciaDeMuestreo.Text);

            switch (cb_TipoSeñal.SelectedIndex)
            {
                // Señal Senoidal
                case 0:
                    double amplitud = double.Parse(((ConfiguracionSeñalSenoidal)(panelConfiguracion.Children[0])).txt_Amplitud.Text);
                    double fase = double.Parse(((ConfiguracionSeñalSenoidal)(panelConfiguracion.Children[0])).txt_Fase.Text);
                    double frecuencia = double.Parse(((ConfiguracionSeñalSenoidal)(panelConfiguracion.Children[0])).txt_Frecuencia.Text);

                    señal = new SeñalSenoidal(amplitud, fase, frecuencia);
                    break;

                // Señal Rampa
                case 1:
                    señal = new SeñalRampa();
                    break;

                // Señal Exponencial
                case 2:
                    double alpha = double.Parse(((ConfiguracionSeñalExponencial)(panelConfiguracion.Children[0])).txt_Alpha.Text);
                    señal = new SeñalExponencial(alpha);
                    break;

                // Señal Rectangular
                case 3:
                    señal = new SeñalRectangular();
                    break;

                default:
                    señal = null;
                    break;
            }

            switch (cb_TipoSeñal_2.SelectedIndex)
            {
                // Señal Senoidal
                case 0:
                    double amplitud = double.Parse(((ConfiguracionSeñalSenoidal)(panelConfiguracion_2.Children[0])).txt_Amplitud.Text);
                    double fase = double.Parse(((ConfiguracionSeñalSenoidal)(panelConfiguracion_2.Children[0])).txt_Fase.Text);
                    double frecuencia = double.Parse(((ConfiguracionSeñalSenoidal)(panelConfiguracion_2.Children[0])).txt_Frecuencia.Text);

                    señal_2 = new SeñalSenoidal(amplitud, fase, frecuencia);
                    break;

                // Señal Rampa
                case 1:
                    señal_2 = new SeñalRampa();
                    break;

                // Señal Exponencial
                case 2:
                    double alpha = double.Parse(((ConfiguracionSeñalExponencial)(panelConfiguracion_2.Children[0])).txt_Alpha.Text);
                    señal_2 = new SeñalExponencial(alpha);
                    break;

                // Señal Rectangular
                case 3:
                    señal_2 = new SeñalRectangular();
                    break;

                default:
                    señal_2 = null;
                    break;
            }

            // Primer Señal
            señal.TiempoInicial = tiempoInicial;
            señal.TiempoFinal = tiempoFinal;
            señal.FrecuenciaMuestreo = frecuenciaMuestreo;

            // Segunda Señal
            señal_2.TiempoInicial = tiempoInicial;
            señal_2.TiempoFinal = tiempoFinal;
            señal_2.FrecuenciaMuestreo = frecuenciaMuestreo;

            señal.construirSeñalDigital();
            señal_2.construirSeñalDigital();

            // Truncar
            if ((bool)ckb_Truncado.IsChecked)
            {
                double n = double.Parse(txt_Truncado.Text);
                señal.truncar(n);
            }

            if ((bool)ckb_Truncado_2.IsChecked)
            {
                double n = double.Parse(txt_Truncado.Text);
                señal.truncar(n);
            }

            // Escalar
            if ((bool)ckb_Escala.IsChecked)
            {
                double factorEscala = double.Parse(txt_EscalaAmplitud.Text);
                señal.escalar(factorEscala);
            }

            if ((bool)ckb_Escala_2.IsChecked)
            {
                double factorEscala = double.Parse(txt_EscalaAmplitud.Text);
                señal.escalar(factorEscala);
            }

            // Desplazar
            if ((bool)ckb_Desplazamiento.IsChecked)
            {
                double factorDesplazamiento = double.Parse(txt_Desplazamiento.Text);
                señal.desplazar(factorDesplazamiento);
            }

            if ((bool)ckb_Desplazamiento_2.IsChecked)
            {
                double factorDesplazamiento = double.Parse(txt_Desplazamiento.Text);
                señal.desplazar(factorDesplazamiento);
            }

            // Actualizar
            señal.actualizarAmplitudMaxima();
            señal_2.actualizarAmplitudMaxima();

            // Definición de la amplitud máxima en función de la señal de mayor amplitud
            if (señal.AmplitudMaxima > señal_2.AmplitudMaxima)
            {
                amplitudMaxima = señal.AmplitudMaxima;
            }
            else
            {
                amplitudMaxima = señal_2.AmplitudMaxima;
            }

            // Limpieza de polylines
            plnGrafica.Points.Clear();
            plnGrafica_2.Points.Clear();

            // Impresión de la amplitud máxima en los labels de la ventana.
            lbl_AmplitudMaxima.Text = amplitudMaxima.ToString("F");
            lbl_AmplitudMinima.Text = "-" + amplitudMaxima.ToString("F");

            if (señal != null)
            {
                // Sirve para recorrer una coleccion o arreglo
                foreach (Muestra muestra in señal.Muestras)
                {
                    plnGrafica.Points.Add(new Point((muestra.X - tiempoInicial) * scrContenedor.Width, (muestra.Y / amplitudMaxima * ((scrContenedor.Height / 2) - 30) * -1 + (scrContenedor.Height / 2))));
                }
            }

            if (señal_2 != null)
            {
                // Recorrido de la colección de muestras de la señal 2
                foreach (Muestra muestra in señal_2.Muestras)
                {
                    plnGrafica_2.Points.Add(new Point((muestra.X - tiempoInicial) * scrContenedor.Width, (muestra.Y / amplitudMaxima * ((scrContenedor.Height / 2) - 30) * -1 + (scrContenedor.Height / 2))));
                }
            }

            // Línea del Eje X
            plnEjeX.Points.Clear();
            plnEjeX.Points.Add(new Point(0, scrContenedor.Height / 2));
            plnEjeX.Points.Add(new Point((tiempoFinal - tiempoInicial) * scrContenedor.Width, scrContenedor.Height / 2));

            // Línea del Eje Y
            plnEjeY.Points.Clear();
            plnEjeY.Points.Add(new Point((-tiempoInicial) * scrContenedor.Width, 0));
            plnEjeY.Points.Add(new Point((-tiempoInicial) * scrContenedor.Width, scrContenedor.Height));
        }


        // Combo Box de la Primera Señal
        private void cb_TipoSeñal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            panelConfiguracion.Children.Clear();
            switch (cb_TipoSeñal.SelectedIndex)
            {
                // Señal Senoidal
                case 0:
                    panelConfiguracion.Children.Add(new ConfiguracionSeñalSenoidal());
                    break;

                // Señal Rampa
                case 1:
                    break;

                // Señal Senoidal
                case 2:
                    panelConfiguracion.Children.Add(new ConfiguracionSeñalExponencial());
                    break;

                // Señal Rectangular
                case 3:
                    break;

                default:
                    break;
            }
        }

        // Combo Box de la Segunda Señal
        private void cb_TipoSeñal_SelectionChanged_2(object sender, SelectionChangedEventArgs e)
        {
            panelConfiguracion_2.Children.Clear();
            switch (cb_TipoSeñal_2.SelectedIndex)
            {
                // Señal Senoidal
                case 0:
                    panelConfiguracion_2.Children.Add(new ConfiguracionSeñalSenoidal());
                    break;

                // Señal Rampa
                case 1:

                    break;

                // Señal Exponencial
                case 2:
                    panelConfiguracion_2.Children.Add(new ConfiguracionSeñalExponencial());
                    break;

                // Señal Rectangular
                case 3:
                    break;

                default:
                    break;
            }
        }

        private void BotonOperacion_Click(object sender, RoutedEventArgs e)
        {
            señalResultado = null;
            switch (cb_TipoOperacion.SelectedIndex)
            {
                // Suma
                case 0:
                    señalResultado = Señal.suma(señal, señal_2);
                    break;

                // Multiplicación
                case 1:
                    señalResultado = Señal.multiplicacion(señal, señal_2);
                    break;

                // Convolución
                case 2:
                    señalResultado = Señal.convolucion(señal, señal_2);
                    break;

                default:
                    break;
            }

            // Actualizar
            señalResultado.actualizarAmplitudMaxima();

            // Limpieza de polylines
            plnGrafica_Resultado.Points.Clear();

            // Impresión de la amplitud máxima en los labels de la ventana.
            lbl_AmplitudMaxima_Resultado.Text = señalResultado.AmplitudMaxima.ToString("F");
            lbl_AmplitudMinima_Resultado.Text = "-" + señalResultado.AmplitudMaxima.ToString("F");

            if (señalResultado != null)
            {
                foreach (Muestra muestra in señalResultado.Muestras)
                {
                    plnGrafica_Resultado.Points.Add(new Point((muestra.X - señalResultado.TiempoInicial) * scrContenedor_Resultado.Width, (muestra.Y / señalResultado.AmplitudMaxima * ((scrContenedor_Resultado.Height / 2) - 30) * -1 + (scrContenedor_Resultado.Height / 2))));
                }
            }

            // Línea del Eje X
            plnEjeX_Resultado.Points.Clear();
            plnEjeX_Resultado.Points.Add(new Point(0, scrContenedor_Resultado.Height / 2));
            plnEjeX_Resultado.Points.Add(new Point((señalResultado.TiempoFinal - señalResultado.TiempoInicial) * scrContenedor_Resultado.Width, scrContenedor_Resultado.Height / 2));

            // Línea del Eje Y
            plnEjeY_Resultado.Points.Clear();
            plnEjeY_Resultado.Points.Add(new Point((-señalResultado.TiempoInicial) * scrContenedor_Resultado.Width, 0));
            plnEjeY_Resultado.Points.Add(new Point((-señalResultado.TiempoInicial) * scrContenedor_Resultado.Width, scrContenedor_Resultado.Height));
        }
    }
}