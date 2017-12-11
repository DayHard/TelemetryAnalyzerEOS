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
using static System.Math;


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
        private IList<string> _safefileNames;
        private IList<string> _safeFilePathes;
        #region Variable

        private const double  FokusKanal2 = 323.0;

        #endregion

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
                _safefileNames = openFileDialog.SafeFileNames;
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
            SetProperties(_safefileNames);
        }
        // Переключение локализации на французкий
        private void btnLangFr_Click(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
            Controls.Clear();
            InitializeComponent();
            SetProperties(_safefileNames);
        }
        // Переключение локализации на английский
        private void btnLangEng_Click(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            Controls.Clear();
            InitializeComponent();
            SetProperties(_safefileNames);
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
        // Создание отчета ошибки
        private void CreateReport(string path, string data)
        {
            using (var sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                sw.WriteLine("Отчет создан: " + DateTime.Now);
                sw.WriteLine("Файл: " + path);
                sw.WriteLine();
                sw.Write(data);
            }
        }
        // Создание файла отчет
        private void CreateReport(string path, string[] data)
        {
            using (var sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                sw.WriteLine("			Отчет об отработке сигналов установок ОЭД 'ПАНЦИРЬ - С1'.");
                sw.WriteLine();
                sw.WriteLine("Файл: " + path);
                sw.WriteLine();
                sw.WriteLine("Отчет создан: " + DateTime.Now);
                sw.WriteLine();
                sw.WriteLine("Прибор №: {0} ", data[8]);
                sw.WriteLine();
                sw.WriteLine("_____________________________________________________________");
                sw.WriteLine("|                  |         |                              | ");
                sw.WriteLine("|      Каналы      | № кадра | Макс. погрешность в пикселах |");
                sw.WriteLine("|                  |         |                              |");
                sw.WriteLine("|__________________|_________|______________________________|");
                sw.WriteLine("|                  |         |                              |");
                sw.WriteLine("|    Канал 1 по q  | {0}     | {1}                          |", data[0], data[1]);
                sw.WriteLine("|__________________|_________|______________________________|");
                sw.WriteLine("|                  |         |                              |");
                sw.WriteLine("|    Канал 1 по fi | {0}     | {1}                          |", data[2], data[3]);
                sw.WriteLine("|__________________|_________|______________________________|");
                sw.WriteLine("|                  |         |                              |");
                sw.WriteLine("|    Канал 2 по q  | {0}     | {1}                          |", data[4], data[5]);
                sw.WriteLine("|__________________|_________|______________________________|");
                sw.WriteLine("|                  |         |                              |");
                sw.WriteLine("|    Канал 2 по fi | {0}     | {1}                          |", data[6], data[7]);
                sw.WriteLine("|__________________|_________|______________________________|");
                sw.WriteLine("Время запаздывания отработки сигналов установок - 20 милисекунд.");
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
            var rightpackages = 0;
            // Создание массива результатов
            var report = new string[9];
            var numberPaketMaxWorkingOffStpChannel1Q = 0;
            var numberPaketMaxWorkingOffStpChannel1Fi = 0;
            var numberPaketMaxWorkingOffStpChannel2Q = 0;
            var numberPaketMaxWorkingOffStpChannel2Fi = 0;
            double tempDelta11 = 0;
            double tempDelta12 = 0;
            double tempDelta21 = 0;
            double tempDelta22 = 0;
            double maxDeltaSetupChannel1Q = 0;
            double maxDeltaSetupChannel1Fi = 0;
            double maxDeltaSetupChannel2Q = 0;
            double maxDeltaSetupChannel2Fi = 0;

            for (var i = 0; i < _decoder[k].ParamsCvses.Length - 2; i++)
            {
                if (_decoder[k].ParamsCvses[i].Launch && _decoder[k].ParamsCvses[i].Shod
                    && !_decoder[k].ParamsCvses[i].SoprLO1 && !_decoder[k].ParamsCvses[i].SoprLO2)
                {
                    // По каналу 1
                    if (_decoder[k].ParamsCvses[i].Deltaqk1 <11000 &&
                        _decoder[k].ParamsCvses[i].Deltafik1 < 14000 && 
                        i > 100 &&
                        _decoder[k].ParamsCvses[i + 1].Deltaqk1 < 11000 && 
                        _decoder[k].ParamsCvses[i + 1].Deltafik1 < 14000)
                    {
                        if (_decoder[k].ParamsCvses[i].Deltaqk1 != 0 &&
                            _decoder[k].ParamsCvses[i].Deltafik1 != 0)
                        {
                            var gradus = (double) _decoder[k].ParamsCvses[i].Deltaqk1 / 3600 * (PI / 180);
                            var pixelKanal1Q = _decoder[k].ParamsOedes[i].Focuspc * Tan(gradus) / (2 * 0.0083);

                            gradus = (double)_decoder[k].ParamsCvses[i].Deltafik1 / 3600 * (PI / 180);
                            var pixelKanal1Fi = _decoder[k].ParamsOedes[i].Focuspc * Tan(gradus) / (2 * 0.0086);

                            gradus = (double)_decoder[k].ParamsOedes[i + 1].Qkd1 / 3600 * (PI / 180);
                            var pixelKanal1Q1 = _decoder[k].ParamsOedes[i].Focuspc * Tan(gradus) / (2 * 0.0083);

                            gradus = (double)_decoder[k].ParamsOedes[i + 1].Fikd1 / 3600 * (PI / 180);
                            var pixelKanal1Fi1 = _decoder[k].ParamsOedes[i + 1].Focuspc * Tan(gradus) / (2 * 0.0086);

                            tempDelta11 = Abs(pixelKanal1Q - pixelKanal1Q1);
                            tempDelta12 = Abs(pixelKanal1Fi - pixelKanal1Fi1);
                        }
                    }
                    // По каналу 2
                    if (_decoder[k].ParamsCvses[i].Deltaqk2 < 1000 &&
                        _decoder[k].ParamsCvses[i].Deltafik2 < 2000 &&
                        i > 205 &&
                        _decoder[k].ParamsCvses[i + 1].Deltaqk2 < 1000 &&
                        _decoder[k].ParamsCvses[i + 1].Deltafik2 < 2000)
                    {
                        var gradus = (double)_decoder[k].ParamsCvses[i].Deltaqk2 / 3600 * (PI / 180);
                        var pixelKanal2Q = FokusKanal2 * Tan(gradus) / (4 * 0.0083);

                        gradus = (double)_decoder[k].ParamsCvses[i].Deltafik2 / 3600 * (PI / 180);
                        var pixelKanal2Fi = FokusKanal2 * Tan(gradus) / (4 * 0.0086);

                        gradus = (double)_decoder[k].ParamsOedes[i + 1].Qkd2 / 3600 * (PI / 180);
                        var pixelKanal2Q1 = FokusKanal2 * Tan(gradus) / (4 * 0.0083);

                        gradus = (double)_decoder[k].ParamsOedes[i + 1].Fikd2 / 3600 * (PI / 180);
                        var pixelKanal2Fi1 = FokusKanal2 * Tan(gradus) / (4 * 0.0086);

                        tempDelta21 = Abs(pixelKanal2Q - pixelKanal2Q1);
                        tempDelta22 = Abs(pixelKanal2Fi - pixelKanal2Fi1);
                    }
                    if (maxDeltaSetupChannel1Q < tempDelta11)
                        numberPaketMaxWorkingOffStpChannel1Q = i;
                    if (maxDeltaSetupChannel1Fi < tempDelta12)
                        numberPaketMaxWorkingOffStpChannel1Fi = i;
                    if (maxDeltaSetupChannel2Q < tempDelta21)
                        numberPaketMaxWorkingOffStpChannel2Q = i;
                    if (maxDeltaSetupChannel2Fi < tempDelta22)
                        numberPaketMaxWorkingOffStpChannel2Fi = i;

                    maxDeltaSetupChannel1Q = Max(maxDeltaSetupChannel1Q, tempDelta11);
                    maxDeltaSetupChannel1Fi = Max(maxDeltaSetupChannel1Fi, tempDelta12);
                    maxDeltaSetupChannel2Q = Max(maxDeltaSetupChannel2Q, tempDelta21);
                    maxDeltaSetupChannel2Fi = Max(maxDeltaSetupChannel2Fi, tempDelta22);

                    //Канал 1 по q
                    report[0] = numberPaketMaxWorkingOffStpChannel1Q.ToString();
                    report[1] = Round(Max(maxDeltaSetupChannel1Q, tempDelta11) / 10, 1).ToString(CultureInfo.InvariantCulture);
                    //Канал 1 по fi
                    report[2] = numberPaketMaxWorkingOffStpChannel1Fi.ToString();
                    report[3] = Round(Max(maxDeltaSetupChannel1Fi, tempDelta12) / 10, 1).ToString(CultureInfo.InvariantCulture);
                    // Канал 2 по q
                    report[4] = numberPaketMaxWorkingOffStpChannel2Q.ToString();
                    report[5] = Round(Max(maxDeltaSetupChannel2Q, tempDelta21), 1).ToString(CultureInfo.InvariantCulture);
                    // Канал 2 по fi
                    report[6] = numberPaketMaxWorkingOffStpChannel2Fi.ToString();
                    report[7] = Round(Max(maxDeltaSetupChannel2Fi, tempDelta22), 1).ToString(CultureInfo.InvariantCulture);
                    //Номер прибора
                    report[8] = _decoder[k].ParamsOedes[0].NumberDevice.ToString();
                    rightpackages++;
                }
                else if (_decoder[k].ParamsCvses[i].Launch && _decoder[k].ParamsCvses[i].Shod
                         && _decoder[k].ParamsCvses[i].SoprLO1 && _decoder[k].ParamsCvses[i].SoprLO2)
                {
                    // Включить в релизной версии
                    CreateReport(_safeFilePathes[k] + ".txt", "Ошибка. Файл не предназначен для данного типа проверок.");
                    return false;
                }
            }

            CreateReport(_safeFilePathes[k] + ".txt", report);
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

        #region FileOpenResult

        private void btnResultN_Click(object sender, EventArgs e)
        {
            OpenFile(0);
        }

        private void OpenFile(int lauchN)
        {
            if (File.Exists(_safeFilePathes[lauchN]))
                System.Diagnostics.Process.Start(_safeFilePathes[lauchN] + ".txt");
            else MessageBox.Show(@"File not found");
        }

        private void btnResultN1_Click(object sender, EventArgs e)
        {
            OpenFile(1);
        }

        private void btnResultN2_Click(object sender, EventArgs e)
        {
            OpenFile(2);
        }

        private void btnResultN3_Click(object sender, EventArgs e)
        {
            OpenFile(3);
        }

        private void btnResultN4_Click(object sender, EventArgs e)
        {
            OpenFile(4);
        }

        private void btnResultN5_Click(object sender, EventArgs e)
        {
            OpenFile(5);
        }

        private void btnResultN6_Click(object sender, EventArgs e)
        {
            OpenFile(6);
        }

        private void btnResultN7_Click(object sender, EventArgs e)
        {
            OpenFile(7);
        }

        private void btnResultN8_Click(object sender, EventArgs e)
        {
            OpenFile(8);
        }

        private void btnResultN9_Click(object sender, EventArgs e)
        {
            OpenFile(9);
        }

        private void btnResultN10_Click(object sender, EventArgs e)
        {
            OpenFile(10);
        }

        private void btnResultN11_Click(object sender, EventArgs e)
        {
            OpenFile(11);
        }

        private void btnResultN12_Click(object sender, EventArgs e)
        {
            OpenFile(12);
        }

        private void btnResultN13_Click(object sender, EventArgs e)
        {
            OpenFile(13);
        }

        private void btnResultSelfDiag_Click(object sender, EventArgs e)
        {
            OpenFile(14);
        }
#endregion
    }
}
