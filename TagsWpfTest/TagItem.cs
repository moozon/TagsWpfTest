﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;


namespace TagsWpfTest
{
    /// <summary>
    /// Класс TagItem реализует возможность хранения имени тэга (метки) и его значения. Значение типа тэга может быть разным - double, int, bool или none, если не хранит значения. Так же TagItem должен содержать список дочерних тэгов, в результате чего организуется древовидная иерархия тэгов.
    /// </summary>
    public class TagItem : INotifyPropertyChanged
    {
        
        private string _value;       //Значение тега
        private string _name;        //Имя тега
        private int _level;          //Уровень вложенности тега
        private string _type;        //Тип тега        
        private string _fullPath;    //Путь вложенности тега
        private XmlNode _root;       //Корневой узел
        private ObservableCollection<TagItem> _allChildCollection;       //Коллекция всех дочерних элементов типа TagItem
        private TagItem _parent;     //Родительский элемент
        private XmlNodeList _emptyXmlNodeList;                           //Пустая коллекция XMLNode элементов


        public XmlNode tag;                     //Переменная TagItem класса
        public List<XmlNode> parentElements;    //Список родительских тегов для вычисления уровня(Level) и пути(FullPath) вложенности
        public XmlDocument xmlDoc;              //Xml документ
        public ObservableCollection<TagItem> childCollection;          //Коллекция дочерних элементов типа TagItem
        public event PropertyChangedEventHandler PropertyChanged;       //Событие на изменение имени
        public bool isNewTag = false;


        //Constructor
        public TagItem(XmlNode element)
        {
            if (element == null)                    //Выход если ссылка пустая  
                return;
            
            tag = element;
            xmlDoc = element.OwnerDocument;
            _root = xmlDoc.DocumentElement;
            _value = element.Value;
            _name = element.Name;
            _type = element.NodeType.ToString();
            _allChildCollection = new ObservableCollection<TagItem>();
            childCollection = new ObservableCollection<TagItem>();
            parentElements = new List<XmlNode>();   //Инициализация списка родительских тегов
            UpdatePathLevel();                      //Вычисление уровня(Level) и пути(FullPath) вложенности
            CreateChildCollection(element);         //Создание коллекции дочерних элементов

        }

        
        /// <summary>
        /// Конструктор для добавления новых типизированных узлов
        /// </summary>
        /// <param name="nameEmptyElement"></param>
        /// <param name="type"></param>
        public TagItem(string nameEmptyElement, string type)
        {
            isNewTag = true;
            XmlNode element = new XmlDocument().CreateElement(nameEmptyElement);
            tag = element;
                xmlDoc = element.OwnerDocument;
                _root = xmlDoc.DocumentElement;
                _value = element.Value;
            
            _name = element.Name;
            _type = type;

            _allChildCollection = new ObservableCollection<TagItem>();
            childCollection = new ObservableCollection<TagItem>();
            parentElements = new List<XmlNode>();   //Инициализация списка родительских тегов


            UpdatePathLevel();                      //Вычисление уровня(Level) и пути(FullPath) вложенности
            CreateChildCollection(element);         //Создание коллекции дочерних элементов
        }

        

        //Properties
        
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;                           //Присваиваем новое значение имени
                UpdatePathLevel();                      //Обновляем уровень(Level) и путь(FullPath) вложенности
                NotifyPropertyChanged();
            }

        }
        public string Value
        {
            get
            {
                return _value;
            }

            set
            {
                this._value = value;
            }

        }
        public int Level
        {
            get
            {
                return _level;
            }

        }
        public string Type
        {
            get
            {
                return _type;
            }

        }
        public string FullPath
        {
            get
            {
                return _fullPath;
            }
        }
        public bool HasChilds
        {
            get
            {
                if (tag.HasChildNodes)
                    return true;
                else
                    return false;
            }
        }
        public XmlNodeList ChildXmlNodes
        {
            get
            {
                if (tag.HasChildNodes)
                    return tag.ChildNodes;
                else
                    return _emptyXmlNodeList;
            }
        }
        public ObservableCollection<TagItem> ChildCollection
        {
            get
            {
                //CreateChildCollection(tag);
                return childCollection;
            }
            set
            {
                childCollection = value;
                //NotifyPropertyChanged();
            }
        }
        public TagItem Parent
        {
            get
            {
                
                try
                {
                    if (tag.ParentNode != null && tag.ParentNode.NodeType != XmlNodeType.Document)
                        _parent = new TagItem(tag.ParentNode);
                    else
                        _parent  = null;
                }
                catch (Exception e)
                {

                    throw e;
                }
                return _parent;

            }
        } 
               

        //Methods

        /// <summary>
        /// //Вычисление уровня(Level) и пути(FullPath) вложенности
        /// </summary>
        private void UpdatePathLevel()
        {
            if (tag.ParentNode != null)
            {
                parentElements.Clear();     //Очистка списка родительских тегов
                _fullPath = "";              //Очистка пути родительских тегов
                if (tag.ParentNode.NodeType != XmlNodeType.Document)    //Условие добавления тегов в список родительских тегов
                    parentElements.Add(tag.ParentNode);
                else
                    parentElements.Add(tag);
                while (true)                //Перебор родительских тегов
                {
                    XmlNode element = parentElements.Last().ParentNode;

                    if (element != null)
                    {
                        if (element.NodeType != XmlNodeType.Document)   //Добавляем в список, если тип тега != Document
                        {
                            parentElements.Add(element);
                        }
                        else break;
                    }
                    else break;
                }

                parentElements.Reverse();                   //Реверс элементов списка для построения пути(FullPath) вложенности
                foreach (XmlNode element in parentElements)
                {
                    _fullPath += element.Name + ".";         //Формирование строки FullPath
                }

                if (_name == "Root")
                    _level = parentElements.Count;           //Вычисление уровня(Level) вложенности
                else
                {
                    _fullPath += _name;
                    _level = parentElements.Count + 1;
                }

                parentElements.Reverse();                   //Обратный реверс
            }
        }
        /// <summary>
        /// Создание коллекции дочерних TagItem элементов указанного тега
        /// </summary>
        public void CreateChildCollection(XmlNode node)
        {
            childCollection.Clear();
            if (node == null)        //Выход, если пустая ссылка
                return;
            if (node.HasChildNodes)
            {
                
                foreach (XmlNode n in node.ChildNodes)    
                {
                    childCollection.Add(new TagItem(n));
                }
            }
            NotifyPropertyChanged();
        }
        /// <summary>
        /// Метод для уведомления главного окна о изменениях
        /// </summary>  
        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// Добавляет новый тег в конец списка дочерних тегов
        /// </summary>
        /// <param _name="newChildTag">Имя нового тега</param>
        public void AddChildTag(string newChildTag)
        {
            try
            {
                XmlElement newNode = xmlDoc.CreateElement(newChildTag);     //Создание нового тега
                tag.AppendChild(newNode);                                   //Добавление нового тега в конец списка дочерних тегов
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }
        /// <summary>
        /// Удаляет указанный тег из дерева
        /// </summary>
        /// <param _name="remChildTagPath">Путь к удаляемому тегу</param>
        public void RemoveChildTag(string remChildTagPath)
        {
            try
            {
                tag.RemoveChild(tag.SelectSingleNode(remChildTagPath));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }
        /// <summary>
        /// Возвращает экземпляр тега типа XmlNode
        /// </summary>
        /// <param _name="pathTag">Полный путь к тегу </param>
        /// <returns></returns>
        public XmlNode GetChildByName(string pathTag)
        {
            try
            {
                return tag.SelectSingleNode(pathTag);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }
        /// <summary>
        /// Возвращает коллекцию дочерних тегов
        /// </summary>
        /// <returns></returns>
        public XmlNodeList GetChilds()
        {            
            return tag.ChildNodes;
        }
        /// <summary>
        /// Переименовывает заданный тег
        /// </summary>
        /// <param _name="newName">Новое имя тега</param>
        public void RenameTag(string newName)
        {
            XmlNode newNode = xmlDoc.CreateElement(newName);    //Создание нового пустого тега
            newNode.InnerXml = tag.InnerXml;                    //Копирование всех вложенных элементов из старого тега в новый
            try
            {
                tag.ParentNode.InsertBefore(newNode, tag);          //Вставка нового тека перед старым
                tag.ParentNode.RemoveChild(tag);                    //Удаление старого тега
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }
        /// <summary>
        /// Выводит на экран имя, путь, значение, тип, уровень вложенности тега
        /// </summary>
        public void Print()
        {
            Console.WriteLine("{2}({3})      {1}      {0}", _value, _type, _fullPath, _level);
        }
 
    }
}   
