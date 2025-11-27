using Front.INICIO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Front
{
    /// <summary>
    /// Lógica de interacción para Servicios.xaml
    /// </summary>
    public partial class Servicios : Page
    {
        public Servicios()
        {
            InitializeComponent();
        }
        private void Tarjeta_MouseEnter(object sender, MouseEventArgs e)
        {
            ResetBolitas();

            if (sender == Tarjeta1)
                Bolita1.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0D1B40"));
            else if (sender == Tarjeta2)
                Bolita2.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0D1B40"));
            else if (sender == Tarjeta3)
                Bolita3.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0D1B40"));
        }

        private void Tarjeta_MouseLeave(object sender, MouseEventArgs e)
        {
            ResetBolitas();
        }

        private void ResetBolitas()
        {
            Bolita1.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9AA6C2"));
            Bolita2.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9AA6C2"));
            Bolita3.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9AA6C2"));
        }
        private void Tarjeta1_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new CentrosMedicos());
        }
        private void Tarjeta2_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new SignosVitales());
        }
        private void Tarjeta3_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new Medicamentos());
        }

        private void BtnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new Inicio());
        }

        private void BtnMiCuenta_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new MiCuenta.MiCuenta());
        }
    }
}
