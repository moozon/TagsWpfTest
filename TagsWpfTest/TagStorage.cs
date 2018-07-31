using System;
using System.Windows;
using System.Xml;

namespace TagsWpfTest
{
    /// <summary>
    /// Класс TagStorage содержит корневой тэг Root типа none, который является родителем всех тэгов, содержащихся в TagStorage. Так же TagStorage выполняет функции по загрузке/выгрузке в файл структуры дерева тэгов и их текущих значений.
    /// </summary>
    public class TagStorage
    {
        //Закрытые поля
        public XmlDocument xmlDoc; //Xml документ
        private XmlNode root;       //Корневой узел

        //private TagItem tag;      //Переменная TagItem класса

        //Открытые поля
        public string fileName = "TagsTest.xml";      //Xml файл
        public XmlNode parent;
        //public string fileName2 = "TagsTest2.xml";    //Тестовый Xml файл

        //Конструктор
        public TagStorage(string fileName)
        {
            this.fileName = fileName;
            xmlDoc = new XmlDocument(); //Создание экземпляра XmlDocument класса
            LoadXmlDocument();          //Загрузка дерева
        }

        //Свойства
        /// <summary>
        /// Возвращает корневой узел
        /// </summary>
        public XmlNode Root
        {
            get
            {
                return root;
            }
            set
            {
                if (value != null && value.NodeType == XmlNodeType.Element)
                    root = value;
            }
        }

        //Закрытые методы
        /// <summary>
        /// Рекурсивно перебирает все теги и выводит на экран их имена, пути, значения, типы, уровни вложенности
        /// </summary>
        /// <param _name="element">Тег с которого строится дерего и выводится на экран</param>
        private void PrintRec(XmlNode element)
        {
            if (element == null)        //Выход, если пустая ссылка
                return;
            TagItem tagItem = new TagItem(element);

            tagItem.Print();

            if (element.HasChildNodes)
            {
                foreach (XmlNode e in element.ChildNodes)
                {
                    PrintRec(e);
                }
            }

        }
        /// <summary>
        /// Загружает дерево тегов из Xml файла
        /// </summary>
        private void LoadXmlDocument()
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                try
                {
                    xmlDoc.Load(fileName);
                    root = xmlDoc.DocumentElement;
                }
                catch (Exception e)
                {
                    MessageBox.Show("Проверьте правильность имени XML файла", "Имя XML файла указано неверно.");
                    //throw new Exception(e.Message, e);

                }
            }
            else
                MessageBox.Show("Сначала выберите Xml файл.", "Не указано имя XML файла");
        }

        //Открытые методы
        /// <summary>
        /// Возвращает указанный тег
        /// </summary>
        /// <param _name="path">Путь к тегу</param>
        /// <returns>Возвращает экземпляр XmlNode класса</returns>
        public XmlNode FindTag(string path)
        {
            try
            {
                return root.SelectSingleNode(path);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }
        /// <summary>
        /// Выводит на экран полное дерево тегов
        /// </summary>
        public void PrintAll()
        {
            try
            {
                PrintRec(root);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }
        /// <summary>
        /// Выводит на экран заданный тег и все его дочерние теги
        /// </summary>
        /// <param _name="name">Полный путь к тегу</param>
        public void PrintTag(string name)
        {
            try
            {
                PrintRec(root.SelectSingleNode(name));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }
        /// <summary>
        /// Выгружает дерево тегов в Xml файл
        /// </summary>
        public void SaveXmlDocument()
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return;
            try
            {
                xmlDoc.Save(fileName);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }
        /// <summary>
        /// Удаляет указанный тег из дерева
        /// </summary>
        /// <param _name="path">Полный путь к тегу</param>
        public void RemoveTag(string path)
        {
            try
            {
                //FindParent()
                xmlDoc.SelectSingleNode(path).CreateNavigator().DeleteSelf();  //Удаление осуществляется с помощью метода DeleteSelf класса XPathNavigator

            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }

        }
        public void RemoveNewTag(string path)
        {
            string stringName = "";
            FindParent(path);
            while (parent.ParentNode != null && parent.ParentNode.NodeType != XmlNodeType.Document)
            {
                stringName = stringName.Insert(0, "/" + parent.Name);
                parent = parent.ParentNode;

            }
            stringName = stringName + "/" + path;
            stringName = stringName.Insert(0, "/");
            try
            {
                //FindParent()
                xmlDoc.SelectSingleNode(stringName).CreateNavigator().DeleteSelf();  //Удаление осуществляется с помощью метода DeleteSelf класса XPathNavigator

            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }

        }
        public void RemoveTypedTag(string path, string value)
        {
            if (path[path.Length - 1] == '/')
                path = path.TrimEnd('/');
            xmlDoc.SelectSingleNode(path).CreateNavigator().SetValue(value);
        }
        /// <summary>
        /// Добавляет новый тег в конец списка дочерних тегов
        /// </summary>
        /// <param _name="path">Родительский тег в теле которого, будет создан новый</param>
        /// <param _name="name">Имя нового тега</param>
        /// <param _name="type">Тип нового тега</param>
        public void AddTag(string path, string name, string type)
        {
            try
            {
                TagItem tagItem = new TagItem(root.SelectSingleNode(path));
                tagItem.AddChildTag(name);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }
        /// <summary>
        /// Переименовывает заданный тег
        /// </summary>
        /// <param _name="path">Путь переименовываемого тега</param>
        /// <param _name="newname">Новое имя тега</param>
        public void RenameTag(string path, string newName)
        {
            try
            {
                TagItem tagItem = new TagItem(root.SelectSingleNode(path));
                tagItem.RenameTag(newName);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        public XmlNode FindParent(string nameChild)
        {
            FindChild(xmlDoc.DocumentElement, nameChild);
            return parent;

            //XmlNode currentNode = childNode;
            //if (currentNode.HasChildNodes)
            //{
            //    foreach (XmlNode node in currentNode.ChildNodes)
            //    {
            //        if (node.Name == currentNode.Name)
            //        {
            //            parent = node.ParentNode;
            //            return;
            //        }
            //        else
            //            FindParent(node);
            //    }
            //}
        }

        public void FindChild(XmlNode node, string nameChild)
        {
            if (node.HasChildNodes)
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    if (child.Name == nameChild)
                    {
                        parent = node;
                        return;
                    }                    
                    FindChild(child, nameChild);
                }
            }
        }

    }
}
