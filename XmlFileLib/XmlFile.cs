using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XmlFileLib
{
    public static class XmlFile
    {
        public static DataTable GetFilledDataTable(string path)
        {
            DataTable dataTable = new DataTable();
            dataTable.TableName = Path.GetFileNameWithoutExtension(path);

            XmlDocument XMLDoc = new XmlDocument();
            XMLDoc.Load(path);

            XmlNodeList list = XMLDoc.GetElementsByTagName("rows");
            foreach (string col in GetAllPossibleAttributes(list[0]))
            {
                dataTable.Columns.Add(col, typeof(string));
            }

            XmlNodeList rows = list[0].ChildNodes;
            foreach (XmlNode node in rows)
            {
                object[] row = new object[dataTable.Columns.Count];
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    row[i] = node.Attributes.GetNamedItem(dataTable.Columns[i].ColumnName)?.Value;
                }
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        public static void SaveDataTableToXMl(DataTable dataTable, string path)
        {
            XmlDocument XMLDoc = new XmlDocument();
            XMLDoc.Load(path);

            XmlNodeList list = XMLDoc.GetElementsByTagName("rows");
            list[0].RemoveAll();

            foreach (DataRow row in dataTable.Rows)
            {
                XmlNode child = XMLDoc.CreateElement("row");
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    string columnName = dataTable.Columns[i].ColumnName;
                    XmlAttribute attribute = XMLDoc.CreateAttribute(columnName);
                    if (!String.IsNullOrEmpty(row[columnName].ToString()))
                    {
                        attribute.Value = row[columnName].ToString();
                        child.Attributes.Append(attribute);
                    }
                }

                list[0].AppendChild(child);
            }

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            settings.NewLineChars = Environment.NewLine;
            settings.Encoding = Encoding.UTF8;

            using (XmlWriter writer = XmlWriter.Create(path, settings))
            {
                XMLDoc.Save(writer);
            }
        }

        private static List<string> GetAllPossibleAttributes(XmlNode node)
        {
            LinkedList<string> linkedList = new LinkedList<string>();

            LinkedListNode<string> prev = new LinkedListNode<string>(node.FirstChild.Attributes[0].Name);
            linkedList.AddFirst(prev);

            foreach (XmlNode child in node.ChildNodes)
            {
                foreach (XmlAttribute attribute in child.Attributes)
                {
                    if (linkedList.Contains(attribute.Name))
                    {
                        prev = linkedList.Find(attribute.Name);
                    }
                    else
                    {
                        prev = linkedList.AddAfter(prev, attribute.Name);
                    }
                }
            }
            return linkedList.ToList();
        }

    }
}
