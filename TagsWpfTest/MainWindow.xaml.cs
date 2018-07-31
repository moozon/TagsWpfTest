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
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string fileName = "";
        private ObservableCollection<TagItem> _tagCollection;
        private XmlElement _emptyElement;
        private TreeViewItem _selectedTreeViewItem;
        private bool _isChanged = false;
        private bool _isText = false;
        private bool _isOpenXmlDocument = false;
        private bool _isClosedWindow = false; 
        public TagStorage tagStorage;
        public TagItem tagItem;
        
        public MainWindow()
        {
            InitializeComponent();
        }


        //Properties

        public ObservableCollection<TagItem> TagCollection
        {
            get
            {
                return _tagCollection;
            }
            set
            {
                _tagCollection = value;
            }
        }



        //Methods

        private void Window_Loaded(object sender, EventArgs e)
        {
            _emptyElement = new XmlDocument().DocumentElement;
            tagItem = new TagItem(_emptyElement);
            _tagCollection = new ObservableCollection<TagItem>();
            this.Title = "TreeView";
            this.DataContext = this;
            Task.Factory.StartNew(() => outTreeView.ItemsSource = TagCollection);
        }

        private string CreateXPathName(XmlNode node)
        {
            string stringName = "";
            while (node.ParentNode != null && node.ParentNode.NodeType != XmlNodeType.Document)
            {
                if (node.NodeType != XmlNodeType.Text)
                {
                    stringName = stringName.Insert(0, "/" + node.Name);
                    node = node.ParentNode;
                }
                else
                {
                    
                    _isText = true;
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
            this._selectedTreeViewItem = (e.OriginalSource as TreeViewItem);
        }

        private void CreateTagCollection(XmlNode element)
        {
            _tagCollection.Clear();
                if (element != null && element.HasChildNodes)
                {
                    foreach (XmlNode e in element.ChildNodes)
                    {
                    _tagCollection.Add(new TagItem(e));
                    }
                }            
        }
                
        private void selectFileButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.FileName = "Document"; 
            ofd.DefaultExt = ".xml";
            ofd.Filter = "XML documents (.xml)|*.xml";            
            Nullable<bool> result = ofd.ShowDialog();
            if (result == true)
            {
                fileName = ofd.FileName;
                selectedFileLabel.Content = "Selected XML file:   " + fileName;
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
            });
            if (tagStorage.Root != null)
                _isOpenXmlDocument = true;
        }

        private void uploadTagTreeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isOpenXmlDocument)
                return;

            MessageBoxResult result = MessageBox.Show("Выгрузить дерево и сохранить изменения в Xml файле?", "Внимание!!!", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                this.tagStorage.SaveXmlDocument();
                Task.Factory.StartNew(() =>
                {
                    this.outTreeView.Dispatcher.Invoke(() => outTreeView.ItemsSource = null);
                });
                _isOpenXmlDocument = false;
                tagStorage = null;
                tagItem = null;
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isOpenXmlDocument)
                return;

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

                if (_tagCollection.Contains(selectedTag))
                {
                    foreach (TagItem tag in this._tagCollection)
                    {
                        if (tag == selectedTag)
                        {
                            selectedTag.ChildCollection.Add(new TagItem(newName, newType));
                            //TagItem selectedTreeViewItemParent2 = (TagItem)ItemsControl.ItemsControlFromItemContainer(_selectedTreeViewItem).DataContext;
                            //selectedTreeViewItemParent2.GetChildByName(selectedTag.Name).AppendChild(tagStorage.xmlDoc.CreateElement(newName));
                        }
                    }
                    _isChanged = true;
                }
                else
                {
                    selectedTag.ChildCollection.Add(new TagItem(newName, newType));

                    TagItem selectedTreeViewItemParent2 = (TagItem)ItemsControl.ItemsControlFromItemContainer(_selectedTreeViewItem).DataContext;
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
                        if (!_isText)
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
                TagItem selectedTreeViewItemParent = (TagItem)ItemsControl.ItemsControlFromItemContainer(_selectedTreeViewItem).DataContext;
                foreach (TagItem tag in selectedTreeViewItemParent.ChildCollection)
                {
                    if (tag == selectedTag)
                    {
                        tag.Name = newName;
                        _isChanged = true;
                        string xPathName = CreateXPathName(selectedTag.tag);
                        //xPathName = xPathName.TrimEnd
                        if (!_isText)
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
                //if (!_isText)
                //    tagStorage.RenameTag(xPathName, newName);
                //else
                //{
                //    //xPathName = xPathName.TrimEnd(@"#text".ToCharArray());
                //    tagStorage.RenameTag(xPathName, newName);
                //}

            }

            _isChanged = true;

            //reeViewItem _selectedTreeViewItem = (TreeViewItem)outTreeView.SelectedItem;
            //TreeViewItem _selectedTreeViewItem = (TreeViewItem)(e.OriginalSource as MenuItem).ItemsSource;
            //TreeViewItem _selectedTreeViewItem = (MenuItem)e.OriginalSource;
            //_selectedTreeViewItem.Header = "123";
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
                if (_tagCollection.Contains(selectedTag))
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
                    //        _isText = true;
                    //        //break;
                    //        xPathName = xPathName.Insert(0, "/" + currentTag.Name);
                    //        currentTag = currentTag.ParentNode;
                    //    }
                    //}
                    string xPathName = CreateXPathName(selectedTag.tag);
                    //xPathName = xPathName.Insert(0, "/");
                    if (!_isText)
                        tagStorage.RemoveTag(xPathName);
                    else
                    {
                        xPathName = xPathName.TrimEnd(@"#text".ToCharArray());
                        tagStorage.RemoveTypedTag(xPathName, "");
                    }
                    if (_isText)
                        _isText = false;
                    _isChanged = true;
                }
                //
                else
                {
                    if (selectedTag.isNewTag)
                    {
                        tagStorage.RemoveNewTag(selectedTag.Name);
                        TagItem selectedTreeViewItemParent2 = (TagItem)ItemsControl.ItemsControlFromItemContainer(_selectedTreeViewItem).DataContext;
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
                    //        _isText = true;
                    //        //break;
                    //        xPathName = xPathName.Insert(0, "/" + node.Name);
                    //        node = node.ParentNode;
                    //    }
                    //}
                    //xPathName = xPathName.Insert(0, "/");
                    string xPathName = CreateXPathName(selectedTag.tag);
                    if (!_isText)
                        tagStorage.RemoveTag(xPathName);
                    else
                    {
                        xPathName = xPathName.TrimEnd(@"#text".ToCharArray());
                        tagStorage.RemoveTypedTag(xPathName, "");
                    }
                    if (_isText)
                        _isText = false;
                    //tagItem.RemoveChildTag(selectedTag.Name);
                    //virtualTree = ItemsControl.ItemsControlFromItemContainer(_selectedTreeViewItem);
                    TagItem selectedTreeViewItemParent = (TagItem)ItemsControl.ItemsControlFromItemContainer(_selectedTreeViewItem).DataContext;
                    //tagItem.RemoveChildTag(selectedTag.Name);
                    //tagStorage.RemoveTag(selectedTag.Name);
                    selectedTreeViewItemParent.ChildCollection.Remove(selectedTag);
                    _isChanged = true;
                }
            }
        }   //Remove

        private void Window_Closing(object sender, CancelEventArgs e)
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
                        _isClosedWindow = true;
                        this.tagStorage.fileName = sfd.FileName;
                        this.tagStorage.SaveXmlDocument();
                    }
                    else
                        _isClosedWindow = true;
                }

            }

            Application.Current.Shutdown();
        }
    }
}
