using System;

using System.Configuration;
using System.Collections.Generic;
using System.Linq;

using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Text;

    public class XmlHelper
    {
        public XmlHelper()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        /// <summary>
        /// 创建一个具有默认根结点的XML文件
        /// </summary>
        /// <returns></returns>
        public static bool CreateFile(string XMLPath, string rootname)
        {
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                XmlDeclaration xn = xmldoc.CreateXmlDeclaration("1.0", "gb2312", null);
                xmldoc.AppendChild(xn);
                XmlElement xmlelem = xmldoc.CreateElement("", rootname, "");
                xmldoc.AppendChild(xmlelem);
                xmldoc.Save(XMLPath);
                return true;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// 根据单一节点的名字获取它的InnerText
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static string FindByName(string XMLPath, string Name)
        {
            XmlDocument xmldoc = CheckFileExit(XMLPath);
            XmlElement root = xmldoc.DocumentElement;
            foreach (XmlNode xmlNode in root.ChildNodes)
            {
                if (xmlNode.Name == Name)

                    return xmlNode.InnerText;
            }
            return "";
        }

        /// <summary>
        /// 根据单一节点的名字以及属性的名字获取其属性的值
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static string FindAttrByName(string XMLPath, string NoteName,string AttrName)
        {
            XmlDocument xmldoc = CheckFileExit(XMLPath);
            XmlElement root = xmldoc.DocumentElement;
            foreach (XmlNode xmlNode in root.ChildNodes)
            {
                if (xmlNode.Name == NoteName)
                {
                    XmlElement elem = xmldoc.GetElementById(NoteName);
                    string str = xmlNode.Attributes[AttrName].Value;
                    return str;
                }
            }
            return "";
        }

        /// <summary>
        /// 根据单一节点的属性获取它的InnerText
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static string FindByAttr(string XMLPath, string Attr)
        {
            XmlDocument xmldoc = CheckFileExit(XMLPath);
            XmlElement root = xmldoc.DocumentElement;
            foreach (XmlNode xmlNode in root.ChildNodes)
            {
                if (xmlNode.Attributes.Item(0).Value == Attr)

                    return xmlNode.InnerText;
            }
            return "";
        }

        /// <summary>
        /// 从指定的XML文件中读取指定路径下第一级节点集合
        /// </summary>
        /// <param name="XMLFile">带路径的XML文件名</param>
        /// <param name="Path">带路径的结点名</param>
        /// <returns></returns>
        public static string[] ReadNodes(string XMLFile, string Path)
        {
            string Values = "";
            if (CheckFileExit(XMLFile) == null)
            {
                throw new Exception("指定的文件不存在！");
                return null;
            }

            XmlDocument xmldoc = CheckFileExit(XMLFile);
            XmlNode Node = FindNode(xmldoc, Path);
            foreach (XmlNode node in Node.ChildNodes)
            {
                Values += node.InnerText.ToString() + ";";
            }
            return Values.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        }

        /// <summary>
        /// 从指定的XML文件中读取指定路径下单个结点的Name
        /// </summary>
        /// <param name="XMLFile"></param>
        /// <param name="Path"></param>
        /// <param name="DefaultValue"></param>
        /// <returns></returns>
        public static string ReadNode(string XMLFile, string Path)
        {
            if (CheckFileExit(XMLFile) == null)
            {
                throw new Exception("指定的文件不存在！");
            }
            XmlDocument xmldoc = CheckFileExit(XMLFile);
            XmlNode Node = FindNode(xmldoc, Path);
            if (Node != null)
            {
                return Node.Name;
            }
            return null;
        }

        /// <summary>
        /// 在指定节点处插入一个节点
        /// </summary>
        /// <param name="XMLFile"></param>
        /// <param name="Path"></param>
        /// <param name="Value">结点值</param>
        public static void InsertNode(string XMLFile, string tor, string value, string text)
        {
            if (CheckFileExit(XMLFile) == null)
            {
                throw new Exception("指定的文件不存在！");
                return;
            }
            XmlDocument myDoc = CheckFileExit(XMLFile);
            XmlNode newnode = CreateElement(myDoc, value, text);
            XmlNode first = myDoc.SelectSingleNode(tor);
            myDoc.InsertAfter(newnode, first);
            //调用insertAfter节点只要要插入节点的父节点(可以是祖先节点),引用节点必须要插入节点的兄弟节点。
            myDoc.Save(XMLFile);

        }

        /// <summary>
        /// 更新指定结点的值
        /// </summary>
        /// <param name="TextName">结点的值</param>
        /// <param name="XNe">要更新的结点</param>
        /// <returns></returns>
        public static bool UpdateNode(string TextName, XmlNode XNe)
        {
            if (XNe != null)
            {
                XNe.InnerText = TextName;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 创建一个指定的XML元素
        /// </summary>
        /// <param name="myDoc"></param>
        /// <param name="ElementName">元素名</param>
        /// <param name="Text">元素值</param>
        /// <returns></returns>
        public static XmlNode CreateElement(XmlDocument myDoc, string ElementName, string Text)
        {
            if (ElementName != "")
            {
                XmlElement XEt = myDoc.CreateElement("", ElementName, "");
                XEt.InnerText = Text;
                return (XmlNode)XEt;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 删除指定的结点
        /// </summary>
        /// <param name="XMLFile"></param>
        /// <param name="Path"></param>
        /// <returns></returns>
        public static bool DeleteNode(string XMLFile, string Path)
        {
            if (CheckFileExit(XMLFile) == null)
            {
                throw new Exception("指定的文件不存在！");
            }
            XmlDocument myDoc = CheckFileExit(XMLFile);
            XmlNode Node = FindNode(myDoc, Path);
            if (Node != null)
            {
                if (Node.ParentNode != null)
                {
                    Node.ParentNode.RemoveChild(Node);
                }
                else
                {
                    myDoc.RemoveChild(Node);
                }
                myDoc.Save(XMLFile);
            }
            return true;
        }



        /// <summary>
        /// 查询指定的XML文件是否存在.
        /// </summary>
        /// <param name="XMLFile"></param>
        /// <returns></returns>
        private static XmlDocument CheckFileExit(string XMLFile)
        {
            XmlDocument xmldoc = new XmlDocument();
            if (File.Exists(XMLFile))
            {
                try
                {
                    xmldoc.Load(XMLFile);
                }
                catch (Exception ex)
                {
                    throw new Exception("载入XML文件[" + XMLFile + "]失败.\r\n" + ex.Message);
                    return null;
                }
                return xmldoc;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取路径结点集指定深度的路径表示
        /// </summary>
        /// <param name="depth"></param>
        /// <returns></returns>
        private static string GetPath(string[] PathNode, int depth)
        {
            string path = "";
            for (int i = 0; i < depth && i < PathNode.Length; i++)
            {
                path += PathNode[i] + "/";
            }
            return path.Substring(0, path.Length - 1);
        }

        /// <summary>
        /// 查询指定路径上的第一个结点
        /// </summary>
        /// <param name="myDoc"></param>
        /// <param name="Path">xpath</param>
        /// <returns></returns>
        private static XmlNode FindNode(XmlDocument myDoc, string Path)
        {
            XmlNode myNode = myDoc.SelectSingleNode(Path);
            if (!(myNode == null))
            {
                return myNode;
            }
            return null;
        }

        /// <summary>
        /// xml序列化
        /// </summary>
        /// <param name="myObject">支持序列化的对象</param>
        public static string Serialize<T>(T myObject)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));

            StringBuilder sb = new StringBuilder();

            using (TextWriter tr = new StringWriter(sb))
            {
                ser.Serialize(tr, myObject);
                tr.Close();
            }
            return sb.ToString();
        }

        /// <summary>
        /// xml反串行化
        /// </summary>
        /// <param name="xml">xml字符串</param>
        public static T Deserialize<T>(string xml)
            where T : class
        {

            if (String.IsNullOrEmpty(xml))
            {
                throw new Exception(String.Format("XmlData is Null or Empty!"));
            }
            T result;

            XmlSerializer ser = new XmlSerializer(typeof(T));

            using (TextReader tReader = new StringReader(xml))
            {
                result = ser.Deserialize(tReader) as T;
                tReader.Close();
            }

            return result;
        }
    }


