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

        public void CheckChilds()
        {
            if (tagItem != null && tagItem.Type == XmlNodeType.Element)
            {

            }
        }

        public ObservableCollection<TagItem> TagItemCollection1
        {
            get
            {
                CreateTagItemCollection1(tagStorage.Root);
                return tagItemCollection1;
                //return tagItem.AllChilds;
                //return tagItem.TagItemCollection;
            }

        }
        public ObservableCollection<XmlNode> TagItemCollection2
        {
            get
            {
                CreateTagItemCollection2(tagStorage.Root);
                return tagItemCollection2;
                //return tagItem.AllChilds;
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
        private ObservableCollection<TagItem> childCollectionMainWindow;
        private DependencyObject parentTreeViewItem;
        private TreeViewItem selectedTreeViewItem;

        public event PropertyChangedEventHandler PropertyChanged;



        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, EventArgs e)
        {
            tagStorage = new TagStorage(fileName);
            tagItem = new TagItem(tagStorage.Root);
            //emptyElement = tagStorage.xmlDoc.CreateElement("Empty");
            emptyElement = new XmlDocument().DocumentElement;
            tagCollection = new ObservableCollection<TagItem>();
            childCollectionMainWindow = new ObservableCollection<TagItem>();
            //Task.Factory.StartNew(() => tagItem = new TagItem(emptyElement));
            //tagItem = new TagItem(emptyElement);            
            //xmlNodelist = tagItem.ChildXmlNodes;

            //outTreeView.ItemsSource = TagItemCollection;
            this.Title = "TreeView";
            CreateTagCollection(tagStorage.Root);
            //myClass = new My();
            //this.DataContext = myClass;
            this.DataContext = this;
            outTreeView.ItemsSource = TagCollection;
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
                return tagItem.Childs;
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

        private void TreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            //TagItem selectedTag = (TagItem)outTreeView.SelectedItem;
            this.selectedTreeViewItem = (e.OriginalSource as TreeViewItem);
            //VisualTreeHelper.

            //parentTreeViewItem = (outTreeView.FindName(selectedTag.Parent.Name) as TreeViewItem);
            //TreeViewItem tvi = (TreeViewItem)(e.OriginalSource as TreeViewItem).Parent;
            //this.parent = tvi.Parent;
            // set the last tree view item selected variable which may be used elsewhere as there is no other way I have found to obtain the TreeViewItem container (may be null)
             
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
                if (element != null && element.HasChildNodes)
                {
                    foreach (XmlNode e in element.ChildNodes)
                    {
                    tagCollection.Add(new TagItem(e));
                    }
                }            
        }

        public void CheckChilds()
        {
            if (tagItem != null && tagItem.Type == XmlNodeType.Element)
            {
                //if (tagItem.HasChilds)
                //outTreeView.ItemTemplate.
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
            //Task.Factory.StartNew(new Action(() =>
            //{
            //    TagStorage tagStorage = new TagStorage(fileName);
            //    TagItem tagItem = new TagItem(tagStorage.Root);
            //    ItemsControl ic = new ItemsControl();
            //    //ic.ItemsSource = tagItem.GetChilds();
            //    outDataGrid.ItemsSource = tagItem.GetChilds();
            //    //XmlNodeList xmlList = tagItem.GetChilds();
            //    //this.outTextBlock.DataContext = xmlList;
            //    //this.outTextBox.DataContext = xmlList;
            //}));

            tagStorage = new TagStorage(fileName);
            tagItem = new TagItem(tagStorage.Root);
            outTreeView.ItemsSource = tagItem.AllChilds;


            //tagItem = await Task.Factory.StartNew(() => new TagItem(tagStorage.Root));
            //outTreeView.ItemsSource = tagItem.GetChilds();


            //Thread th = new Thread(new ThreadStart(() =>
            //{

            //    //ic.ItemsSource = tagItem.GetChilds();
            //    outTreeView.Dispatcher.Invoke(() => outTreeView.ItemsSource = tagItem.GetChilds());
            //    //XmlNodeList xmlList = tagItem.GetChilds();
            //    //this.outTextBlock.DataContext = xmlList;
            //    //this.outTextBox.DataContext = xmlList;
            //}));
            //th.SetApartmentState(ApartmentState.STA);
            //th.Start();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            TagItem selectedTag = (TagItem)outTreeView.SelectedItem;

            if (tagCollection.Contains(selectedTag))
            {
                foreach (TagItem tag in tagCollection)
                {
                    if (tag == selectedTag)
                        tag.Childs.Add(new TagItem(tagStorage.Root));
                }
            }
            else
            {
                TagItem parent = selectedTag.Parent;
                parent.Childs.Add(new TagItem(tagStorage.Root));
                parent.NotifyPropertyChanged();
            }

            //foreach (XmlNode el in )
            //MessageBox.Show(outTreeView.SelectedItem.ToString(), selectedTag.Name);
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            
            TagItem selectedTag = (TagItem)outTreeView.SelectedItem;
            TreeViewItem selectedTreeViewItem = (TreeViewItem)outTreeView.SelectedItem;
            //TreeViewItem selectedTreeViewItem = (TreeViewItem)(e.OriginalSource as MenuItem).ItemsSource;
            //TreeViewItem selectedTreeViewItem = (MenuItem)e.OriginalSource;
            selectedTreeViewItem.Header = "123";
            //(e.OriginalSource is TreeViewItem).Name = "123";
            //MessageBox.Show("Rename");
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            //TreeViewItem selectedTVI = outTreeView.SelectedItem as TreeViewItem;
            //TreeViewItem item = (TreeViewItem)(outTreeView.ItemContainerGenerator.ContainerFromItem(outTreeView.SelectedItem));

            DependencyObject o = selectedTreeViewItem;

            TreeViewItem selectedItem = (TreeViewItem)outTreeView.SelectedItem; 
            TagItem selectedTag = (TagItem)outTreeView.SelectedItem;

            if (tagCollection.Contains(selectedTag))
            {
                this.TagCollection.Remove(selectedTag);
            }
            else
            {

                TagItem parent = selectedTag.Parent;
                parent.RemoveChildTag(selectedTag.Name);

                //MessageBox.Show(parent.childCollection.Remove(selectedTag).ToString());
                //parent.Childs.Remove(selectedTag);

                parent.CreateChildCollection(parent.tag);
                NotifyPropertyChanged();
                //parent.NotifyPropertyChanged();
                //CreateTagCollection(tagStorage.Root);
                //parent.childCollection.RemoveAt(0);
                //MessageBox.Show(parent.childCollection.RemoveAt(0));
                //MessageBox.Show(selectedTag.Parent.childCollection.Remove(selectedTag).ToString());
                //outTreeView.UpdateDefaultStyle();
                //outTreeView.
                //out
                //outTreeView.ItemsSource = TagCollection;
                //MessageBox.Show("");
                //TagItem selectedTag = (TagItem)outTreeView.SelectedItem;
                //tagCollection.Remove((TagItem)outTreeView.SelectedItem);

                //myClass.tagItemCollection2.Remove((XmlNode)outTreeView.SelectedItem);
            }
        }

        private void unloadTagTreeButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(tagCollection.Count.ToString());
        }

        private void outTreeView_Expanded(object sender, RoutedEventArgs e)
        {
            
            //TreeViewItem item = (TreeViewItem)e.OriginalSource;
            //TagItem selectedTag = (TagItem)outTreeView.SelectedItem;
            

            ////item.Items.Clear();
            ////if (selectedTag is TagItem)
            ////{
            ////TagItem selectedTag = (TagItem)item.Tag;

            //foreach (TagItem t in selectedTag.Childs)
            //{
            //    TreeViewItem newItem = new TreeViewItem();
            //    newItem.Tag = t;
            //    newItem.Header = t.Name;
            //    newItem.Items.Add("*");
            //    item.Items.Add(newItem);
            //}
        }
    
    }
}
