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

    class ItemArvore
    {
        public TreeViewItem pai { get; set; }
        public TreeViewItem item { get; set; }
    }
    public class Conteudo
    {
        public string ID { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public string Comments { get; set; }
    }
    /*MeuItem - Contem uma lista de todos conteudos (linha na datagrid) de um imtem da árvore,
     * dessa maneira qualquer alteração é salvada na memória
     * 
     */
    class MeuItem
    {
        public MeuItem()
        {
            conteudo_mostar = new List<Conteudo>();
        }
        public List<Conteudo> conteudo_mostar;
    }
    /* Aba - Contem todas as informaçoes da aba
     *
     * 
     * */
    class Aba
    {
        public Aba(string file_path)
        {
            this.file_path = file_path;
            all_items = new Dictionary<string, MeuItem>();
            all_items_tree = new List<ItemArvore>();
            file_lines = File.ReadAllLines(file_path).ToList();
        }
        //Todas as linha do arquivo
        public List<string> file_lines;
        //Todos os elementos da árvore
        public List<ItemArvore> all_items_tree;
        //Todos os items de cada item da árvore
        public Dictionary<string,MeuItem> all_items;
        public string file_path;
    }
    public partial class MainWindow : Window
    {
        //Aba selecionada
        Dictionary<string, Aba> todas_abas;
        Aba aba_selecionada;
        bool content_hiden = false;
        //Nome do item da arvore selecionado
        string item_selecionado = "";

        public MainWindow()
        {
            todas_abas = new Dictionary<string, Aba>();
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
        private TreeViewItem EncontrarItem(string header)
        {
            foreach(ItemArvore item in aba_selecionada.all_items_tree)
            {
                if(item.item.Header.ToString() == header)
                {
                    return item.item;
                }
            }
            return null;
        }
        //Carrega a tree do arquivo selecionado
        private void Carregar_Tree()
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
                                //filetree_main.Items.Add(newChild);
                                ItemArvore newItem = new ItemArvore();
                                newItem.item = newChild;
                                newItem.pai = null;
                                aba_selecionada.all_items_tree.Add(newItem);
                            }
                            //Caso contrário encontrar pai na lista de todos os item e inserir este item como filho do pai
                            else
                            {
                                if( (parent =EncontrarItem(tokens[2])) != null)
                                {
                                    newChild = new TreeViewItem();
                                    newChild.Header = tokens[1];
                                    newChild.Selected += treeItem_Selected;
                                    //parent.Items.Add(newChild);
                                    ItemArvore newItem = new ItemArvore();
                                    newItem.pai = parent;
                                    newItem.item = newChild;
                                    aba_selecionada.all_items_tree.Add(newItem);
                                }
                            }
                            
                        }
                    }
                }  
            }
        }
        //Carrega o conteúdo selecionado na tree
        private void Carregar_Conteudo()
        {
            //aba_selecionada.conteudo_mostar.Clear();
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
                            if(tokens[1] == item_selecionado)
                            {   
                                
                                if(!aba_selecionada.all_items.ContainsKey(item_selecionado))
                                {
                                    aba_selecionada.all_items.Add(item_selecionado, new MeuItem());
                                    isItem_find = true;
                                }
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
                            //Adiciona conteudo, ou seja, uma linha do DataGrid, na respectiva aba e item da árvore
                            aba_selecionada.all_items[item_selecionado].conteudo_mostar.Add(newContent);


                        }
                    }
                }
            }
            Mostrar_conteudo();

        }

        private void Mostrar_tree()
        {
            foreach(ItemArvore it_tree in aba_selecionada.all_items_tree)
            {
                if(it_tree.pai == null)
                {
                    filetree_main.Items.Add(it_tree.item);
                }
                else
                {
                    //É apenas necessário setar o pai apenas uma vez, as vezes subsequente é desnecessário, e levantara uma exessao
                    if (it_tree.item.Parent == null)
                    {
                        it_tree.pai.Items.Add(it_tree.item);
                    }
                    
                }
            }
        }

        private void Mostrar_conteudo()
        {
            ctntGrid.Items.Clear();
            //Assegura que irá imprimir o conteudo de um elemento da árvore que existe
            if (aba_selecionada.all_items.ContainsKey(item_selecionado))
            {
                foreach (Conteudo cont in aba_selecionada.all_items[item_selecionado].conteudo_mostar)
                {
                    ctntGrid.Items.Add(cont);
                }
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
                if (!todas_abas.ContainsKey(openDlg.FileName))
                {
                    Aba umaAba = new Aba(openDlg.FileName);
                    aba_selecionada = umaAba;
                    todas_abas.Add(openDlg.FileName, umaAba);
                    MenuItem newItem = new MenuItem();
                    newItem.Header = openDlg.FileName;
                    newItem.Click += openTab_Click;
                    openFiles.Items.Add(newItem);
                    //Limpa tree
                    filetree_main.Items.Clear();
                    Carregar_Tree();
                    //Limpa contúdo
                    ctntGrid.Items.Clear();
                    Mostrar_tree();
                }
            }
        }

        private ItemArvore EncontrarItemArvore(TreeViewItem tree_item)
        {
            foreach(ItemArvore item in aba_selecionada.all_items_tree)
            {
                if(item.item == tree_item)
                {
                    return item;
                }
            }
            return null;
        }

        private void WriteFile(string file_path)
        {
            List<ItemArvore> all_items_tree = aba_selecionada.all_items_tree;
            string line = "";
            List<string> all_lines = new List<string>();
            string aux;
            TreeViewItem m_item;
            Title = all_items_tree.Count().ToString();
            foreach(ItemArvore item_ar in all_items_tree)
            {
                aux = " main";
                if(item_ar.pai != null)
                {
                    aux = " " + item_ar.pai.Header.ToString();
                }
                line = "# " + item_ar.item.Header.ToString() + aux;
                all_lines.Add(line);
                item_selecionado = item_ar.item.Header.ToString();
                Carregar_Conteudo();
                foreach (Conteudo cont in aba_selecionada.all_items[item_selecionado].conteudo_mostar)
                {
                    line = cont.ID + ", " + cont.Description + ", " + cont.Value + ", " + cont.Comments;
                    all_lines.Add(line);
                }

            }

            File.AppendAllLines(file_path, all_lines);
        }
        private void menuSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog svDlg = new SaveFileDialog();
            svDlg.AddExtension = true;
            svDlg.Filter = "Arquivo Zylix (*.zylix)|*.zylix";
            if(svDlg.ShowDialog() == true)
            {
                WriteFile(svDlg.FileName);
            }
        }

        private void treeItem_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)e.OriginalSource;
            item_selecionado = item.Header.ToString();
            Carregar_Conteudo();
        }

        private void MenuEditar_Click(object sender, RoutedEventArgs e)
        {
            if (ctntGrid.SelectedCells.Count > 0) { 
                Conteudo ctd = (Conteudo)ctntGrid.SelectedCells[0].Item;
                EditWindow eW = new EditWindow(ref ctd);
                //Iniciar a tela de edição e "pausar" a tela principal
                eW.ShowDialog();
                //Chamar esta funçao de volta para autualizar os valores
                Mostrar_conteudo();
            }
        }

        private void openTab_Click(object sender, RoutedEventArgs e)
        {
            MenuItem m_item = (MenuItem)e.OriginalSource;
            if (todas_abas.ContainsKey(m_item.Header.ToString()))
            {
                aba_selecionada = todas_abas[m_item.Header.ToString()];
                filetree_main.Items.Clear();
                Mostrar_tree();
                Mostrar_conteudo();
            }
        }
    }
}
