using System;
using System.IO;

namespace TelemetryAnalyzerEOS
{
     //Поля параметров протокола стыковки
     public  class DockPrlParam
     {
        private bool _launch; //пуск
        private bool _shod; //сход
        private bool _soprLo1; //Сопр ЛО1
        private bool _soprLo2; //Сопр ЛО2
        private bool _opencap; //Откр Кр
        private bool _resetpc; //СБРОСпс
        private bool _mode; //Режим
        private bool _groundtarget; //Назем цель
        private bool _clsF; //CLS-F

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
     }
   public class Decoder
   {
       public DockPrlParam[] DockPrlParams;

       private const uint SingBegin = 0xE4EF7289;
       private const uint SingEnd = 0x76EFFF28;
       private const uint CsPointer = 2300 * 4 + 8;
       private byte[] _data;

       //Считывание данных из файла в массив
        public bool Open(string path)
        {
            try
            {
                _data = new byte[(int)new FileInfo(path).Length];
                using (BinaryReader bReader = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read)))
                {
                    _data = bReader.ReadBytes(_data.Length);
                }

                //Проверка заголовка, если да выделям память под количество пакетов
                if (HeaderIsValid())
                {
                    DockPrlParams = new DockPrlParam[(uint)(_data[2] << 8 | _data[3])];
                    for (int i = 0; i < DockPrlParams.Length; i++)
                    {
                        DockPrlParams[i] = new DockPrlParam();
                    }
                    return true;
                }

                //Считаем количество валидных пакетов
                int counter = 0;
                for (int i = 0; i < _data.Length; i+=4)
                {
                    if ((uint)((_data[i] << 24) | (_data[i + 1] << 16) | (_data[i + 2] << 8) |
                                _data[i + 3]) == SingBegin)
                    {
                        if (PackagesIsValid(i))
                            counter ++;
                    }
                }
                //Выделяем память под данные
                DockPrlParams = new DockPrlParam[counter];
                for (int i = 0; i < DockPrlParams.Length; i++)
                {
                    DockPrlParams[i] = new DockPrlParam();
                }
                return true;
            }
            catch
            {
                return false;
            }
            //var a = PackagesIsValid(2300 * 4 + 8 + 4);
            //var a = PackagesIsValid(9392);
        }
        // Декодирование параметров протокола стыковки
       public bool Decode()
       {

           return false;
       } 
        //Проверка заголовка структуры на валидность
        public bool HeaderIsValid()
        {
            // Считываем записанную контрольную сумму заголовка
            uint controlSum = (uint)((_data[CsPointer] << 24) + (_data[CsPointer + 1] << 16)
                               + (_data[CsPointer + 2] << 8) + _data[CsPointer + 3]);
            // Высчитываем контрольную сумму заголовка
            uint countedSum = 0;
            for (int i = 0; i < CsPointer; i += 4)
            {
                countedSum += (uint)((_data[i] << 24) | (_data[i + 1] << 16) | (_data[i + 2] << 8) | _data[i + 3]);
            }
            //Устанавливаем флаг валидности заголовка
            if (controlSum == countedSum)
                return true;
            return false;
        }
        //Проверка пакета на валидность
       public bool PackagesIsValid(int counter)
       {
           if ((uint) ((_data[counter] << 24) | (_data[counter + 1] << 16) | (_data[counter + 2] << 8) |
                       _data[counter + 3]) == SingBegin)
           {
               uint countedSum = 0;
               int i = counter - 4;

               do
               {
                   i += 4;
                   countedSum += (uint) ((_data[i] << 24) | (_data[i + 1] << 16) | (_data[i + 2] << 8) | _data[i + 3]);
               } while ((uint) ((_data[i] << 24) | (_data[i + 1] << 16) | (_data[i + 2] << 8) |
                                _data[i + 3]) != SingEnd);
               i += 4;
               uint controlSum = (uint) ((_data[i] << 24) | (_data[i + 1] << 16) | (_data[i + 2] << 8) | _data[i + 3]);
               if (controlSum == countedSum) return true;

           }
           return false;
       }
    }
}
