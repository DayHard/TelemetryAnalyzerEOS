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
        #region Variable
        private delegate void TaskStatusChanged();
        private event TaskStatusChanged TaskIsComplite;
        private  LoadingForm _loadingForm;

        private Decoder[] _decoder;
        private List<ComboBox> _cblist;
        private List<Button> _btnlist;
        private List<PictureBox> _pblist;
        private IList<string> _safefileNames;
        private IList<string> _safeFilePathes;

        private const double  FokusKanal2 = 323.0;
        private readonly string[] _cbstatus = new string[15];

        #endregion

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
                t.SelectedIndex = 3;
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
                        if(CheckingTheAccumulationTime(i))
                            SetSuccedImage(i);
                        else SetFailImage(i);
                        break;
                    case "3":
                        if(CheckingTheCaptureAndRecaptureTime(i))
                            SetSuccedImage(i);
                        else SetFailImage(i);
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
        /// <summary>
        /// Создание отчета
        /// </summary>
        /// <param name="path"> Пусть к файлу отчета</param>
        /// <param name="data"> Текст отчета</param>
        private static void CreateReport(string path, string data)
        {
            using (var sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                sw.WriteLine("Отчет создан: " + DateTime.Now);
                sw.WriteLine("Файл: " + path);
                sw.WriteLine();
                sw.Write(data);
            }
        }

        /// <summary>
        /// Установка изображения "Failed"
        /// </summary>
        /// <param name="i">Номер файла</param>
        private void SetFailImage(int i)
        {
            Invoke(new MethodInvoker(delegate
            {
                _pblist[i].Image = Properties.Resources.Status_Fail;
                _btnlist[i].Enabled = true;
            }));
        }
        /// <summary>
        /// Установка изображения "Success"
        /// </summary>
        /// <param name="i">Номер файла</param>
        private void SetSuccedImage(int i)
        {
            Invoke(new MethodInvoker(delegate
            {
                _pblist[i].Image = Properties.Resources.Status_Success;
                _btnlist[i].Enabled = true;
            }));
        }
        // Метод, вызываеммый событием ThreadIsComplite. Сброс прогресс бара. Необходим для корректной отработки GUI. 
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

        #region Тесты

        /// <summary>
        /// Тест №1: Проверка общей работоспособности
        /// </summary>
        /// <param name="k">Номер файла</param>
        /// <returns></returns>
        private bool GeneralHealthCheck(int k)
        {
            var fvForm = new FocusValueForm();
            fvForm.ShowDialog();
            var focus = fvForm.FocusValue;
            if (focus < 300 || focus > 400)
            {
                CreateReport(_safeFilePathes[k] + ".txt", "Ошибка. Значение фокуса введено не верно.");
                return false;
            }

            //Создание массива отчета
            var report = new string[32];
            report[0] = _decoder[k].NumberDevice.ToString();

            #region Таблица 1                     
            double speedDelta;
            int numberPaketStartKanal1 = 0, numberPaketStartKanal2 = 0,
                numberPaketFinishKanal1 = 0, numberPaketFinishKanal2 = 0,
                idxLo1 = 0, idxLo2Uz = 0;
            var neprerOldLo1 = 0;
            var neprerOldLo2Uz = 0;
            var numPkLo1 = new ushort[10001];
            var numPkLo2Uzk = new ushort[10001];

            // Канал 1
            for (int i = 0; i < _decoder[k].ParamsOedes.Length; i++)
            {
                if (!_decoder[k].ParamsOedes[i].IndLo1) continue;
                var neprerNewLo1 = i;
                if (neprerNewLo1 == neprerOldLo1 + 1)//будет отображать установленные флаги если они были выставлены минимум 2 такта подряд
                {
                    numPkLo1[idxLo1] = (ushort)i;
                    idxLo1++;
                }
                neprerOldLo1 = neprerNewLo1;

                if (numberPaketStartKanal1 == 0)
                {
                    numberPaketStartKanal1 = i;                    //номер пакета в котором впервые появляется INDLO1
                }
                numberPaketFinishKanal1 = i - 1; //(-1 для подгона)
            }

            var numPkBeginLo1 = numPkLo1[0] - 1;//"-1" нужно для взятия первого пакета в кот. устан. флаг, т.к. в вычислении номера этого пакета из-за условия он увеличивается на 1
            var numPkEndLo1 = numPkLo1[idxLo1 - 2]; // ПОДГОН! (было -1)

            if (_decoder[k].ParamsOedes[numPkBeginLo1].FrameNumber > 
                _decoder[k].ParamsOedes[numberPaketStartKanal1].FrameNumber)
            {
                numPkBeginLo1 = numberPaketStartKanal1;
            }

            double speed = 0;
            for (int iii = 1; iii < 10; iii++)
            {

                var q1K1 = _decoder[k].ParamsOedes[numPkBeginLo1 + iii].Qkd1 +
                            _decoder[k].ParamsOedes[numPkBeginLo1 + iii].Qkk1;
                var q0K1 = _decoder[k].ParamsOedes[numPkBeginLo1 + iii - 1].Qkd1 +
                            _decoder[k].ParamsOedes[numPkBeginLo1 + iii - 1].Qkk1;
                var fi1K1 = _decoder[k].ParamsOedes[numPkBeginLo1 + iii].Fikd1 +
                         _decoder[k].ParamsOedes[numPkBeginLo1 + iii].Fikk1;
                var fi0K1 = _decoder[k].ParamsOedes[numPkBeginLo1 + iii - 1].Fikd1 +
                         _decoder[k].ParamsOedes[numPkBeginLo1 + iii - 1].Fikk1;
                speedDelta = Sqrt(Pow(q1K1 - q0K1, 2) + Pow(fi1K1 - fi0K1, 2));
                if (speed < speedDelta) speed = speedDelta;
            }
            speed = speed * 50 / 3600;

            // Массив отчета канал 1
            report[1] = numPkBeginLo1.ToString();
            report[2] = Round(speed,1).ToString(CultureInfo.CurrentCulture);
            report[3] = _decoder[k].ParamsOedes[numPkBeginLo1].Yc1.ToString();
            report[4] = _decoder[k].ParamsOedes[numPkBeginLo1].Yc2.ToString();
            report[5] = numPkEndLo1.ToString();
            report[6] = @"0.0";
            report[7] = _decoder[k].ParamsOedes[numPkEndLo1].Yc1.ToString();
            report[8] = _decoder[k].ParamsOedes[numPkEndLo1].Yc2.ToString();


            // Канал 2

            for (int i = 0; i < _decoder[k].ParamsOedes.Length; i++)
            {
                if (!_decoder[k].ParamsOedes[i].IndLo2) continue;
                var neprerNewLo2Uz = i;
                if (neprerNewLo2Uz == neprerOldLo2Uz + 1)//будет отображать установленные флаги если они были выставлены минимум 2 такта подряд
                {
                    numPkLo2Uzk[idxLo2Uz] = (ushort)i;
                    idxLo2Uz++;
                }
                neprerOldLo2Uz = neprerNewLo2Uz;

                if (numberPaketStartKanal2 == 0)
                {
                    numberPaketStartKanal2 = i;                    //номер пакета в котором впервые появляется INDLO1
                }
                numberPaketFinishKanal2 = i; //(-1 для подгона)
            }

            var numPkBeginLo2Uz = numPkLo2Uzk[0] - 1;//"-1" нужно для взятия первого пакета в кот. устан. флаг, т.к. в вычислении номера этого пакета из-за условия он увеличивается на 1
            var numPkEndLo2Uz = numPkLo2Uzk[idxLo2Uz - 2]; // ПОДГОН! (было -1)

            if (_decoder[k].ParamsOedes[numPkBeginLo2Uz].FrameNumber >
                _decoder[k].ParamsOedes[numberPaketStartKanal2].FrameNumber)
            {
                numPkBeginLo2Uz = numberPaketStartKanal2;
            }

            speed = 0;
            for (int iii = 1; iii < 10; iii++)
            {


                var q1K2 = _decoder[k].ParamsOedes[numPkBeginLo2Uz + iii].Qkd2 +
                        _decoder[k].ParamsOedes[numPkBeginLo2Uz + iii].Qkk2;
                var q0K2 = _decoder[k].ParamsOedes[numPkBeginLo2Uz + iii - 1].Qkd2 +
                        _decoder[k].ParamsOedes[numPkBeginLo2Uz + iii - 1].Qkk2;
                var fi1K2 = _decoder[k].ParamsOedes[numPkBeginLo2Uz + iii].Fikd2 +
                         _decoder[k].ParamsOedes[numPkBeginLo2Uz + iii].Fikk2;
                var fi0K2 = _decoder[k].ParamsOedes[numPkBeginLo2Uz + iii - 1].Fikd2 +
                         _decoder[k].ParamsOedes[numPkBeginLo2Uz + iii - 1].Fikk2;
                speedDelta = Sqrt(Pow(q1K2 - q0K2, 2) + Pow(fi1K2 - fi0K2, 2));
                if (speed < speedDelta) speed = speedDelta;
            }
            speed = speed * 50 / 3600;

            // Массив отчета канал 2
            report[9] = numPkBeginLo2Uz.ToString();
            report[10] = Round(speed, 1).ToString(CultureInfo.CurrentCulture);
            report[11] = _decoder[k].ParamsOedes[numPkBeginLo2Uz].Yc1.ToString();
            report[12] = _decoder[k].ParamsOedes[numPkBeginLo2Uz].Yc2.ToString();
            report[13] = numPkEndLo2Uz.ToString();
            report[14] = @"0.0";
            report[15] = _decoder[k].ParamsOedes[numPkEndLo2Uz].Yc1.ToString();
            report[16] = _decoder[k].ParamsOedes[numPkEndLo2Uz].Yc2.ToString();
            #endregion

            #region Таблица 2 (Таблица размера полей анализа)

           // Вычисление параментров первой строки 
            if (_decoder[k].ParamsOedes[numPkBeginLo1].FrameNumber >
                _decoder[k].ParamsOedes[numberPaketStartKanal1].FrameNumber)
            {
                numPkBeginLo1 = numberPaketStartKanal1;
            }

            // Отчет начинается с элемента 17
            // Строка 1 таблицы
            report[17] = _decoder[k].ParamsOedes[numPkBeginLo1].FrameNumber.ToString();
            report[18] = ((double)_decoder[k].ParamsOedes[numPkBeginLo1].Focuspc / 10).ToString(CultureInfo.CurrentCulture);
            report[19] = Round((double)_decoder[k].ParamsOedes[numPkBeginLo1].DeltaA1 / 120, 1).ToString(CultureInfo.CurrentCulture);

            // Строка 2 таблицы
            report[20] = _decoder[k].ParamsOedes[numPkBeginLo1].FrameNumber.ToString();
            report[21] = ((double)_decoder[k].ParamsOedes[numPkBeginLo1].Focuspc / 10).ToString(CultureInfo.CurrentCulture);
            report[22] = Round((double)_decoder[k].ParamsOedes[numPkBeginLo1].DeltaA1 / 120, 1).ToString(CultureInfo.CurrentCulture);

            // Строка 3 таблицы
            report[23] = _decoder[k].ParamsOedes[numberPaketFinishKanal1].FrameNumber.ToString();
            report[24] = ((double)_decoder[k].ParamsOedes[numberPaketFinishKanal1].Focuspc / 10).ToString(CultureInfo.CurrentCulture);
            report[25] = Round((double)_decoder[k].ParamsOedes[numberPaketFinishKanal1].DeltaA1 / 120, 1).ToString(CultureInfo.CurrentCulture);

            // Строка 4 таблицы
            report[26] = _decoder[k].ParamsOedes[numPkBeginLo2Uz].FrameNumber.ToString();
            report[27] = focus.ToString(CultureInfo.CurrentCulture);
            report[28] = Round((double)_decoder[k].ParamsOedes[numPkBeginLo2Uz].DeltaA2 / 120, 1).ToString(CultureInfo.CurrentCulture);

            // Строка 5 таблицы
            report[29] = _decoder[k].ParamsOedes[numPkEndLo2Uz].FrameNumber.ToString();
            report[30] = focus.ToString(CultureInfo.CurrentCulture);
            report[31] = Round((double)_decoder[k].ParamsOedes[numPkEndLo2Uz].DeltaA2 / 120, 1).ToString(CultureInfo.CurrentCulture);
            #endregion

            GeneralHealthCheckReport(_safeFilePathes[k] + ".txt", report);
            return true;
        }
        // Отчет: Проверка общей работоспособности
        private static void GeneralHealthCheckReport(string path, string[] report)
        {
            if (report.Length != 32) return;     
            
            using (var sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                sw.WriteLine("			Отчет о прохождении сеанса боевой работы ОЭД 'ПАНЦИРЬ - С1'.");
                sw.WriteLine();                
                sw.WriteLine("Файл: " + path);
                sw.WriteLine();
                sw.WriteLine("Отчет создан: " + DateTime.Now);
                sw.WriteLine();
                sw.WriteLine("Прибор №: {0} ", report[0]);
                sw.WriteLine();
                sw.WriteLine("__________________________________________________________________");
                sw.WriteLine("|                  |          |          | Уровень   | Уровень   |");
                sw.WriteLine("|   Режим          | № кадра  | Скорость | сигнала в | сигнала в |");
                sw.WriteLine("|                  |          | (гр/c)   | канале 1  | канале 2  |");
                sw.WriteLine("|__________________|__________|__________|___________|___________|");
                sw.WriteLine("|         |        |          |          |           |           |");
                sw.WriteLine("| Сопрово | начало |   {0}    |   {1}    |   {2}     |   {3}     |", report[1], report[2], report[3], report[4]);
                sw.WriteLine("| ждение  |________|__________|__________|___________|___________|");
                sw.WriteLine("| ЛО1     |        |          |          |           |           |");
                sw.WriteLine("|         | конец  |   {0}    |   {1}    |   {2}     |   {3}     |", report[5], report[6], report[7], report[8]);
                sw.WriteLine("|_________|________|__________|__________|___________|___________|");
                sw.WriteLine("|         |        |          |          |           |           |");
                sw.WriteLine("| Сопрово | начало |   {0}    |   {1}    |   {2}     |   {3}     |", report[9], report[10], report[11], report[12]);
                sw.WriteLine("| ждение  |________|__________|__________|___________|___________|");
                sw.WriteLine("| ЛО2     |        |          |          |           |           |");
                sw.WriteLine("| _(узк.) | конец  |   {0}    |   {1}    |   {2}     |   {3}     |", report[13], report[14], report[15], report[16]);
                sw.WriteLine("|_________|________|__________|__________|___________|___________|");
                sw.WriteLine(" ЛО1 - импульс. источник в к1; ЛО2 - импульс. источник в к2; ");
                sw.WriteLine();
                sw.WriteLine("			Таблица размеров полей анализа.");
                sw.WriteLine();
                sw.WriteLine("__________________________________________________________________");
                sw.WriteLine("|                 |         |            |                       |");
                sw.WriteLine("|      Режим      | № кадра | Фокус (мм) | Размер поля анализа   |");
                sw.WriteLine("|                 |         |            | (в угловых минутах ±) |");
                sw.WriteLine("|_________________|_________|____________|_______________________|");
                sw.WriteLine("|                 |         |            |                       |");
                sw.WriteLine("|   Обнар. Кан1.  |  {0}    |  {1}       | {2}                   |", report[17], report[18], report[19]);
                sw.WriteLine("|_________________|_________|____________|_______________________|");
                sw.WriteLine("|                 |         |            |                       |");
                sw.WriteLine("|Сопр. (нач) Кан1.|  {0}    |  {1}       | {2}                   |", report[20], report[21], report[22]);
                sw.WriteLine("|_________________|_________|____________|_______________________|");
                sw.WriteLine("|                 |         |            |                       |");
                sw.WriteLine("|Сопр. (кон) Кан1.|  {0}    |  {1}       | {2}                   |", report[23], report[24], report[25]);
                sw.WriteLine("|_________________|_________|____________|_______________________|");
                sw.WriteLine("|                 |         |            |                       |");
                sw.WriteLine("|Обнар. Кан2.     |  {0}    |  {1}       | {2}                   |", report[26], report[27], report[28]);
                sw.WriteLine("|_________________|_________|____________|_______________________|");
                sw.WriteLine("|                 |         |            |                       |");
                sw.WriteLine("|Сопр. Кан2.      |  {0}    |  {1}       | {2}                   |", report[29], report[30], report[31]);
                sw.WriteLine("|_________________|_________|____________|_______________________|");
            }
        }

        /// <summary>
        /// Тест №2: Проверка отработки сигналов установок ОЭД
        /// </summary>
        /// <param name="k">Номер файла</param>
        /// <returns></returns>
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
                    report[8] = _decoder[k].NumberDevice.ToString();
                    rightpackages++;
                }
                else if (_decoder[k].ParamsCvses[i].Launch && _decoder[k].ParamsCvses[i].Shod
                         && _decoder[k].ParamsCvses[i].SoprLO1 && _decoder[k].ParamsCvses[i].SoprLO2)
                {
                    CreateReport(_safeFilePathes[k] + ".txt", "Ошибка. Файл не предназначен для данного типа проверок.");
                    return false;
                }
            }

            CheckingTheTuningOfThePlantSignalsReport(_safeFilePathes[k] + ".txt", report);
            return rightpackages != 0;
        }
        // Отчет: Проверка отработки сигналов установок
        private static void CheckingTheTuningOfThePlantSignalsReport(string path, string[] data)
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
        /// Тест №3: Проверка времени накопления ФПЗС матриц ОЭД
        /// </summary>
        /// <param name="k">Номер файла</param>
        /// <returns></returns>
        private bool CheckingTheAccumulationTime(int k)
        {
            if (_decoder[k].ParamsCvses.TakeWhile(t => t.Dkp <= 5000).Any())
            {
                CreateReport(_safeFilePathes[k] + ".txt", "Ошибка. Файл не предназначен для данного типа проверок.");
                return false;
            }
            var loRangeForm = new LoRangeForm();
            loRangeForm.ShowDialog();

            var num = 0; //номер кадра введеной дальности ЛО
            int chl1BorderLow = 0, chl1BorderHight = 0;
            int chl2BorderLow = 0, chl2BorderHight = 0;
            double cmostime = 0, cmostime2 = 0;
            var report = new string[17];

            double dlo = loRangeForm.Range;
            if (dlo < 0 || dlo > 65536)
            {
                CreateReport(_safeFilePathes[k] + ".txt", "Ошибка. Дальность может быть от 0 до 65535");
                return false;
            }
            for (int i = 0; i < _decoder[k].ParamsOedes.Length - 1; i++)
            {
                if (_decoder[k].ParamsCvses[i].Dkp != (short) dlo) continue;
                num = i;
                break;
            }
            // В случае ввода неверной дальности
            if (num == 0)
            {
                CreateReport(_safeFilePathes[k] + ".txt", "Ошибка. Введена неверная дальность.");
                return false;
            }
            

            var l1 = num;
            while (_decoder[k].ParamsOedes[l1].Yc1 >=
                        _decoder[k].ParamsOedes[num].Yc1 / 1.5)
            {
                chl1BorderLow = l1;
                l1--;
                if (l1 == 0)
                break;
            }
      
            l1 = chl1BorderLow;
            while (_decoder[k].ParamsOedes[l1].Yc1 >=
                   _decoder[k].ParamsOedes[num].Yc1 / 1.5)
            {
                chl1BorderHight = l1;
                l1++;
                if (l1 == _decoder[k].ParamsOedes.Length)
                    break;
            }

            var l2 = num;
            while (_decoder[k].ParamsOedes[l2].Yc1 >=
                   _decoder[k].ParamsOedes[num].Yc1 / 1.5)
            {
                chl2BorderLow = l2;
                l2--;
                if (l2 == 0)
                    break;
            }
            l2 = chl2BorderLow;
            while (_decoder[k].ParamsOedes[l2].Yc2 >=
                   _decoder[k].ParamsOedes[num].Yc2 / 1.5)
            {
                //!!! Не понятно почему -1!!!
                chl2BorderHight = l1 - 1;
                l2++;
                if (l2 == _decoder[k].ParamsOedes.Length)
                    break;
            }

            if (chl1BorderLow > 0)
                {
                    cmostime = (double) (_decoder[k].ParamsCvses[chl1BorderLow].Dkp
                                         - _decoder[k].ParamsCvses[chl1BorderHight].Dkp) / 150;
                    report[0] = 1.ToString();
                    report[1] = _decoder[k].ParamsOedes[num].Yc1.ToString();
                    report[2] = dlo.ToString(CultureInfo.InvariantCulture);
                    report[3] = chl1BorderLow.ToString();
                    report[4] = _decoder[k].ParamsCvses[chl1BorderLow].Dkp.ToString();
                    report[5] = chl1BorderHight.ToString();
                    report[6] = _decoder[k].ParamsCvses[chl1BorderHight].Dkp.ToString();
                    report[7] = cmostime.ToString(CultureInfo.InvariantCulture);
                }
                if (chl2BorderLow > 0)
                {
                    cmostime2 = (double) (_decoder[k].ParamsCvses[chl2BorderLow].Dkp
                                          - _decoder[k].ParamsCvses[chl2BorderHight].Dkp) / 150;
                    report[8] = 2.ToString();
                    report[9] = _decoder[k].ParamsOedes[num].Yc2.ToString();
                    report[10] = dlo.ToString(CultureInfo.InvariantCulture);
                    report[11] = chl2BorderLow.ToString();
                    report[12] = _decoder[k].ParamsCvses[chl2BorderLow].Dkp.ToString();
                    report[13] = chl2BorderHight.ToString();
                    report[14] = _decoder[k].ParamsCvses[chl2BorderHight].Dkp.ToString();
                    report[15] = cmostime2.ToString(CultureInfo.InvariantCulture);
                }
                report[16] = _decoder[k].NumberDevice.ToString();
                CheckingTheAccumulationTimeReport(_safeFilePathes[k] + ".txt", report);
            return !(cmostime < 8) && !(cmostime > 18) && !(cmostime2 < 8) && !(cmostime2 > 18);
        }
        // Отчет: Проверка времени накопления
        private static void CheckingTheAccumulationTimeReport(string path, string[] report)
        {

            using (var sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                sw.WriteLine("			Отчет о времени накопления ФПЗС матриц ОЭД 'ПАНЦИРЬ - С1'.");
                sw.WriteLine();
                sw.WriteLine("Файл: " + path);
                sw.WriteLine();
                sw.WriteLine("Отчет создан: " + DateTime.Now);
                sw.WriteLine();
                sw.WriteLine("Прибор №: {0} ", report[16]);
                sw.WriteLine();
                sw.WriteLine("Номер канала                                 -| {0}", report[0]);
                sw.WriteLine();
                sw.WriteLine("Значение уровня сигнала, ед.                 -| {0}", report[1]);
                sw.WriteLine();
                sw.WriteLine("Дальность до ЛО, м.                          -| {0}", report[2]);
                sw.WriteLine();
                sw.WriteLine("Кадр начала времени накопления               -| {0}", report[3]);
                sw.WriteLine();
                sw.WriteLine("Дальность начала времени накопления, м       -| {0}", report[4]);
                sw.WriteLine();
                sw.WriteLine("Кадр завершения времени накопления           -| {0}", report[5]);
                sw.WriteLine();
                sw.WriteLine("Дальность завершения времени накопления, м   -| {0}", report[6]);
                sw.WriteLine();
                sw.WriteLine("Время накопления, мкс                        -| {0}", report[7]);
                sw.WriteLine();

                sw.WriteLine();
                sw.WriteLine("Номер канала                                 -| {0}", report[8]);
                sw.WriteLine();
                sw.WriteLine("Значение уровня сигнала, ед.                 -| {0}", report[9]);
                sw.WriteLine();
                sw.WriteLine("Дальность до ЛО, м.                          -| {0}", report[10]);
                sw.WriteLine();
                sw.WriteLine("Кадр начала времени накопления               -| {0}", report[11]);
                sw.WriteLine();
                sw.WriteLine("Дальность начала времени накопления, м       -| {0}", report[12]);
                sw.WriteLine();
                sw.WriteLine("Кадр завершения времени накопления           -| {0}", report[13]);
                sw.WriteLine();
                sw.WriteLine("Дальность завершения времени накопления, м   -| {0}", report[14]);
                sw.WriteLine();
                sw.WriteLine("Время накопления, мкс                        -| {0}", report[15]);
                sw.WriteLine();
            }
        }

        /// <summary>
        /// Тест №4: Проверка времени захвата и перезахвата
        /// </summary>
        /// <param name="k">Номер файла</param>
        /// <returns></returns>
        private bool CheckingTheCaptureAndRecaptureTime(int k)
        {
            var report = new string[13];
            report[0] = _decoder[k].NumberDevice.ToString();

            for (int i = 0; i < _decoder[k].ParamsOedes.Length; i++)
            {
                var begin1Ch1 = -1;
                // Канал 1
                // Определяем время захвата и перезахвата для канала 1
                if (_decoder[k].ParamsCvses[i].Launch && _decoder[k].ParamsCvses[i].Shod &&
                    _decoder[k].ParamsOedes[i].Capopen && _decoder[k].ParamsOedes[i].Serviceable &&
                    _decoder[k].ParamsCvses[i].SoprLO1 && _decoder[k].ParamsCvses[i].SoprLO2 &&
                    !_decoder[k].ParamsOedes[i].IndLo1 && !_decoder[k].ParamsOedes[i].IndLo1 &&
                    _decoder[k].ParamsOedes[i].Yc1 == 0 && _decoder[k].ParamsOedes[i].Yc1 == 0)
                {
                    for (int j = i; j < _decoder[k].ParamsOedes.Length; j++)
                    {
                        if (_decoder[k].ParamsOedes[j].Yc1 <= 0 || begin1Ch1 != -1 ) continue; 
                        
                        begin1Ch1 = j;
                        while (_decoder[k].ParamsOedes[j].Yc1 > 0 && !_decoder[k].ParamsOedes[j].IndLo1)
                        {
                            j++;
                        }
                        var end1Ch1 = j;
                        
                        if (begin1Ch1 == -1 || end1Ch1 == -1 ) continue;
                    
                        var recap1Ch1 = (end1Ch1 - begin1Ch1) * 0.02;

                        if (end1Ch1 - begin1Ch1 == 0 && end1Ch1 != -1 && begin1Ch1 != -1)
                            recap1Ch1 = 0.02;

                        while (_decoder[k].ParamsOedes[j].Yc1 > 0)
                        {
                            if (j == _decoder[k].ParamsOedes.Length - 1) break;
                            j++;
                        }
                        while (_decoder[k].ParamsOedes[j].Yc1 == 0)
                        {
                            j++;
                        }

                        var begin2Ch1 = j;
                        while (_decoder[k].ParamsOedes[j].Yc1 > 0 && !_decoder[k].ParamsOedes[j].IndLo1)
                        {
                            j++;
                        }
                        var end2Ch1 = j;

                        var recap2Ch1 = (end2Ch1 - begin2Ch1) * 0.02;

                        if (end2Ch1 - begin2Ch1 == 0 && end1Ch1 != -1 && begin1Ch1 != -1)
                            recap2Ch1 = 0.02;

                        // Проверяем полученные данные на валидность
                        if (begin1Ch1 == _decoder[k].ParamsOedes.Length - 1)
                        {
                            begin1Ch1 = 0;
                            end1Ch1 = 0;
                            recap1Ch1 = 0;
                        }
                        if (begin2Ch1 == _decoder[k].ParamsOedes.Length - 1)
                        {
                            begin2Ch1 = 0;
                            end2Ch1 = 0;
                            recap2Ch1 = 0;
                        }

                        //Формируем отчет
                        report[1] = begin1Ch1.ToString();
                        report[2] = end1Ch1.ToString();
                        report[3] = recap1Ch1.ToString(CultureInfo.CurrentCulture);
                        report[4] = begin2Ch1.ToString();
                        report[5] = end2Ch1.ToString();
                        report[6] = recap2Ch1.ToString(CultureInfo.CurrentCulture);
                    }
                }

                var begin1Ch2 = -1;
                // Канал 2
                // Определяем время захвата и перезахвата для канала 2
                if (_decoder[k].ParamsCvses[i].Launch && _decoder[k].ParamsCvses[i].Shod &&
                    _decoder[k].ParamsOedes[i].Capopen && _decoder[k].ParamsOedes[i].Serviceable &&
                    _decoder[k].ParamsCvses[i].SoprLO1 && _decoder[k].ParamsCvses[i].SoprLO2 &&
                    !_decoder[k].ParamsOedes[i].IndLo1 && !_decoder[k].ParamsOedes[i].IndLo1 &&
                    _decoder[k].ParamsOedes[i].Yc2 == 0 && _decoder[k].ParamsOedes[i].Yc2 == 0)
                {
                    for (int j = i; j < _decoder[k].ParamsOedes.Length; j++)
                    {
                        if (_decoder[k].ParamsOedes[j].Yc2 <= 0 || begin1Ch2 != -1) continue;

                        begin1Ch2 = j;
                        while (_decoder[k].ParamsOedes[j].Yc2 > 0 && !_decoder[k].ParamsOedes[j].IndLo2)
                        {
                            j++;
                        }
                        var end1Ch2 = j;

                        if (begin1Ch2 == -1 || end1Ch2 == -1) continue;

                        var recap1Ch2 = (end1Ch2 - begin1Ch2) * 0.02;

                        if (end1Ch2 - begin1Ch2 == 0 && end1Ch2 != -1 && begin1Ch2 != -1)
                            recap1Ch2 = 0.02;

                        while (_decoder[k].ParamsOedes[j].Yc2 > 0)
                        {
                            if (j >= _decoder[k].ParamsOedes.Length - 1) break;
                            j++;
                        }
                        while (_decoder[k].ParamsOedes[j].Yc2 == 0)
                        {
                            j++;
                        }

                        var begin2Ch2 = j;
                        while (_decoder[k].ParamsOedes[j].Yc2 > 0 && !_decoder[k].ParamsOedes[j].IndLo2)
                        {
                            j++;
                        }
                        var end2Ch2 = j;

                        var recap2Ch2 = (end2Ch2 - begin2Ch2) * 0.02;

                        if (end2Ch2 - begin2Ch2 == 0 && end1Ch2 != -1 && begin1Ch2 != -1)
                            recap2Ch2 = 0.02;

                        // Проверяем полученные данные на валидность
                        if (begin1Ch2 == _decoder[k].ParamsOedes.Length - 1)
                        {
                            begin1Ch2 = 0;
                            end1Ch2 = 0;
                            recap1Ch2 = 0;
                        }
                        if (begin2Ch2 == _decoder[k].ParamsOedes.Length - 1)
                        {
                            begin2Ch2 = 0;
                            end2Ch2 = 0;
                            recap2Ch2 = 0;
                        }

                        //Формируем отчет
                        report[7] = begin1Ch2.ToString();
                        report[8] = end1Ch2.ToString();
                        report[9] = recap1Ch2.ToString(CultureInfo.CurrentCulture);
                        report[10] = begin2Ch2.ToString();
                        report[11] = end2Ch2.ToString();
                        report[12] = recap2Ch2.ToString(CultureInfo.CurrentCulture);
                    }
                }
                // Прерываем поиск, когда время определено
                if (begin1Ch1 != -1 && begin1Ch2 != -1) break;
            }
            // Проверяем, прошёл ли тест 
            if (report[1] == null)
            {
                CreateReport(_safeFilePathes[k] + ".txt", "Ошибка. Файл не предназначен для данного типа проверок.");
                return false;
            }

            CheckingTheCaptureAndRecaptureTimeReport(_safeFilePathes[k] + ".txt", report);
            return true;
        }
        // Отчет: Проверка времени захвата и перезахвата
        private static void CheckingTheCaptureAndRecaptureTimeReport(string path, string[] report)
        {
            using (var sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                sw.WriteLine("			Отчет о проверке времени захвата и перезахвата ОЭД 'ПАНЦИРЬ - С1'.");
                sw.WriteLine();
                sw.WriteLine("Файл: " + path);
                sw.WriteLine();
                sw.WriteLine("Отчет создан: " + DateTime.Now);
                sw.WriteLine();
                sw.WriteLine("Прибор №: {0} ", report[0]);
                sw.WriteLine();
                sw.WriteLine("Канал 1");
                sw.WriteLine();
                sw.WriteLine("Начало захвата         -| {0}", report[1]);
                sw.WriteLine();
                sw.WriteLine("Окончание захвата      -| {0}", report[2]);
                sw.WriteLine();
                sw.WriteLine("Время захвата, мс.     -| {0}", report[3]);
                sw.WriteLine();
                sw.WriteLine("Начало перезахвата     -| {0}", report[4]);
                sw.WriteLine();
                sw.WriteLine("Окончание перезахвата  -| {0}", report[5]);
                sw.WriteLine();
                sw.WriteLine("Время перезахвата, мс. -| {0}", report[6]);
                sw.WriteLine();
                sw.WriteLine("Канал 2");
                sw.WriteLine();
                sw.WriteLine("Начало захвата         -| {0}", report[7]);
                sw.WriteLine();
                sw.WriteLine("Окончание захвата      -| {0}", report[8]);
                sw.WriteLine();
                sw.WriteLine("Время захвата, мс.     -| {0}", report[9]);
                sw.WriteLine();
                sw.WriteLine("Начало перезахвата     -| {0}", report[10]);
                sw.WriteLine();
                sw.WriteLine("Окончание перезахвата  -| {0}", report[11]);
                sw.WriteLine();
                sw.WriteLine("Время перезахвата, мс. -| {0}", report[12]);
                sw.WriteLine();

            }
        }

        //Анализ результатов диагностики
        private void DiagnosticResultsAnalysis()
        {
            
        }

        #endregion

        // Считывание состояния элементов GUI
        private void timerGUI_Tick(object sender, EventArgs e)
        {
            if (_cblist == null) return;
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
