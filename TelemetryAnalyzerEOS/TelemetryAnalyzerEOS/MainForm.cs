using System;
using System.Collections.Generic;
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
        private Decoder[] _decoder;
        private IList<string> _fileNames;
        private List<ComboBox> _cblist;
        private List<Button> _btnlist;
        public MainForm()
        {
            // Установка текущей UI культуры приложения
            LoadConfiguration();
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            lbVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            //Загрузка файлов, выбранных пользователем
            if (!LoadFiles())
                Close();
        }

        // Установка параметров Label, ComboBox, PictureBox, Button
        private void SetProperties(IList<string> safeFileNames)
        {
            lbLaunchN.Visible = true;

            var lblist = new List<Label>
            {
                lbLaunchN,
                lbLaunchN1,
                lbLaunchN2,
                lbLaunchN3,
                lbLaunchN4,
                lbLaunchN5,
                lbLaunchN6,
                lbLaunchN7,
                lbLaunchN8,
                lbLaunchN9,
                lbLaunchN10,
                lbLaunchN11,
                lbLaunchN12,
                lbLaunchN13,
                lbSelfDiagnosis
            };

            for (int i = 0; i < safeFileNames.Count; i++)
            {
               lblist[i].Text = safeFileNames[i] + @" :";
               lblist[i].Visible = true;
            }

            _cblist = new List<ComboBox>
            {
                cbLaunchN,
                cbLaunchN1,
                cbLaunchN2,
                cbLaunchN3,
                cbLaunchN4,
                cbLaunchN5,
                cbLaunchN6,
                cbLaunchN7,
                cbLaunchN8,
                cbLaunchN9,
                cbLaunchN10,
                cbLaunchN11,
                cbLaunchN12,
                cbLaunchN13,
                cbSelfDiagnosis
            };

             for (int i = 0; i < safeFileNames.Count; i++)
                 _cblist[i].Visible = true;

            var pblist = new List<PictureBox>
            {
                pbStatusN,
                pbStatusN1,
                pbStatusN2,
                pbStatusN3,
                pbStatusN4,
                pbStatusN5,
                pbStatusN6,
                pbStatusN7,
                pbStatusN8,
                pbStatusN9,
                pbStatusN10,
                pbStatusN11,
                pbStatusN12,
                pbStatusN13,
                pbStatusSelfDiagnosis
            };

            for (int i = 0; i < safeFileNames.Count; i++)
                pblist[i].Visible = true;

            _btnlist = new List<Button>
            {
                btnResultN,
                btnResultN1,
                btnResultN2,
                btnResultN3,
                btnResultN4,
                btnResultN5,
                btnResultN6,
                btnResultN7,
                btnResultN8,
                btnResultN9,
                btnResultN10,
                btnResultN11,
                btnResultN12,
                btnResultN13,
                btnResultSelfDiag
            };

            for (int i = 0; i < safeFileNames.Count; i++)
                _btnlist[i].Visible = true;

            Height = 135 + safeFileNames.Count * 31;
        }

        // Обновление файла конфигурации
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            UpdateConfiguration();
        }
        // Загрузка файла setting для установки текущей культуры
        private static void LoadConfiguration()
        {
            if (File.Exists("setting.xml"))
            {
                var xDoc = XDocument.Load("setting.xml");
                var xLang = xDoc.Descendants("Language").First();
                ChooseLanguage(xLang.Value);
            }
            else
            {
                ChooseLanguage(CultureInfo.CurrentCulture.Name);
            }
        }
        //Выбор текущего языка
        private static void ChooseLanguage(string lang)
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
        private static void UpdateConfiguration()
        {
            var writer = new XmlTextWriter("setting.xml", Encoding.UTF8);
            writer.WriteStartDocument();
            writer.WriteStartElement("xml");
            writer.WriteEndElement();
            writer.Close();

            var doc = new XmlDocument();
            doc.Load("setting.xml");
            var element = doc.CreateElement("OESDataDownloader");
            doc.DocumentElement?.AppendChild(element);

            var languageEl = doc.CreateElement("Language"); // даём имя
            languageEl.InnerText = Thread.CurrentThread.CurrentUICulture.ToString(); // и значение
            element.AppendChild(languageEl); // и указываем кому принадлежит

            doc.Save("setting.xml");
        }
        // Загрузка файла с данными
        private bool LoadFiles()
        {
            openFileDialog.FileName = null;
            if (openFileDialog.ShowDialog() == DialogResult.OK && openFileDialog.FileNames.Length <= 15)
            {

                _decoder = new Decoder[openFileDialog.FileNames.Length];
                for (int i = 0; i < openFileDialog.FileNames.Length; i++)
                {
                    _decoder[i] = new Decoder();

                    if (!_decoder[i].Open(openFileDialog.FileNames[i]))
                        MessageBox.Show(@"Can`t open file " + openFileDialog.FileNames[i], @"Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (!_decoder[i].Decode())
                        MessageBox.Show(@"Can`t decode file " + openFileDialog.FileNames[i], @"Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // Настройка видимости Label, ComboBox, PictureBox и Button согласно количеству загруженных файлов
                _fileNames = openFileDialog.SafeFileNames;
                SetProperties(_fileNames);
                if (_decoder.Length != 0)
                    btnStartAnalyze.Enabled = true;
            }
            else 
            {
                // Если пользователем выбрано больше 15 файлов
                // Запрос на повтор выбора файла или закрытие приложения
                if (MessageBox.Show(
                        @"Некорректное количество выбранных файлов. Количество файлов не может быть равным 0 и превышать 15. Попробовать снова?",
                        @"Ошибка", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning) == DialogResult.Retry)
                    LoadFiles();
                else return false;
            }
            return true;
        }
        // Переключение локализации на русский
        private void btnLangRus_Click(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
            Controls.Clear();
            InitializeComponent();
            SetProperties(_fileNames);
        }
        // Переключение локализации на французкий
        private void btnLangFr_Click(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
            Controls.Clear();
            InitializeComponent();
            SetProperties(_fileNames);
        }
        // Переключение локализации на английский
        private void btnLangEng_Click(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            Controls.Clear();
            InitializeComponent();
            SetProperties(_fileNames);
        }
        //Тестирование, согласно установленным параметрам
        private void btnStartAnalyze_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < _decoder.Length; i++)
            {
                switch (_cblist[i].SelectedIndex.ToString())
                {
                    case "0": 
                        GeneralHealthCheck(i);
                        break;
                    case "1":
                        CheckingTheTuningOfThePlantSignals(i);
                        break;
                    case "2":
                        CheckingTheAccumulationTime(i);
                        break;
                    case "3":
                        CheckingTheCaptureAndRecaptureTime(i);
                        break;
                    case "4":
                        DiagnosticResultsAnalysis(i);
                        break;
                }
            }
        }

        //Проверка общей работоспособности
        private void GeneralHealthCheck(int possition)
        {
            
        }
        //Проверка отработки сигналов установок
        private void CheckingTheTuningOfThePlantSignals(int possition)
        {
            for (int i = 0; i < _decoder[possition].ParamsCvses.Length; i++)
            {
                if (_decoder[possition].ParamsCvses[i].Launch && _decoder[possition].ParamsCvses[i].Shod
                    && !_decoder[possition].ParamsCvses[i].SoprLO1 && !_decoder[possition].ParamsCvses[i].SoprLO2)
                {
                    
                }                
            }
        }
        //Проверка времени накопления
        private void CheckingTheAccumulationTime(int possition)
        {
            
        }
        //Проверка времени захвата и перезахвата
        private void CheckingTheCaptureAndRecaptureTime(int possition)
        {
            
        }
        //Анализ результатов диагностики
        private void DiagnosticResultsAnalysis(int possition)
        {
            
        }
    }
}
