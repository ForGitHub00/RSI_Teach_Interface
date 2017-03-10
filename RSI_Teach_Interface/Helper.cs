using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RSI_Teach_Interface {
     public static class Helper {
        public static double GetValues(string strXML, string[] par) {
            XDocument xdoc = XDocument.Parse(strXML);
            if (par.Length == 3) {
                foreach (XElement phoneElement in xdoc.Element(par[0]).Elements(par[1])) {
                    XAttribute nameAttribute = phoneElement.Attribute(par[2]);
                    if (nameAttribute != null) {
                        return Convert.ToDouble(nameAttribute.Value.Replace('.', ','));
                    }
                }              
            } else if (par.Length == 2)  {
                foreach (XElement phoneElement in xdoc.Element(par[0]).Elements(par[1])) {
                    return Convert.ToDouble(phoneElement.Value.Replace('.', ','));              
                }
            }
            return 0;
        }
        public static void WriteToFile(string path, string data) {
            string[] temp = ReadFromFile(path);
            if (!temp.Contains(data)) {
                try {
                    using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default)) {
                        sw.WriteLine(data);
                    }
                } catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public static string[] ReadFromFile(string path) {
            List<string> result = new List<string>();
            try {
                using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default)) {
                    string line;
                    while ((line = sr.ReadLine()) != null) {
                        result.Add(line);
                    }
                }
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }

            return result.ToArray();
        }

        public static string SetValue(string strXML, string[] par, double val) {
            XDocument xdoc = XDocument.Parse(strXML);
            if (par.Length == 3) {
                foreach (XElement phoneElement in xdoc.Element(par[0]).Elements(par[1])) {
                    XAttribute nameAttribute = phoneElement.Attribute(par[2]);
                    if (nameAttribute != null) {
                        nameAttribute.Value = val.ToString().Replace(',', '.');
                    }
                }
            } else if (par.Length == 2) {
                foreach (XElement phoneElement in xdoc.Element(par[0]).Elements(par[1])) {
                    phoneElement.Value = val.ToString().Replace(',', '.');
                }
            }
            return xdoc.ToString();
        }
    }
}
