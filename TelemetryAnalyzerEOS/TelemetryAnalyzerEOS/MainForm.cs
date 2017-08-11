using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace TelemetryAnalyzerEOS
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            LoadConfiguration();
            InitializeComponent();
            LoadFiles();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            lbVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        // Обновление файла конфигурации
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            UpdateConfiguration();
        }
        // Загрузка файла setting для установки текущей культуры
        private void LoadConfiguration()
        {
            if (File.Exists("setting.xml"))
            {
                XDocument xDoc = XDocument.Load("setting.xml");
                XElement xLang = xDoc.Descendants("Language").First();
                ChooseLanguage(xLang.Value);
            }
            else
            {
                ChooseLanguage(CultureInfo.CurrentCulture.Name);
            }
        }
        //Выбор текущего языка
        private void ChooseLanguage(string lang)
        {
            switch (lang)
            {
                case "ru-RU":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
                    break;
                case "ru-BY":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
                    break;
                case "en-US":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                    break;
                case "fr-FR":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
                    break;
                default:
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-BY");
                    break;
            }
        }
        // Обновление файла конфигурации
        private void UpdateConfiguration()
        {
            XmlTextWriter writer = new XmlTextWriter("setting.xml", Encoding.UTF8);
            writer.WriteStartDocument();
            writer.WriteStartElement("xml");
            writer.WriteEndElement();
            writer.Close();

            XmlDocument doc = new XmlDocument();
            doc.Load("setting.xml");
            XmlNode element = doc.CreateElement("OESDataDownloader");
            doc.DocumentElement?.AppendChild(element);

            XmlNode languageEl = doc.CreateElement("Language"); // даём имя
            languageEl.InnerText = Thread.CurrentThread.CurrentUICulture.ToString(); // и значение
            element.AppendChild(languageEl); // и указываем кому принадлежит

            doc.Save("setting.xml");
        }
        // Загрузка файла с данными
        private void LoadFiles()
        {
            openFileDialog.FileName = null;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Decoder[] decoder = new Decoder[openFileDialog.FileNames.Length];
                for (int i = 0; i < openFileDialog.FileNames.Length; i++)
                {      
                    decoder[i] = new Decoder();
                    if (!decoder[i].Open(openFileDialog.FileNames[i]))
                        MessageBox.Show(@"Can`t open file", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                    if(!decoder[i].Decode())
                        MessageBox.Show(@"Can`t decode file", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        // Переключение локализации на русский
        private void btnLangRus_Click(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
            Controls.Clear();
            InitializeComponent();
        }
        // Переключение локализации на французкий
        private void btnLangFr_Click(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
            Controls.Clear();
            InitializeComponent();
        }
        // Переключение локализации на английский
        private void btnLangEng_Click(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            Controls.Clear();
            InitializeComponent();
        }

    }
}
