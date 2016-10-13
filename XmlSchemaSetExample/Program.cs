using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;


namespace XmlSchemaSetExample
{
    class Program
    {
        static void Main()
        {
            //Console.WriteLine(IsValidXml("4.xml", "Fiducian.xsd"));
            //Console.ReadLine();
            StringBuilder builder = new StringBuilder();

            String connectionString = ConfigurationManager.ConnectionStrings["Fiducian"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("select FisXML from ApplicationFisXML FOR XML AUTO", connection))
            {
                connection.Open();
                using (XmlReader reader = command.ExecuteXmlReader())
                {
                    while (reader.Read())
                    {
                        String row = $"<{reader.Name}";
                        if(reader.HasAttributes)
                        {
                            String attributes = String.Empty;
                            for(int i = 0; i < reader.AttributeCount; i++)
                            {
                                attributes = reader.GetAttribute(i);
                                attributes += reader.ReadInnerXml();
                                attributes += reader.Value;
                                attributes += reader.XmlLang;
                            }
                        }
                        String element = $"<{reader.Name}";
                        builder.AppendLine(element);
                    }
                }
            }

            Console.WriteLine(builder.ToString());

            byte[] array = Encoding.UTF8.GetBytes(builder.ToString());
            using (FileStream stream = new FileStream("/my.xml", FileMode.OpenOrCreate))
            {
                stream.Write(array, 0, array.Length);
            }



            //      string xsdMarkup =
            //          @"<xsd:schema xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
            // <xsd:element name='Root'>
            //  <xsd:complexType>
            //   <xsd:sequence>
            //    <xsd:element name='Child1' minOccurs='1' maxOccurs='1'/>
            //    <xsd:element name='Child2' minOccurs='1' maxOccurs='1'/>
            //   </xsd:sequence>
            //  </xsd:complexType>
            // </xsd:element>
            //</xsd:schema>";
            //      XmlSchemaSet schemas = new XmlSchemaSet();
            //      schemas.Add("", XmlReader.Create(new StringReader(xsdMarkup)));

            //      XDocument doc1 = new XDocument(
            //          new XElement("Root",
            //              new XElement("Child1", "content1"),
            //              new XElement("Child2", "content1")
            //          )
            //      );


            //      XDocument doc2 = new XDocument(
            //          new XElement("Root",
            //              new XElement("Child1", "content1"),
            //              new XElement("Child3", "content1")
            //          )
            //      );

            //      Console.WriteLine("Validating doc1");
            //      bool errors = false;
            //      doc1.Validate(schemas, (o, e) =>
            //      {
            //          Console.WriteLine("{0}", e.Message);
            //          errors = true;
            //      });
            //      Console.WriteLine("doc1 {0}", errors ? "did not validate" : "validated");

            //      Console.WriteLine();
            //      Console.WriteLine("Validating doc2");
            //      errors = false;
            //      doc2.Validate(schemas, (o, e) =>
            //      {
            //          Console.WriteLine("{0}", e.Message);
            //          errors = true;
            //      });
            //      Console.WriteLine("doc2 {0}", errors ? "did not validate" : "validated");
            //      Console.WriteLine();

            //      using (var client = new WebClient())
            //      {
            //          var values = new NameValueCollection();
            //          var bytesXSD = Encoding.UTF8.GetBytes(xsdMarkup);
            //          var base64XSD = Convert.ToBase64String(bytesXSD);
            //          var bytesXML = Encoding.UTF8.GetBytes(doc2.ToString());
            //          var base64XML = Convert.ToBase64String(bytesXML);
            //          client.Headers.Set(HttpRequestHeader.ContentType, "application/json");

            //          String finalString = $"{{'xml':'{base64XML}', 'xsd':'{base64XSD}'}}";
            //          String aaaa = String.Empty;
            //          try
            //          {
            //              var response = client.UploadString("http://www.utilities-online.info/backend/xsd/validate", "POST", finalString);
            //          }
            //          catch (WebException ex)
            //          {
            //              using (var reader = new StreamReader(ex.Response.GetResponseStream()))
            //              {
            //                  aaaa = reader.ReadToEnd();
            //              }
            //          }

            //          Console.WriteLine(aaaa);
            //      }
        }

        public static bool IsValidXml(string xmlFilePath, string xsdFilePath)
        {
            var xdoc = XDocument.Load(xmlFilePath);
            var schemas = new XmlSchemaSet();
            schemas.Add(null, xsdFilePath);
            Boolean result = true;
            xdoc.Validate(schemas, (sender, e) =>
            {
                result = false;
            });

            return result;

        }



    }
}
