using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Library
{
    public static class XmlClass
    {
        public static XmlDocument XmlOpen(string path)
        {
            if (!File.Exists(path))
                return CreateNew();

            XmlDocument xmlDoc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(path, settings);
            xmlDoc.Load(reader);
            reader.Close();
            return xmlDoc;
        }

        public static XmlDocument CreateNew()
        {
            XmlDocument xmldoc = new XmlDocument();
            //加入XML的声明段落,<?xml version="1.0" encoding="gb2312"?>
            XmlDeclaration xmldecl;
            xmldecl = xmldoc.CreateXmlDeclaration("1.0", "utf-8", null);
            xmldoc.AppendChild(xmldecl);
            return xmldoc;
        }

        public static XmlDocument CreateNew(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
            XmlDocument xmldoc = new XmlDocument();
            //加入XML的声明段落,<?xml version="1.0" encoding="gb2312"?>
            XmlDeclaration xmldecl;
            xmldecl = xmldoc.CreateXmlDeclaration("1.0", "utf-8", null);
            xmldoc.AppendChild(xmldecl);
            return xmldoc;
        }

        public static void XmlClearContent(XmlDocument xmlDoc)
        {
            xmlDoc.RemoveAll();
        }

    }
}
