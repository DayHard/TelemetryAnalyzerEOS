using System.IO;

namespace TelemetryAnalyzerEOS
{
   public  class Decoder
   {
        private const uint SingBegin = 0xE4EF7289;
        private const uint SingEnd = 0x76EFFF28;
        private const uint CsPointer = 2300 * 4 + 8;
        private byte[] _data;

        //Считывание данных из файла в массив
        public  void Open(string path)
        {
            _data = new byte[(int)new FileInfo(path).Length];
            using (BinaryReader bReader = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read)))
            {
                _data = bReader.ReadBytes(_data.Length);
            }
            var a = PackagesIsValid(2300 * 4 + 8 + 4);
            //var a = PackagesIsValid(9392);
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
