using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace TelemetryAnalyzerEOS
{
    public partial class MainForm : Form
    {
        private delegate void TaskStatusChanged();
        private event TaskStatusChanged TaskIsComplite;
        private  LoadingForm _loadingForm;

        private Decoder[] _decoder;
        private List<ComboBox> _cblist;
        private List<Button> _btnlist;
        private List<PictureBox> _pblist;
        private IList<string> _safeFileNames;
        private IList<string> _safeFilePathes;

        private readonly string[] _cbstatus = new string[15];

        public MainForm()
        {
            // Установка текущей UI культуры приложения
            LoadConfiguration();
            //Инициализация GUI
            InitializeComponent();
            // Подпись на событие завершения потока обработки
            TaskIsComplite += TaskStatus;
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            // Получение текущей версии ПО
            lbVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        //Загрузка файлов
        private void MainForm_Shown(object sender, EventArgs e)
        {
            //Загрузка файлов, выбранных пользователем
            if (!LoadFiles())
                Close();
        }
        // Установка параметров Label, ComboBox, PictureBox, Button
        private void SetProperties(IList<string> safeFileNames)
        {

            // Создание списка Label 
            // установка видимости , согласно количеству загруженных файлов
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

            // Создание списка ComboBox 
            // установка видимости , согласно количеству загруженных файлов
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

            // Создание списка PictureBox 
            _pblist = new List<PictureBox>
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
                _pblist[i].Visible = true;
            
            // Создание списка Button, 
            // установка видимости кнопок согласно количеству загруженных файлов
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

            //!!! FOR DEBUGING
            foreach (ComboBox t in _cblist)
            {
                t.SelectedIndex = 1;
            }

            // Установка размеров формы, в соответствии с количеством выбранных файлов
            Height = 135 + safeFileNames.Count * 31;
            btnStartAnalyze.Enabled = true;
        }
        // Обновление файла конфигурации
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Обновление файла конфигурации языков
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
            if (openFileDialog.ShowDialog() == DialogResult.OK && openFileDialog.FileNames.Length <= 15)
            {
                SetProperties(openFileDialog.SafeFileNames);
                _safeFileNames = openFileDialog.SafeFileNames;
                _safeFilePathes = openFileDialog.FileNames;
            }
            else 
            {
                // Если пользователем выбрано больше 15 файлов
                // Запрос на повтор выбора файла или закрытие приложения
                if (MessageBox.Show(
                        @"Некорректное количество выбранных файлов. Количество файлов не может быть равным 0 и превышать 15. Попробовать снова?",
                        @"Внимание", MessageBoxButtons.RetryCancel, MessageBoxIcon.Information) == DialogResult.Retry)
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
            SetProperties(_safeFileNames);
        }
        // Переключение локализации на французкий
        private void btnLangFr_Click(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
            Controls.Clear();
            InitializeComponent();
            SetProperties(_safeFileNames);
        }
        // Переключение локализации на английский
        private void btnLangEng_Click(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            Controls.Clear();
            InitializeComponent();
            SetProperties(_safeFileNames);
        }
        //Тестирование, согласно установленным параметрам
        private void btnStartAnalyze_Click(object sender, EventArgs e)
        {
            // Выделение памяти, согласно количеству загруженных файлов
            // Класс отвечает за Открытие\Декодирование файлов
            _decoder = new Decoder[openFileDialog.FileNames.Length];

            // Инициализация класса, для отображения прогресс бара
            _loadingForm = new LoadingForm
            {
                Owner = this,
                StartPosition = StartPosition
            };
                _loadingForm.Show();

            // Установка максимального значения прогресс бара, согласно количеству файлов
            _loadingForm.pbAnalysing.Maximum = _decoder.Length;
            // Отключение основных формы, для невозможности вмешательства в процесс декодирования
            gbLoadedFileList.Enabled = false;
            btnStartAnalyze.Enabled = false;
            btnLangRus.Enabled = false;
            btnLangFr.Enabled = false;
            btnLangEng.Enabled = false;

            // Создание тасков для декодирования (иначе подвисает форма)
            for (int i = 0; i < openFileDialog.FileNames.Length; i++)
            {
                Task.Factory.StartNew(DataAnalysis,i);
            }  
        }
        //Загрузка, декодирование и анализ данных
        private void DataAnalysis(object possition)
        {
            var i = (int) possition;
            // Проверка, выбран ли тест
            if (_cbstatus[i] != "-1")
            {
                // Инициализация декодера для текущего файла
                _decoder[i] = new Decoder();
                // Открытие файла и считывание его в глобальный массив
                _decoder[i].Open(openFileDialog.FileNames[i]);
                // Декодирование файла 
                _decoder[i].Decode();
                // Запуск теста в соответсвии с выбранный в ComboBox
                switch (_cbstatus[i])
                {
                    case "0":
                        if (GeneralHealthCheck(i))
                             SetSuccedImage(i);
                        else SetFailImage(i);
                        break;
                    case "1":
                        if (CheckingTheTuningOfThePlantSignals(i))
                            SetSuccedImage(i);
                        else SetFailImage(i);
                        break;
                    case "2":
                        CheckingTheAccumulationTime();
                        break;
                    case "3":
                        CheckingTheCaptureAndRecaptureTime();
                        break;
                    case "4":
                        DiagnosticResultsAnalysis();
                        break;
                    default:
                        throw new Exception("ComboBox switch exception.");
                }
            }

            // Вызов события завершения работы потока (необходимо для работы прогесс бара)
            TaskIsComplite?.Invoke();
        }

        private static void CreateReport(string path, string data)
        {
            using (var sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                sw.Write(data);
            }
        } 
       /// <summary>
       /// Установка изображения не удачно.
       /// </summary>
       /// <param name="i">Номер файла.</param>
        private void SetFailImage(int i)
        {
            Invoke(new MethodInvoker(delegate
            {
                _pblist[i].Image = Properties.Resources.Status_Fail;
                _btnlist[i].Enabled = true;
            }));
        }
        /// <summary>
        /// Установка изображения успех.
        /// </summary>
        /// <param name="i">Номер файла.</param>
        private void SetSuccedImage(int i)
        {
            Invoke(new MethodInvoker(delegate
            {
                _pblist[i].Image = Properties.Resources.Status_Success;
                _btnlist[i].Enabled = true;
            }));
        }
        // Метод изменения статуса прогресс бара
        /// <summary>
        /// Метод, вызываеммый событием ThreadIsComplite. Необходим для корректной отработки GUI.
        /// </summary>
        private void TaskStatus()
        {
            // Метод работает в массиве потоков, для доступа к GUI Invoke обязателен
            Invoke(new MethodInvoker(delegate
            {
                _loadingForm.pbAnalysing.PerformStep();
                if (_loadingForm.pbAnalysing.Value != _decoder.Length) return;
                // Включение формы, по завершению работы потоков декодера
                gbLoadedFileList.Enabled = true;
                btnStartAnalyze.Enabled = true;
                btnLangRus.Enabled = true;
                btnLangFr.Enabled = true;
                btnLangEng.Enabled = true;
                // Скрытие формы с прогресс баром Так же установка значения прогресс бара в 0
                _loadingForm.Hide();
                // Установка значения прогресс бара в 0
                _loadingForm.pbAnalysing.Value = 0;
            }));
        }

        //Проверка общей работоспособности
        private static bool GeneralHealthCheck(int k)
        {
            Thread.Sleep(2000);
            return true;
        }
        //Проверка отработки сигналов установок
        private bool CheckingTheTuningOfThePlantSignals(int k)
        {
            int rightpackages = 0;

            foreach (var paramsCvse in _decoder[k].ParamsCvses)
            {
                if (paramsCvse.Launch && paramsCvse.Shod
                    && !paramsCvse.SoprLO1 && !paramsCvse.SoprLO2)
                {   
                    // Указать какой диапазон!
                    if (true)
                    {
                        //paramsCvse.
                    }
                    rightpackages++;
                }else if (paramsCvse.Launch && paramsCvse.Shod
                          && paramsCvse.SoprLO1 && paramsCvse.SoprLO2)
                {
                    CreateReport(_safeFilePathes[k] + ".txt","Ошибка. Файл не предназначен для данного типа проверок.");
                    return false;
                }
            }
            return rightpackages != 0;
        }
        //Проверка времени накопления
        private void CheckingTheAccumulationTime()
        {
            
        }
        //Проверка времени захвата и перезахвата
        private void CheckingTheCaptureAndRecaptureTime()
        {
            
        }
        //Анализ результатов диагностики
        private void DiagnosticResultsAnalysis()
        {
            
        }
        // Считывание состояния элементов GUI
        private void timerGUI_Tick(object sender, EventArgs e)
        {
            if(_cblist != null)
            for (int i = 0; i < _cblist.Count; i++)
            {
                _cbstatus[i] = _cblist[i].SelectedIndex.ToString();
            }
        }

        private void btnResultN_Click(object sender, EventArgs e)
        {
            //CreateReport("1.txt", "Error");
            if (File.Exists(@"1.txt"))
                System.Diagnostics.Process.Start(@"1.txt");
            else MessageBox.Show(@"File not found");
        }
    }
}
