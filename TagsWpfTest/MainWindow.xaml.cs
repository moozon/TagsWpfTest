using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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
using System.Xml;


namespace TagsWpfTest
{

    class Category
    {
        public string Name { get; set; }
        public List<Traffic> trafficList { get; set; }
    }
    class Traffic
    {
        public string TrafficDescription { get; set; }
        public Traffic(string trafic)
        {
            TrafficDescription = trafic;
        }
    }
    class CategoryCreator
    {
        static public List<Category> GetCreatorList()
        {
            Category cA = new Category();
            cA.Name = "категория A";
            cA.trafficList = new List<Traffic>()
            {
                new Traffic("мотоцикл"),
                new Traffic("мотороллер"),
                new Traffic("другие мототранспортные средства")
            };

            Category cB = new Category();
            cB.Name = "категория B";
            cB.trafficList = new List<Traffic>()
            {
                new Traffic("автомобили не тяжелей 3.5т и меньше 8 мест")
            };

            Category cC = new Category();
            cC.Name = "категория С";
            cC.trafficList = new List<Traffic>()
            {
                new Traffic("автомобили тяжелей 3.5т")
            };

            Category cD = new Category();
            cD.Name = "категория D";
            cD.trafficList = new List<Traffic>()
            {
                new Traffic("автомобили для перевозки пассажиров")
            };

            Category cE = new Category();
            cE.Name = "категория E";
            cE.trafficList = new List<Traffic>()
            {
                new Traffic("составы транспортных средств с тягачом")
            };

            return new List<Category>() { cA, cB, cC, cD, cE };
        }
    }
    class Item : INotifyPropertyChanged
    {
        public Item()
        {
            this.Child = new ObservableCollection<Item>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public ObservableCollection<Item> Child { get; private set; }
    }
    class My
    {
        //private string _name;
        //private XmlNodeType _type;
        private string fileName = "E:/TagsWpfTest.xml";
        TagStorage tagStorage;
        TagItem tagItem;
        XmlNodeList list;
        public ObservableCollection<TagItem> tagItemCollection1;
        public ObservableCollection<XmlNode> tagItemCollection2;

        public My()
        {
            tagStorage = new TagStorage(fileName);
            tagItem = new TagItem(tagStorage.Root);
            //this._name = tagItem.Name;
            //this._type = tagItem.Type;            
            tagItemCollection1 = new ObservableCollection<TagItem>();
            tagItemCollection2 = new ObservableCollection<XmlNode>();
        }

        public ObservableCollection<TagItem> TagItemCollection1
        {
            get
            {
                CreateTagItemCollection1(tagStorage.Root);
                return tagItemCollection1;
                //return tagItem.AllChildCollection;
                //return tagItem.TagItemCollection;
            }

        }
        public ObservableCollection<XmlNode> TagItemCollection2
        {
            get
            {
                CreateTagItemCollection2(tagStorage.Root);
                return tagItemCollection2;
                //return tagItem.AllChildCollection;
                //return tagItem.TagItemCollection;
            }

        }
        public XmlNodeList TagItemCollection3
        {
            get
            {
                list = tagItem.ChildXmlNodes;
                return list;
                //return tagItem.TagItemCollection;
            }

        }
        private void CreateTagItemCollection1(XmlNode element)
        {
            if (element == null)        //Выход, если пустая ссылка
                return;
            tagItemCollection1.Add(new TagItem(element));

            if (element.HasChildNodes)
            {
                foreach (XmlNode e in element.ChildNodes)
                {
                    CreateTagItemCollection1(e);
                }
            }
        }
        private void CreateTagItemCollection2(XmlNode element)
        {
            if (element == null)        //Выход, если пустая ссылка
                return;
            tagItemCollection2.Add(element);

            if (element.HasChildNodes)
            {
                foreach (XmlNode e in element.ChildNodes)
                {
                    CreateTagItemCollection2(e);
                }
            }
        }
    }

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string fileName = "e:/TagsWpfTest.xml";
        public TagStorage tagStorage;
        public TagItem tagItem;
        XmlElement emptyElement;
        XmlNodeList emptyXmlNodeList;
        XmlNodeList xmlNodelist;
        My myClass;
        private ObservableCollection<TagItem> tagCollection;
        //private ObservableCollection<TagItem> childCollectionMainWindow;
        private DependencyObject parentTreeViewItem;
        private TreeViewItem selectedTreeViewItem;
        private ItemsControl virtualTree;
        private bool _isChanged = false;
        private bool isText = false;
        public event PropertyChangedEventHandler PropertyChanged;
        //private Thread threadLoad;
        //private Thread threadLoad;
        Task taskLoad;
        Task taskUpload;

        public MainWindow()
        {
            InitializeComponent();
            //System.Windows.Input.ApplicationCommands.
        }
        private void Window_Loaded(object sender, EventArgs e)
        {
            //tagStorage = new TagStorage(fileName);
            
            //emptyElement = tagStorage.xmlDoc.CreateElement("Empty");
            emptyElement = new XmlDocument().DocumentElement;
            tagItem = new TagItem(emptyElement);
            tagCollection = new ObservableCollection<TagItem>();
            //childCollectionMainWindow = new ObservableCollection<TagItem>();
            
            //tagItem = new TagItem(emptyElement);            
            //xmlNodelist = tagItem.ChildXmlNodes;

            //outTreeView.ItemsSource = TagItemCollection;
            this.Title = "TreeView";
            //CreateTagCollection(tagStorage.Root);
            //myClass = new My();
            //this.DataContext = myClass;
            this.DataContext = this;
            Task.Factory.StartNew(() => outTreeView.ItemsSource = TagCollection);
            //outTreeView.ItemsSource = TagCollection;
            //outTreeView.ItemsSource = CategoryCreator.GetCreatorList();
            //Test();
        }





        //Properties
                

        public ObservableCollection<TagItem> MainCollectionMainWindow
        {
            get
            {
                //CreateTagCollection();
                return tagCollection;
            }
        }

        public ObservableCollection<TagItem> ChildCollectionMainWindow
        {
            get
            {
                return tagItem.ChildCollection;
            }
        }

        public ObservableCollection<TagItem> TagCollection
        {
            get
            {
                //CreateTagCollection();
                return tagCollection;
            }
            set
            {
                tagCollection = value;
            }
        }

        public XmlNodeList XmlNodeCollection
        {
            get
            {
                xmlNodelist = tagItem.ChildXmlNodes;
                return xmlNodelist;
                //return tagItem.TagItemCollection;
            }

        }


        //Methods

        private string CreateXPathName(XmlNode node)
        {
            string stringName = "";
            //XmlNode node = selectedTag.tag;

            while (node.ParentNode != null && node.ParentNode.NodeType != XmlNodeType.Document)
            {
                if (node.NodeType != XmlNodeType.Text)
                {
                    stringName = stringName.Insert(0, "/" + node.Name);
                    node = node.ParentNode;
                }
                else
                {
                    //tagStorage.RemoveTypedTag(xPathName, "");
                    isText = true;
                    //break;
                    stringName = stringName.Insert(0, "/" + node.Name);
                    node = node.ParentNode;
                }
            }
            stringName = stringName.Insert(0, "/");
            return stringName;
        }
        /// <summary>
        /// Запоминаем выделенный узел
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            this.selectedTreeViewItem = (e.OriginalSource as TreeViewItem);
        }

        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Test()
        {

            foreach (TagItem t in ChildCollectionMainWindow)
                MessageBox.Show(t.Name);
            //MessageBox.Show(tagItem.Parent.Name);

        }

        private void CreateTagCollection(XmlNode element)
        {
            tagCollection.Clear();
                if (element != null && element.HasChildNodes)
                {
                    foreach (XmlNode e in element.ChildNodes)
                    {
                    tagCollection.Add(new TagItem(e));
                    }
                }            
        }

        private void loadTree()
        {

        }
        private void UnloadTree()
        {

        }
        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isChanged)
            {
                MessageBoxResult result = MessageBox.Show("Структура дерева изменена. Сохранить дерево в Xml файл?", "Внимание!!!", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
                    sfd.FileName = "Document"; 
                    sfd.DefaultExt = ".xml"; 
                    sfd.Filter = "XML documents (.xml)|*.xml";

                    // Show open file dialog box
                    Nullable<bool> resultSfd = sfd.ShowDialog();
                    if (resultSfd == true)
                    {
                        this.tagStorage.fileName = sfd.FileName;
                        this.tagStorage.SaveXmlDocument();
                    }
                }
                
            }

            Application.Current.Shutdown();
        }

        private void selectFileButton_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.FileName = "Document"; // Default file _name
            ofd.DefaultExt = ".xml"; // Default file extension
            ofd.Filter = "XML documents (.xml)|*.xml"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = ofd.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document and show fileName
                fileName = ofd.FileName;
                selectedFileLabel.Content = "Selected XML file: " + fileName;
            }
        }

        private void loadTagTreeButton_Click(object sender, RoutedEventArgs e)
        {
            tagStorage = new TagStorage(fileName);
            tagItem = new TagItem(tagStorage.Root);
            CreateTagCollection(tagStorage.Root);

            Task.Factory.StartNew(() => 
            { 
            

            outTreeView.Dispatcher.Invoke(() => outTreeView.ItemsSource = TagCollection);
            //XmlNodeList xmlList = tagItem.GetChilds();
            //this.outTextBlock.DataContext = xmlList;
            //this.outTextBox.DataContext = xmlList;
            });


            //taskLoad = new Task(() =>
            //{
            //    tagStorage = new TagStorage(fileName);
            //    tagItem = new TagItem(tagStorage.Root);
            //    CreateTagCollection(tagStorage.Root);

            //    outTreeView.Dispatcher.Invoke(() => outTreeView.ItemsSource = TagCollection);
            //    //XmlNodeList xmlList = tagItem.GetChilds();
            //    //this.outTextBlock.DataContext = xmlList;
            //    //this.outTextBox.DataContext = xmlList;
            //});
            //taskLoad.Start();
                
                


            //thread = new Thread(new ThreadStart(() =>
            //{
            //    tagStorage = new TagStorage(fileName);
            //    tagItem = new TagItem(tagStorage.Root);
            //    CreateTagCollection(tagStorage.Root);
                
            //    outTreeView.Dispatcher.Invoke(() => outTreeView.ItemsSource = TagCollection);
            //    //XmlNodeList xmlList = tagItem.GetChilds();
            //    //this.outTextBlock.DataContext = xmlList;
            //    //this.outTextBox.DataContext = xmlList;
            //}));
            //thread.SetApartmentState(ApartmentState.STA);
            //thread.Start();
            loadTagTreeButton.IsEnabled = false;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var newTagWindow = new NewTagWindow();
            newTagWindow.ShowDialog();

            string newName = newTagWindow.NewName;
            string newType = newTagWindow.NewType;

            if (string.IsNullOrWhiteSpace(newName))
            {
                MessageBox.Show("Введите имя");
                return;
            }

            bool isValidType = false;

            foreach(string type in Enum.GetNames(typeof(XmlNodeType)))
            {
                if (newType == type)
                    isValidType = true;
            }

            if (!isValidType)
            {
                MessageBox.Show("Неверно указан тип. Повторите попытку", "Внимание!!!");
                return;
            }
            else
            {
                TagItem selectedTag = (TagItem)outTreeView.SelectedItem;

                if (tagCollection.Contains(selectedTag))
                {
                    foreach (TagItem tag in this.tagCollection)
                    {
                        if (tag == selectedTag)
                        {
                            selectedTag.ChildCollection.Add(new TagItem(newName, newType));
                            //TagItem selectedTreeViewItemParent2 = (TagItem)ItemsControl.ItemsControlFromItemContainer(selectedTreeViewItem).DataContext;
                            //selectedTreeViewItemParent2.GetChildByName(selectedTag.Name).AppendChild(tagStorage.xmlDoc.CreateElement(newName));
                        }
                    }
                    _isChanged = true;
                }
                else
                {
                    selectedTag.ChildCollection.Add(new TagItem(newName, newType));

                    TagItem selectedTreeViewItemParent2 = (TagItem)ItemsControl.ItemsControlFromItemContainer(selectedTreeViewItem).DataContext;
                    selectedTreeViewItemParent2.GetChildByName(selectedTag.Name).AppendChild(tagStorage.xmlDoc.CreateElement(newName));
                    //selectedTag.tag.AppendChild(tagStorage.xmlDoc.CreateElement(newName));
                    _isChanged = true;
                }
            }
            
        }   //Add item

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            var renameWindow = new RenameWindow();
            renameWindow.ShowDialog();

            string newName = renameWindow.NewName;

            if (string.IsNullOrWhiteSpace(newName))
            {
                MessageBox.Show("Введите имя");
                return;
            }

            TagItem selectedTag = (TagItem)outTreeView.SelectedItem;
            if (this.TagCollection.Contains(selectedTag))
            {
                foreach (TagItem tag in TagCollection)
                {
                    if (tag == selectedTag)
                    {
                        tag.Name = newName;
                        _isChanged = true;
                        string xPathName = CreateXPathName(selectedTag.tag);
                        //xPathName = xPathName.TrimEnd
                        if (!isText)
                            tagStorage.RenameTag(xPathName, newName);
                        else
                        {
                            //xPathName = xPathName.TrimEnd(@"#text".ToCharArray());
                            tagStorage.RenameTag(xPathName, newName);
                        }
                        return;
                    }
                }
            }
            else
            {
                TagItem selectedTreeViewItemParent = (TagItem)ItemsControl.ItemsControlFromItemContainer(selectedTreeViewItem).DataContext;
                foreach (TagItem tag in selectedTreeViewItemParent.ChildCollection)
                {
                    if (tag == selectedTag)
                    {
                        tag.Name = newName;
                        _isChanged = true;
                        string xPathName = CreateXPathName(selectedTag.tag);
                        //xPathName = xPathName.TrimEnd
                        if (!isText)
                            tagStorage.RenameTag(xPathName, newName);
                        else
                        {
                            //xPathName = xPathName.TrimEnd(@"#text".ToCharArray());
                            tagStorage.RenameTag(xPathName, newName);
                        }
                        return;
                    }

                }
                //string xPathName = CreateXPathName(selectedTag.tag);
                ////xPathName = xPathName.TrimEnd
                //if (!isText)
                //    tagStorage.RenameTag(xPathName, newName);
                //else
                //{
                //    //xPathName = xPathName.TrimEnd(@"#text".ToCharArray());
                //    tagStorage.RenameTag(xPathName, newName);
                //}

            }

            _isChanged = true;

            //reeViewItem selectedTreeViewItem = (TreeViewItem)outTreeView.SelectedItem;
            //TreeViewItem selectedTreeViewItem = (TreeViewItem)(e.OriginalSource as MenuItem).ItemsSource;
            //TreeViewItem selectedTreeViewItem = (MenuItem)e.OriginalSource;
            //selectedTreeViewItem.Header = "123";
            //(e.OriginalSource is TreeViewItem).Name = "123";
            //MessageBox.Show("Rename");
        }   //Rename item

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {       
            
            
            MessageBoxResult result = MessageBox.Show("Удалить выбранную ветвь?", "Внимание!!!", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {

                TagItem selectedTag = (TagItem)outTreeView.SelectedItem;
                //Выбранный тег принадлежит родительской колекции
                if (tagCollection.Contains(selectedTag))
                {
                    this.TagCollection.Remove(selectedTag);
                    //string xPathName = "";
                    //XmlNode currentTag = selectedTag.tag;
                    //while (currentTag.ParentNode != null && currentTag.ParentNode.NodeType != XmlNodeType.Document)
                    //{
                    //    if (currentTag.NodeType != XmlNodeType.Text)
                    //    {
                    //        xPathName = xPathName.Insert(0, "/" + currentTag.Name);
                    //        currentTag = currentTag.ParentNode;
                    //    }
                    //    else
                    //    {
                    //        //tagStorage.RemoveTypedTag(xPathName, "");
                    //        isText = true;
                    //        //break;
                    //        xPathName = xPathName.Insert(0, "/" + currentTag.Name);
                    //        currentTag = currentTag.ParentNode;
                    //    }
                    //}
                    string xPathName = CreateXPathName(selectedTag.tag);
                    //xPathName = xPathName.Insert(0, "/");
                    if (!isText)
                        tagStorage.RemoveTag(xPathName);
                    else
                    {
                        xPathName = xPathName.TrimEnd(@"#text".ToCharArray());
                        tagStorage.RemoveTypedTag(xPathName, "");
                    }
                    if (isText)
                        isText = false;
                    _isChanged = true;
                }
                //
                else
                {
                    if (selectedTag.isNewTag)
                    {
                        tagStorage.RemoveNewTag(selectedTag.Name);
                        TagItem selectedTreeViewItemParent2 = (TagItem)ItemsControl.ItemsControlFromItemContainer(selectedTreeViewItem).DataContext;
                        //tagItem.RemoveChildTag(selectedTag.Name);
                        //tagStorage.RemoveTag(selectedTag.Name);
                        selectedTreeViewItemParent2.ChildCollection.Remove(selectedTag);
                        return;
                    }
                    //string xPathName = "";
                    //XmlNode node = selectedTag.tag;

                    //while (node.ParentNode != null && node.ParentNode.NodeType != XmlNodeType.Document)
                    //{
                    //    if (node.NodeType != XmlNodeType.Text)
                    //    {
                    //        xPathName = xPathName.Insert(0, "/" + node.Name);
                    //        node = node.ParentNode;
                    //    }
                    //    else
                    //    {
                    //        //tagStorage.RemoveTypedTag(xPathName, "");
                    //        isText = true;
                    //        //break;
                    //        xPathName = xPathName.Insert(0, "/" + node.Name);
                    //        node = node.ParentNode;
                    //    }
                    //}
                    //xPathName = xPathName.Insert(0, "/");
                    string xPathName = CreateXPathName(selectedTag.tag);
                    if (!isText)
                        tagStorage.RemoveTag(xPathName);
                    else
                    {
                        xPathName = xPathName.TrimEnd(@"#text".ToCharArray());
                        tagStorage.RemoveTypedTag(xPathName, "");
                    }
                    if (isText)
                        isText = false;
                    //tagItem.RemoveChildTag(selectedTag.Name);
                    //virtualTree = ItemsControl.ItemsControlFromItemContainer(selectedTreeViewItem);
                    TagItem selectedTreeViewItemParent = (TagItem)ItemsControl.ItemsControlFromItemContainer(selectedTreeViewItem).DataContext;
                    //tagItem.RemoveChildTag(selectedTag.Name);
                    //tagStorage.RemoveTag(selectedTag.Name);
                    selectedTreeViewItemParent.ChildCollection.Remove(selectedTag);
                    _isChanged = true;
                }
            }
        }   //Remove

        private void uploadTagTreeButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Выгрузить дерево и сохранить изменения в Xml файле?", "Внимание!!!", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                this.tagStorage.SaveXmlDocument();
                Task.Factory.StartNew(() =>
                {                    
                    this.outTreeView.Dispatcher.Invoke(() => outTreeView.ItemsSource = null);
                });
                
                //taskLoad = null;
                loadTagTreeButton.IsEnabled = true;
            }

            //tagStorage.Root.RemoveAll();
            //tagStorage.Root.AppendChild(tagItem.)




            //foreach (TagItem tag in outTreeView.ItemsSource)
            //tagStorage.Root.AppendChild(tag.tag);
            //this.outTreeView.ItemsSource = null;

            //foreach (TagItem tag in TagCollection)
            // tagStorage.Root.AppendChild(tag.tag);

        }

        private void outTreeView_Expanded(object sender, RoutedEventArgs e)
        {
            
            //TreeViewItem item = (TreeViewItem)e.OriginalSource;
            //TagItem selectedTag = (TagItem)outTreeView.SelectedItem;
            

            ////item.Items.Clear();
            ////if (selectedTag is TagItem)
            ////{
            ////TagItem selectedTag = (TagItem)item.Tag;

            //foreach (TagItem t in selectedTag.ChildCollection)
            //{
            //    TreeViewItem newItem = new TreeViewItem();
            //    newItem.Tag = t;
            //    newItem.Header = t.Name;
            //    newItem.Items.Add("*");
            //    item.Items.Add(newItem);
            //}
        }

        private void TextBoxIn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("");
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            
                Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
                sfd.FileName = "Document";
                sfd.DefaultExt = ".xml";
                sfd.Filter = "XML documents (.xml)|*.xml";

                
                Nullable<bool> resultSfd = sfd.ShowDialog();
                if (resultSfd == true)
                {
                string oldFileName = this.tagStorage.fileName;
                this.tagStorage.fileName = sfd.FileName;
                this.tagStorage.SaveXmlDocument();
                this.tagStorage.fileName = oldFileName;
                }
            _isChanged = false;

        }
    }
}
