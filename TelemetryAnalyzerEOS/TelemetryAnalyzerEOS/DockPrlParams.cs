namespace TelemetryAnalyzerEOS
{
    //Поля параметров протокола стыковки
    //Направление ОЭД-ЦВС
    public class DockPrlParamOedCvs
    {
        private uint _numberDevice;
        private short _framenumber;
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
        private short _qkd1; //Сл.2
        private short _fikd1; //Сл.3
        private short _qkk1; //Сл.4
        private short _fikk1; //Сл.5
        private short _qkd2; //Сл.6
        private short _fikd2; //Сл.7
        private short _qkk2; //Сл.8
        private short _fikk2; //Сл.9
        private short _dkp; //Сл.10
        // ReSharper disable once InconsistentNaming
        private short _Lfi1; //Сл.11
        // ReSharper disable once InconsistentNaming
        private short _Lq1; // Сл.11
        // ReSharper disable once InconsistentNaming
        private short _Lfi2; //Сл.12
        // ReSharper disable once InconsistentNaming
        private short _Lq2; //Сл.12
        private short _yc1; //Сл.13
        private short _yc2; //Сл.14
        private short _nch; //Сл.15
        private short _deltaA1; //Сл.16
        private short _deltaA2; //Сл.17
        // ReSharper disable once InconsistentNaming
        private short _K1cp; //Сл.18
        // ReSharper disable once InconsistentNaming
        private short _K2cp; //Сл.19
        private short _nc; //Сл.20
        private short _focuspc; //Сл.21
        // ReSharper disable once InconsistentNaming
        private short _Sdiafr1; //Сл.22
        private short _tn1; //Сл.23
        // ReSharper disable once InconsistentNaming
        private short _Ttec; //Сл.24

        public DockPrlParamOedCvs()
        {
            _framenumber = 0;
            //Сл.1 boolean
            _ready = false;
            _indLo1 = false;
            _indLo2 = false;
            _capclosed = false;
            _capopen = false;
            _illumination = false;
            _heat = false;
            _cooling = false;
            _tnormal = false;
            _strobK = false;
            _serviceable = false;
            _yzkK2 = false;
            _yClsF = false;

            //Int32
            _qkd1 = 0; //Сл.2
            _fikd1 = 0; //Сл.3
            _qkk1 = 0; //Сл.4
            _fikk1 = 0; //Сл.5
            _qkd2 = 0; //Сл.6
            _fikd2 = 0; //Сл.7
            _qkk2 = 0; //Сл.8
            _fikk2 = 0; //Сл.9
            _dkp = 0; //Сл.10
            _Lfi1 = 0; //Сл.11
            _Lq1 = 0; // Сл.11
             _Lfi2 = 0; //Сл.12
            _Lq2 = 0; //Сл.12
             _yc1 = 0; //Сл.13
            _yc2 = 0; //Сл.14
            _nch = 0; //Сл.15
            _deltaA1 = 0; //Сл.16
            _deltaA2 = 0; //Сл.17
            _K1cp = 0; //Сл.18
            _K2cp = 0; //Сл.19
            _nc = 0; //Сл.20
            _focuspc = 0; //Сл.21
            _Sdiafr1 = 0; //Сл.22
            _tn1 = 0; //Сл.23
             _Ttec = 0; //Сл.24
    }

    #region Incapsulation
    public short FrameNumber
    {
    get { return _framenumber; }
    set { _framenumber = value; }
    }
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

        public short Qkd1
        {
            get { return _qkd1; }
            set { _qkd1 = value; }
        }

        public short Fikd1
        {
            get { return _fikd1; }
            set { _fikd1 = value; }
        }

        public short Qkk1
        {
            get { return _qkk1; }
            set { _qkk1 = value; }
        }

        public short Fikk1
        {
            get { return _fikk1; }
            set { _fikk1 = value; }
        }

        public short Qkd2
        {
            get { return _qkd2; }
            set { _qkd2 = value; }
        }

        public short Fikd2
        {
            get { return _fikd2; }
            set { _fikd2 = value; }
        }

        public short Qkk2
        {
            get { return _qkk2; }
            set { _qkk2 = value; }
        }

        public short Fikk2
        {
            get { return _fikk2; }
            set { _fikk2 = value; }
        }

        public short Dkp
        {
            get { return _dkp; }
            set { _dkp = value; }
        }

        public short Lfi1
        {
            get { return _Lfi1; }
            set { _Lfi1 = value; }
        }

        public short Lq1
        {
            get { return _Lq1; }
            set { _Lq1 = value; }
        }

        public short Lfi2
        {
            get { return _Lfi2; }
            set { _Lfi2 = value; }
        }

        public short Lq2
        {
            get { return _Lq2; }
            set { _Lq2 = value; }
        }

        public short Yc1
        {
            get { return _yc1; }
            set { _yc1 = value; }
        }

        public short Yc2
        {
            get { return _yc2; }
            set { _yc2 = value; }
        }

        public short Nch
        {
            get { return _nch; }
            set { _nch = value; }
        }

        public short DeltaA1
        {
            get { return _deltaA1; }
            set { _deltaA1 = value; }
        }

        public short DeltaA2
        {
            get { return _deltaA2; }
            set { _deltaA2 = value; }
        }

        public short K1Cp
        {
            get { return _K1cp; }
            set { _K1cp = value; }
        }

        public short K2Cp
        {
            get { return _K2cp; }
            set { _K2cp = value; }
        }

        public short Nc
        {
            get { return _nc; }
            set { _nc = value; }
        }

        public short Focuspc
        {
            get { return _focuspc; }
            set { _focuspc = value; }
        }

        public short Sdiafr1
        {
            get { return _Sdiafr1; }
            set { _Sdiafr1 = value; }
        }

        public short Tn1
        {
            get { return _tn1; }
            set { _tn1 = value; }
        }

        public short Ttec
        {
            get { return _Ttec; }
            set { _Ttec = value; }
        }

        public uint NumberDevice
        {
            get { return _numberDevice; }
            set { _numberDevice = value; }
        }

        #endregion
    }
    //Поля параметров протокола стыковки
    //Направление ЦВС-ОЭД
    public class DockPrlParamCvsOed
    {
        private short _framenumber;
        //Сл.1 boolean
        private bool _launch; //Пуск
        private bool _shod; //Сход
        private bool _soprLo1; //Сопр ЛО1
        private bool _soprLo2; //Сопр ЛО2
        private bool _opencap; //Откр Кр
        private bool _resetpc; //СБРОСпс
        private bool _mode; //Режим
        private bool _groundtarget; //Назем цель
        //Int32
        private short _deltaqk1; //Сл.2
        private short _deltafik1; // Сл.3
        private short _deltaqk2; //Сл.4
        private short _deltafik2; //Сл.5
        private short _qkpod; //Сл.6
        private short _fikpod; //Сл.7
        private short _dkp; //Сл.8
        private short _qkgdu; //Сл.9
        private short _fikgdu;  //Сл.10

        public DockPrlParamCvsOed()
        {
            _framenumber = 0;
            //Сл.1 boolean
            _launch = false; //Пуск
            _shod = false; //Сход
            _soprLo1 = false; //Сопр ЛО1
            _soprLo2 = false; //Сопр ЛО2
            _opencap = false; //Откр Кр
            _resetpc = false; //СБРОСпс
            _mode = false; //Режим
            _groundtarget = false; //Назем цель
            //Int32
            _deltaqk1 = 0; //Сл.2
            _deltafik1 = 0; // Сл.3
            _deltaqk2 = 0; //Сл.4
            _deltafik2 = 0; //Сл.5
            _qkpod = 0; //Сл.6
            _fikpod = 0; //Сл.7
            _dkp = 0; //Сл.8
            _qkgdu = 0; //Сл.9
            _fikgdu = 0;  //Сл.10 
    }
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

        public short Deltaqk1
        {
            get { return _deltaqk1; }
            set { _deltaqk1 = value; }
        }

        public short Deltafik1
        {
            get { return _deltafik1; }
            set { _deltafik1 = value; }
        }

        public short Deltaqk2
        {
            get { return _deltaqk2; }
            set { _deltaqk2 = value; }
        }

        public short Deltafik2
        {
            get { return _deltafik2; }
            set { _deltafik2 = value; }
        }

        public short Qkpod
        {
            get { return _qkpod; }
            set { _qkpod = value; }
        }

        public short Fikpod
        {
            get { return _fikpod; }
            set { _fikpod = value; }
        }

        public short Dkp
        {
            get { return _dkp; }
            set { _dkp = value; }
        }

        public short Qkgdu
        {
            get { return _qkgdu; }
            set { _qkgdu = value; }
        }

        public short Fikgdu
        {
            get { return _fikgdu; }
            set { _fikgdu = value; }
        }

        public short FrameNumber
        {
            get { return _framenumber; }
            set { _framenumber = value; }
        }

        #endregion
    }
}
