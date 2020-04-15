﻿using System;
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

namespace ZylixTest
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        bool content_hiden = false;
        public MainWindow()
        {
            InitializeComponent();

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {   
            //Caso o tamnho da tela seja maior que 200 px...
            if (e.NewSize.Width >= 200){
                //Caso a coluna de conteúdo esteje escondida, mostrar novamente ela
                if (content_hiden){
                    content_hiden = false;
                    ctntScroll.Visibility = Visibility.Visible;
                    ctntColum.Width = new GridLength(1,GridUnitType.Star);
                }
                //Caso a tela diminua a 300px de largura, desativar o scroll bar da coluna de conteúdo
                if (e.NewSize.Width <= 300){
                    ctntScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    //ctntScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                }
                else{
                    ctntScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                    //ctntScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                }
            }
            //Caso a largura seja menor de 200 px, esconder a coluna 2 (coluna de conteúdo)
            else{
                content_hiden = true;
                ctntScroll.Visibility = Visibility.Collapsed;
                ctntColum.Width = GridLength.Auto;
            }
            
        }
    }
}
