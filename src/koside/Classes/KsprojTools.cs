using System;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace koside.Classes
{
    class KsprojTools
    {
        static private decimal DefVersion = 0.1M;
        static private decimal MinVersion = 0.5M;
        static private Version curVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

        static public string ProjectFile(string name, string ksprojname, string folder, bool? makeDir, System.Collections.Generic.List<string> title, System.Collections.Generic.List<string> file_name, int index, string install, string ksprojpath)
        {
            // Name = Project Name. ksprojname = .ksproj file name. Folder = root folder. Title = tab titles. File_name = file_name. index =  selected index. install = current install.
            string path;
            XmlTextWriter writer;
            int i = 0;
            if (ksprojname == "Default")
                ksprojname = name;
            if (makeDir != null)
            {
                if (makeDir == true)
                {
                    path = System.IO.Path.Combine(folder, ksprojname);
                }
                else
                {
                    path = folder;
                }
            }
            else
                path = ksprojpath;
            if(!path.Contains(".ksproj"))
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);

            if (!path.Contains(".ksproj"))
            {
                writer = new XmlTextWriter(System.IO.Path.Combine(path, ksprojname + ".ksproj"), Encoding.UTF8);
            }
            else
            {
                writer = new XmlTextWriter(System.IO.Path.Combine(path), Encoding.UTF8);
            }
            writer.WriteStartDocument(true);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;
            writer.WriteStartElement("ksproj");
                writer.WriteStartElement("metadata");
                    writer.WriteStartElement("projname");
                    writer.WriteString(name);
                    writer.WriteEndElement();
                    writer.WriteStartElement("rootdir");
                    if (!path.Contains(".ksproj"))
                        writer.WriteString(path);
                    else
                        writer.WriteString(System.IO.Path.GetDirectoryName(path));
                    writer.WriteEndElement();
                    writer.WriteStartElement("formatversion");
                    writer.WriteString(DefVersion.ToString());
                    writer.WriteEndElement();
                    writer.WriteStartElement("kodeversion");
                    writer.WriteString(curVersion.ToString());
                    writer.WriteEndElement();
                    writer.WriteStartElement("minimumversion");
                    writer.WriteString(MinVersion.ToString());
                    writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteStartElement("settings");
                    writer.WriteStartElement("selectedindex");
                    if(index == -1)
                        writer.WriteString("0");
                    else
                        writer.WriteString(index.ToString());
                    writer.WriteEndElement();
                    writer.WriteStartElement("selectedinstall");
                    writer.WriteString(install);
                    writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteStartElement("tabs");
                if (index == -1 && title == null && file_name == null)
                {
                    writer.WriteStartElement("tab");
                    writer.WriteStartElement("index");
                    writer.WriteString("0");
                    writer.WriteEndElement();
                    writer.WriteStartElement("title");
                    writer.WriteString(ksprojname + ".ks         X");
                    writer.WriteEndElement();
                    writer.WriteStartElement("file_name");
                    writer.WriteString(System.IO.Path.Combine(path, ksprojname + ".ks"));
                    writer.WriteEndElement();
                    writer.WriteEndElement();

                    using (var file = System.IO.File.Create(System.IO.Path.Combine(path, ksprojname + ".ks")))
                    {

                    }
                }
                else
                {
                    foreach (string s in title)
                    {
                        writer.WriteStartElement("tab");
                        writer.WriteStartElement("index");
                        writer.WriteString(i.ToString());
                        writer.WriteEndElement();
                        writer.WriteStartElement("title");
                        writer.WriteString(s);
                        writer.WriteEndElement();
                        writer.WriteStartElement("file_name");
                        writer.WriteString(file_name[i]);
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        i++;
                    }
                }
                writer.WriteEndElement();
                writer.WriteStartElement("libraries");
                writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();

            if (!path.Contains(".ksproj"))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(path, "Libraries"));
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(path, "NoExport"));
                return System.IO.Path.Combine(path, ksprojname + ".ksproj");
            }else
                return path;
        }
    }
}
