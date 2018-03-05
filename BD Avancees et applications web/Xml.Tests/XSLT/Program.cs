using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace XSLT
{
    class Program
    {
        static void Main(string[] args)
        {

            string xsltPath = "CDsDe1983.xslt";
            string xmlPath = "CDs.xml";
            string outPath = "out.html";

            var doc = new XPathDocument(xmlPath);
            var xslt = new System.Xml.Xsl.XslCompiledTransform(true);

            xslt.Load(xsltPath);

            using (var writer = new System.Xml.XmlTextWriter(outPath, Encoding.UTF8))
            {
                xslt.Transform(doc, writer);
            }
            System.Diagnostics.Process.Start(outPath);
        }
    }
}
