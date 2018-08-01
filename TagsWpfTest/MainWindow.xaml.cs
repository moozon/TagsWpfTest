using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;


namespace TagsWpfTest
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _fileName = "";                          //Путь к XML файлу
        private ObservableCollection<TagItem> _tagCollection;   //Коллекция 1-го уровня
        private XmlElement _emptyElement;                       //Пустой XML узел 
        private TreeViewItem _selectedTreeViewItem;             //Выделенный элемент TreeView
        private bool _isChanged = false;                        //Если есть изменения в дереве
        private bool _isText = false;                           //Если тип узла текст
        private bool _isOpenXmlDocument = false;                //Если XML документ открыт

        public TagStorage tagStorage;                           //Переменная TagStorage
        public TagItem tagItem;                                 //Переменная TagItem
        
        //Constructor
        public MainWindow()
        {
            InitializeComponent();
        }


        //Properties

        /// <summary>
        /// Коллекция 1-го уровня
        /// </summary>
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

        /// <summary>
        /// Инициализация при загрузке окна
        /// </summary>
        private void Window_Loaded(object sender, EventArgs e)
        {
            //Пустой XML узел
            _emptyElement = new XmlDocument().DocumentElement;
            //Создание екземпляра для загрузки пустого дерева
            tagItem = new TagItem(_emptyElement);
            _tagCollection = new ObservableCollection<TagItem>();
            this.Title = "TreeView";
            //В качестве контента данных используем самих себя
            this.DataContext = this;
            //Загрузка пустого дерева в отдельном потоке
            Task.Factory.StartNew(() => outTreeView.Dispatcher.Invoke(() => outTreeView.ItemsSource = TagCollection));
        }
        /// <summary>
        /// Формирование полного пути тега для метода RemoveTag() для измения шаблона дерева
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
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
        /// Запоминаем выделенный узел при выделении в TreeView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            this._selectedTreeViewItem = (e.OriginalSource as TreeViewItem);
        }
        /// <summary>
        /// Создание коллекции элементов 1-го уровня для биндинга
        /// </summary>
        /// <param name="element"></param>
        private void CreateTagCollection(XmlNode element)
        {
            //Предварительная очистка коллекции
            _tagCollection.Clear();
                if (element != null && element.HasChildNodes)
                {
                    foreach (XmlNode e in element.ChildNodes)
                    {
                    _tagCollection.Add(new TagItem(e));
                    }
                }            
        }
        /// <summary>
        /// Загрузка XML файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>    
        private void selectFileButton_Click(object sender, RoutedEventArgs e)
        {
            //Выбор XML файла
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.FileName = "Document"; 
            ofd.DefaultExt = ".xml";
            ofd.Filter = "XML documents|*.xml";            
            Nullable<bool> result = ofd.ShowDialog();
            if (result == true)
            {
                _fileName = ofd.FileName;
                selectedFileLabel.Content = "Selected XML file:   " + _fileName;
            }
        }
        /// <summary>
        /// Загрузка дерева
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadTagTreeButton_Click(object sender, RoutedEventArgs e)
        {
            //Инстансыруем классы
            tagStorage = new TagStorage(_fileName);
            tagItem = new TagItem(tagStorage.Root);
            //Создаем главную коллекцию
            CreateTagCollection(tagStorage.Root);
            //Привязываем коллекцию к TreeView в отдельном потоке
            Task.Factory.StartNew(() => outTreeView.Dispatcher.Invoke(() => outTreeView.ItemsSource = TagCollection));
            //Документ открыт, если все в порядке
            if (tagStorage.Root != null)                
                _isOpenXmlDocument = true;
        }
        /// <summary>
        /// Выгрузка дерева
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uploadTagTreeButton_Click(object sender, RoutedEventArgs e)
        {   
            //Игнорируем, если документ не открыт
            if (!_isOpenXmlDocument)
                return;
            //Запрос на выгрузку дерева
            MessageBoxResult result = MessageBox.Show("Выгрузить дерево и сохранить изменения в Xml файле?", "Внимание!!!", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                this.tagStorage.SaveXmlDocument();
                //Выгрузка дерева в другом потоке
                Task.Factory.StartNew(() =>
                {
                    this.outTreeView.Dispatcher.Invoke(() => outTreeView.ItemsSource = null);
                });
                //Документ закрыт
                _isOpenXmlDocument = false;
                //Зануляем инстансы 
                tagStorage = null;
                tagItem = null;
            }
        }
        /// <summary>
        /// Сохранение дерева в укзаный XML файл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            //Игнорируем, если документ не открыт
            if (!_isOpenXmlDocument)
                return;
            //Вывод диалога для выбора файла
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.FileName = "Document";
            sfd.DefaultExt = ".xml";
            sfd.Filter = "XML documents|*.xml";
            Nullable<bool> resultSfd = sfd.ShowDialog();
            if (resultSfd == true)
            {
                //Меняем имя файла на выбранное в диалоге, сохраняем документ и возвращаем старое имя(Для выгрузки дерева в тот фаайл из которого загрущили)
                string oldFileName = this.tagStorage.fileName;
                this.tagStorage.fileName = sfd.FileName;
                this.tagStorage.SaveXmlDocument();
                this.tagStorage.fileName = oldFileName;
            }
            //Изменения сохранены
            _isChanged = false;

        }
        /// <summary>
        /// Добавление тега
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //Окна для ввода имени и типа нового тега
            var newTagWindow = new NewTagWindow();
            newTagWindow.ShowDialog();
            //Записываем двнные в переменные
            string newName = newTagWindow.NewName;
            string newType = newTagWindow.NewType;
            //Проверка на Null
            if (string.IsNullOrWhiteSpace(newName))
            {
                MessageBox.Show("Введите имя");
                return;
            }
            //Проверка на корректность введенного типа
            bool isValidType = false;
            foreach(string type in Enum.GetNames(typeof(XmlNodeType)))
            {
                if (newType == type)
                    isValidType = true;
            }
            //Уведомление, если тип введен не правильно
            if (!isValidType)
            {
                MessageBox.Show("Неверно указан тип. Повторите попытку", "Внимание!!!");
                return;
            }
            else
            {
                //Выделенный тег
                TagItem selectedTag = (TagItem)outTreeView.SelectedItem;
                //Принадлежит главной коллекции
                if (_tagCollection.Contains(selectedTag))
                {
                    //Поиск в коллекции
                    foreach (TagItem tag in this._tagCollection)
                    {
                        if (tag == selectedTag)
                        {   
                            //Добавление нового тега в локальную коллекцию тега
                            selectedTag.ChildCollection.Add(new TagItem(newName, newType));
                        }
                    }
                    //Изменения
                    _isChanged = true;
                }
                //Тег принадлежит Виртуальному дереву
                else
                {
                    //Добавление нового тега в локальную коллекцию тега
                    selectedTag.ChildCollection.Add(new TagItem(newName, newType));
                    TagItem selectedTreeViewItemParent2 = (TagItem)ItemsControl.ItemsControlFromItemContainer(_selectedTreeViewItem).DataContext;
                    selectedTreeViewItemParent2.GetChildByName(selectedTag.Name).AppendChild(tagStorage.xmlDoc.CreateElement(newName));
                    //Изменнеия
                    _isChanged = true;
                }
            }
            
        } 
        /// <summary>
        /// Переименование тега
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            //Окно для ввода нового имени
            var renameWindow = new RenameWindow();
            renameWindow.ShowDialog();
            //Сохраняем новое имя в переменную
            string newName = renameWindow.NewName;
            //проверка на пустую строку имени
            if (string.IsNullOrWhiteSpace(newName))
            {
                MessageBox.Show("Введите имя");
                return;
            }
            //Выбранный тег в TreeView
            TagItem selectedTag = (TagItem)outTreeView.SelectedItem;
            //Выбранный тег принадлежит главной колекции
            if (this.TagCollection.Contains(selectedTag))
            {
                //Находим тег который нужно переименовать
                foreach (TagItem tag in TagCollection)
                {
                    if (tag == selectedTag)
                    {
                        //Изменяем имя
                        tag.Name = newName;
                        //Обновляем щаблон дерева
                        string xPathName = CreateXPathName(selectedTag.tag);
                        if (!_isText)
                            tagStorage.RenameTag(xPathName, newName);
                        else
                        {
                            tagStorage.RenameTag(xPathName, newName);
                        }
                        //Бмли изменения
                        _isChanged = true;
                        //ВЫходим их цикла при нахождении
                        return;
                    }
                }
            }
            //Тег принадлежит Виртуальному дереву
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
            }
            //Дерево изменено
            _isChanged = true;

        }  
        /// <summary>
        /// Удаление тега
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {     
            MessageBoxResult result = MessageBox.Show("Удалить выбранную ветвь?", "Внимание!!!", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                TagItem selectedTag = (TagItem)outTreeView.SelectedItem;
                //Выбранный тег принадлежит главной колекции
                if (_tagCollection.Contains(selectedTag))
                {
                    //Очищаем главную коллецию
                    this.TagCollection.Remove(selectedTag);
                    //Формирование полного пути тега
                    string xPathName = CreateXPathName(selectedTag.tag);
                    //Если тип Текст, то изменяем строку пути используем перегрузку метода удаления тега из дерева
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
                //Тег принадлежит Виртуальному дереву 
                else
                {   //Если добавленный тег
                    if (selectedTag.isNewTag)
                    {
                        //Удаляем из шаблона дерева
                        tagStorage.RemoveNewTag(selectedTag.Name);
                        //Находим родительский контейнер выбранного тега в TreeView 
                        TagItem selectedTreeViewItemParent2 = (TagItem)ItemsControl.ItemsControlFromItemContainer(_selectedTreeViewItem).DataContext;
                        //Удаляем выбранный тег
                        selectedTreeViewItemParent2.ChildCollection.Remove(selectedTag);
                        return;
                    }
                    //Формирование полного пути тега
                    string xPathName = CreateXPathName(selectedTag.tag);
                    //Аналогичное описание в ифе
                    if (!_isText)
                        tagStorage.RemoveTag(xPathName);
                    else
                    {
                        xPathName = xPathName.TrimEnd(@"#text".ToCharArray());
                        tagStorage.RemoveTypedTag(xPathName, "");
                    }
                    if (_isText)
                        _isText = false;
                    TagItem selectedTreeViewItemParent = (TagItem)ItemsControl.ItemsControlFromItemContainer(_selectedTreeViewItem).DataContext;
                    selectedTreeViewItemParent.ChildCollection.Remove(selectedTag);
                    _isChanged = true;
                }
            }
        } 
        /// <summary>
        /// Закрытие окна. Если есть несохраненные изменения, то сохраняем
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
    }
}
