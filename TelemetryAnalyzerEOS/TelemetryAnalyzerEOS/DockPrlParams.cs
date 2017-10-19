namespace TelemetryAnalyzerEOS
{
    //Поля параметров протокола стыковки
    //Направление ОЭД-ЦВС
    public class DockPrlParamOedCvs
    {
        private ushort _framenumber;
        //Сл.1 boolean
        private bool _ready; //Готов
        private bool _indLo1; //Инд ЛО1
        private bool _indLo2; // Инд ЛО2
        private bool _capclosed; //Кр Закр (Крышка закрыта)
        private bool _capopen; //Кр Откр (Крышка открыта)
        private bool _illumination; // Засв (Засветка)
        private bool _heat; // Нагрев
        private bool _cooling; // Охл (Охлаждение)
        private bool _tnormal; //Тнорма (Температура в норме)
        private bool _strobK; //Строб К
        private bool _serviceable; //Исправен
        private bool _yzkK2; //УЗК-К2
        private bool _yClsF; //Y-CLS-F
        //Int32
        private int _qkd1; //Сл.2
        private int _fikd1; //Сл.3
        private int _qkk1; //Сл.4
        private int _fikk1; //Сл.5
        private int _qkd2; //Сл.6
        private int _fikd2; //Сл.7
        private int _qkk2; //Сл.8
        private int _fikk2; //Сл.9
        private int _dkp; //Сл.10
        // ReSharper disable once InconsistentNaming
        private int _Lfi1; //Сл.11
        // ReSharper disable once InconsistentNaming
        private int _Lq1; // Сл.11
        // ReSharper disable once InconsistentNaming
        private int _Lfi2; //Сл.12
        // ReSharper disable once InconsistentNaming
        private int _Lq2; //Сл.12
        private int _yc1; //Сл.13
        private int _yc2; //Сл.14
        private int _nch; //Сл.15
        private int _deltaA1; //Сл.16
        private int _deltaA2; //Сл.17
        // ReSharper disable once InconsistentNaming
        private int _K1cp; //Сл.18
        // ReSharper disable once InconsistentNaming
        private int _K2cp; //Сл.19
        private int _nc; //Сл.20
        private int _focuspc; //Сл.21
        // ReSharper disable once InconsistentNaming
        private int _Sdiafr1; //Сл.22
        private int _tn1; //Сл.23
        // ReSharper disable once InconsistentNaming
        private int _Ttec; //Сл.24

        #region Incapsulation
        public bool Ready
        {
            get { return _ready; }
            set { _ready = value; }
        }

        public bool IndLo1
        {
            get { return _indLo1; }
            set { _indLo1 = value; }
        }

        public bool IndLo2
        {
            get { return _indLo2; }
            set { _indLo2 = value; }
        }

        public bool Capclosed
        {
            get { return _capclosed; }
            set { _capclosed = value; }
        }

        public bool Capopen
        {
            get { return _capopen; }
            set { _capopen = value; }
        }

        public bool Illumination
        {
            get { return _illumination; }
            set { _illumination = value; }
        }

        public bool Heat
        {
            get { return _heat; }
            set { _heat = value; }
        }

        public bool Cooling
        {
            get { return _cooling; }
            set { _cooling = value; }
        }

        public bool Tnormal
        {
            get { return _tnormal; }
            set { _tnormal = value; }
        }

        public bool StrobK
        {
            get { return _strobK; }
            set { _strobK = value; }
        }

        public bool Serviceable
        {
            get { return _serviceable; }
            set { _serviceable = value; }
        }

        public bool YzkK2
        {
            get { return _yzkK2; }
            set { _yzkK2 = value; }
        }

        public bool YClsF
        {
            get { return _yClsF; }
            set { _yClsF = value; }
        }

        public int Qkd1
        {
            get { return _qkd1; }
            set { _qkd1 = value; }
        }

        public int Fikd1
        {
            get { return _fikd1; }
            set { _fikd1 = value; }
        }

        public int Qkk1
        {
            get { return _qkk1; }
            set { _qkk1 = value; }
        }

        public int Fikk1
        {
            get { return _fikk1; }
            set { _fikk1 = value; }
        }

        public int Qkd2
        {
            get { return _qkd2; }
            set { _qkd2 = value; }
        }

        public int Fikd2
        {
            get { return _fikd2; }
            set { _fikd2 = value; }
        }

        public int Qkk2
        {
            get { return _qkk2; }
            set { _qkk2 = value; }
        }

        public int Fikk2
        {
            get { return _fikk2; }
            set { _fikk2 = value; }
        }

        public int Dkp
        {
            get { return _dkp; }
            set { _dkp = value; }
        }

        public int Lfi1
        {
            get { return _Lfi1; }
            set { _Lfi1 = value; }
        }

        public int Lq1
        {
            get { return _Lq1; }
            set { _Lq1 = value; }
        }

        public int Lfi2
        {
            get { return _Lfi2; }
            set { _Lfi2 = value; }
        }

        public int Lq2
        {
            get { return _Lq2; }
            set { _Lq2 = value; }
        }

        public int Yc1
        {
            get { return _yc1; }
            set { _yc1 = value; }
        }

        public int Yc2
        {
            get { return _yc2; }
            set { _yc2 = value; }
        }

        public int Nch
        {
            get { return _nch; }
            set { _nch = value; }
        }

        public int DeltaA1
        {
            get { return _deltaA1; }
            set { _deltaA1 = value; }
        }

        public int DeltaA2
        {
            get { return _deltaA2; }
            set { _deltaA2 = value; }
        }

        public int K1Cp
        {
            get { return _K1cp; }
            set { _K1cp = value; }
        }

        public int K2Cp
        {
            get { return _K2cp; }
            set { _K2cp = value; }
        }

        public int Nc
        {
            get { return _nc; }
            set { _nc = value; }
        }

        public int Focuspc
        {
            get { return _focuspc; }
            set { _focuspc = value; }
        }

        public int Sdiafr1
        {
            get { return _Sdiafr1; }
            set { _Sdiafr1 = value; }
        }

        public int Tn1
        {
            get { return _tn1; }
            set { _tn1 = value; }
        }

        public int Ttec
        {
            get { return _Ttec; }
            set { _Ttec = value; }
        }

        public ushort FrameNumber
        {
            get { return _framenumber; }
            set { _framenumber = value; }
        }

        #endregion
    }
    //Поля параметров протокола стыковки
    //Направление ЦВС-ОЭД
    public class DockPrlParamCvsOed
    {
        private ushort _framenumber;
        //Сл.1 boolean
        private bool _launch; //Пуск
        private bool _shod; //Сход
        private bool _soprLo1; //Сопр ЛО1
        private bool _soprLo2; //Сопр ЛО2
        private bool _opencap; //Откр Кр
        private bool _resetpc; //СБРОСпс
        private bool _mode; //Режим
        private bool _groundtarget; //Назем цель
        private bool _clsF; //CLS-F
        //Int32
        private int _deltaqk1; //Сл.2
        private int _deltafik1; // Сл.3
        private int _deltaqk2; //Сл.4
        private int _deltafik2; //Сл.5
        private int _qkpod; //Сл.6
        private int _fikpod; //Сл.7
        private int _dkp; //Сл.8
        private int _qkgdu; //Сл.9
        private int _fikgdu;  //Сл.10

        #region Incapsulation
        public bool Launch
        {
            get { return _launch; }
            set { _launch = value; }
        }

        public bool Shod
        {
            get { return _shod; }
            set { _shod = value; }
        }

        public bool SoprLO1
        {
            get { return _soprLo1; }
            set { _soprLo1 = value; }
        }

        public bool SoprLO2
        {
            get { return _soprLo2; }
            set { _soprLo2 = value; }
        }

        public bool Opencap
        {
            get { return _opencap; }
            set { _opencap = value; }
        }

        public bool Resetpc
        {
            get { return _resetpc; }
            set { _resetpc = value; }
        }

        public bool Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        public bool Groundtarget
        {
            get { return _groundtarget; }
            set { _groundtarget = value; }
        }

        public bool CLS_F
        {
            get { return _clsF; }
            set { _clsF = value; }
        }

        public int Deltaqk1
        {
            get { return _deltaqk1; }
            set { _deltaqk1 = value; }
        }

        public int Deltafik1
        {
            get { return _deltafik1; }
            set { _deltafik1 = value; }
        }

        public int Deltaqk2
        {
            get { return _deltaqk2; }
            set { _deltaqk2 = value; }
        }

        public int Deltafik2
        {
            get { return _deltafik2; }
            set { _deltafik2 = value; }
        }

        public int Qkpod
        {
            get { return _qkpod; }
            set { _qkpod = value; }
        }

        public int Fikpod
        {
            get { return _fikpod; }
            set { _fikpod = value; }
        }

        public int Dkp
        {
            get { return _dkp; }
            set { _dkp = value; }
        }

        public int Qkgdu
        {
            get { return _qkgdu; }
            set { _qkgdu = value; }
        }

        public int Fikgdu
        {
            get { return _fikgdu; }
            set { _fikgdu = value; }
        }

        public ushort FrameNumber
        {
            get { return _framenumber; }
            set { _framenumber = value; }
        }

        #endregion
    }
}
