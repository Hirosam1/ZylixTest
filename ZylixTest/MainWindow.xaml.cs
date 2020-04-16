using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

//Classe Aba, que contem informaçao do arquivo


namespace ZylixTest
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    /// 
    public class Conteudo
    {
        public string ID { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public string Comments { get; set; }
    }

    class Aba
    {
        public Aba(string file_path)
        {
            this.file_path = file_path;
            all_items = new List<TreeViewItem>();
            conteudo_mostar = new List<Conteudo>();
            file_lines = File.ReadAllLines(file_path).ToList();
        }
        public List<Conteudo> conteudo_mostar;
        public List<string> file_lines;
        public List<TreeViewItem> all_items;
        public string file_path;
    }
    public partial class MainWindow : Window
    {

        Aba aba_selecionada;
        bool content_hiden = false;

        public MainWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
        private TreeViewItem EncontrarItem(string header)
        {
            foreach(TreeViewItem item in aba_selecionada.all_items)
            {
                if(item.Header.ToString() == header)
                {
                    return item;
                }
            }
            return null;
        }
        //Carrega a tree do arquivo selecionado
        private void CarregarTela_Tree()
        {
            List<string> lines = aba_selecionada.file_lines;
            TreeViewItem newChild;
            TreeViewItem parent;
            string[] tokens;
            foreach (string line in lines)
            {
                if (line.Length > 0)
                {
                    //Caso encontre um item novo
                    if (line[0] == '#')
                    {
                        tokens = line.Split(' ');
                        if(tokens.Length > 2)
                        {
                            //Caso o pai desse item seje main, inserir o item na raiz da arvore
                            if (tokens[2] == "main")
                            {
                                newChild = new TreeViewItem();
                                newChild.Header = tokens[1];
                                newChild.Selected += treeItem_Selected;
                                filetree_main.Items.Add(newChild);
                                aba_selecionada.all_items.Add(newChild);
                            }
                            //Caso contrário encontrar pai na lista de todos os item e inserir este item como filho do pai
                            else
                            {
                                if( (parent =EncontrarItem(tokens[2])) != null)
                                {
                                    newChild = new TreeViewItem();
                                    newChild.Header = tokens[1];
                                    newChild.Selected += treeItem_Selected;
                                    parent.Items.Add(newChild);
                                    aba_selecionada.all_items.Add(newChild);
                                }
                            }
                            
                        }
                    }
                }  
            }
        }
        //Carrega o conteúdo selecionado na tree
        private void Carregar_Conteudo(string target)
        {
            aba_selecionada.conteudo_mostar.Clear();
            bool isItem_find = false;
            string[] tokens;
            foreach (string line in aba_selecionada.file_lines)
            {
                if (line.Length > 0)
                {
                    if (line[0] == '#')
                    {
                        if (isItem_find)
                        {
                            Mostrar_conteudo();
                            return;
                        }
                        tokens = line.Split(' ');
                        if (tokens.Length > 2)
                        {
                            if(tokens[1] == target)
                            {
                                isItem_find = true;
                            }
                        }
                    }else if (isItem_find)
                    {
                        tokens = line.Split(',');
                        if(tokens.Length > 3)
                        {
                            RowDefinition row = new RowDefinition();
                            row.Height = new GridLength(1, GridUnitType.Star);
                            Conteudo newContent = new Conteudo();
                            newContent.ID = tokens[0];
                            newContent.Description = tokens[1];
                            newContent.Value = tokens[2];
                            newContent.Comments = tokens[3];
                            //ctntGrid.Items.Add(newContent);
                            aba_selecionada.conteudo_mostar.Add(newContent);

                        }
                    }
                }
            }
            Mostrar_conteudo();

        }

        private void Mostrar_conteudo()
        {
            //Isto deveria autualizar o data grid
            ctntGrid.Items.Clear();
            foreach (Conteudo cont in aba_selecionada.conteudo_mostar)
            {
                ctntGrid.Items.Add(cont);
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {   
            //Caso o tamnho da tela seja maior que 200 px...
            if (e.NewSize.Width >= 200){
                //Caso a coluna de conteúdo esteje escondida, mostrar novamente ela
                if (content_hiden){
                    content_hiden = false;
                    //Mostra o conteúdo dentro do Scroll
                    ctntScroll.Visibility = Visibility.Visible;
                    ctntColum.Width = new GridLength(4,GridUnitType.Star);
                    //treeColum.Width = GridLength.Auto;
                }
                //Caso a tela diminua a 300px de largura, desativar o scroll bar da coluna de conteúdo
                if (e.NewSize.Width <= 300){
                    ctntScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                }
                else{
                    ctntScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                }
            }
            //Caso a largura seja menor de 200 px, esconder a coluna 2 (coluna de conteúdo)
            else{
                content_hiden = true;
                //Esconde o conteúdo dentro do scoll
                ctntScroll.Visibility = Visibility.Hidden;
                ctntColum.Width = new GridLength(0);
                //Seta a coluna da árcore como *
                treeColum.Width = new GridLength(1, GridUnitType.Star);
            }
            
        }

        private void menuLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Filter = "Arquivo Zylix (*.zylix)|*.zylix";
            if(openDlg.ShowDialog() == true){
                Aba umaAba = new Aba(openDlg.FileName);
                aba_selecionada = umaAba;
                CarregarTela_Tree();
            }
        }


        private void menuSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog svDlg = new SaveFileDialog();
            svDlg.AddExtension = true;
            svDlg.Filter = "Arquivo Zylix (*.zylix)|*.zylix";
            Conteudo ctd = (Conteudo) ctntGrid.SelectedCells[0].Item;
            Title = ctd.ID.ToString();
            /*if(svDlg.ShowDialog() == true)
            {
                File.WriteAllLines(svDlg.FileName, aba_selecionada.file_lines);
            }*/
        }

        private void treeItem_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)e.OriginalSource;
            string target = item.Header.ToString();
            Carregar_Conteudo(target);
        }

        private void MenuEditar_Click(object sender, RoutedEventArgs e)
        {
            if (ctntGrid.SelectedCells.Count > 0) { 
                Conteudo ctd = (Conteudo)ctntGrid.SelectedCells[0].Item;
                EditWindow eW = new EditWindow(ref ctd);
                eW.ShowDialog();
                //Chamar esta funçao de volta para autualizar os valores
                Mostrar_conteudo();
            }
        }
    }
}
