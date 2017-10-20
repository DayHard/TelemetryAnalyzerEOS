using System.IO;
using System.Windows.Forms;

namespace TelemetryAnalyzerEOS
{
    public class Decoder
    {
        public DockPrlParamOedCvs[] ParamsOedes;
        public DockPrlParamCvsOed[] ParamsCvses;

        private const uint SingBegin = 0xE4EF7289;
        private const uint SingEnd = 0x76EFFF28;
        private const uint CsPointer = 2300 * 4 + 8;
        private byte[] _data;

        /// <summary>
        /// Открытие файла телеметрии.
        /// </summary>
        /// <param name="path">Путь к файлу.</param>
        /// <returns>Возвращает true, если файл был открыт успешно.</returns>
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
                    ParamsOedes = new DockPrlParamOedCvs[(uint)(_data[2] << 8 | _data[3])];
                    //Инициализируем
                    for (int i = 0; i < ParamsOedes.Length; i++)
                    {
                        ParamsOedes[i] = new DockPrlParamOedCvs();
                    }
                    ParamsCvses = new DockPrlParamCvsOed[(uint)(_data[2] << 8 | _data[3])];
                    // Инициализируем
                    for (int i = 0; i < ParamsCvses.Length; i++)
                    {
                        ParamsCvses[i] = new DockPrlParamCvsOed();
                    }
                    return true;
                }

                ////Считаем количество валидных пакетов
                //int counter = 0;
                //for (int i = 0; i < _data.Length; i += 4)
                //{
                //    if ((uint)((_data[i] << 24) | (_data[i + 1] << 16) | (_data[i + 2] << 8) |
                //                _data[i + 3]) == SingBegin)
                //    {
                //        if (PackagesIsValid(i))
                //            counter++;
                //    }
                //}
                ////Выделяем память под данные OED
                //ParamsOedes = new DockPrlParamOedCvs[counter];
                //for (int i = 0; i < ParamsOedes.Length; i++)
                //{
                //    ParamsOedes[i] = new DockPrlParamOedCvs();
                //}
                ////Выделяем память под данные CVS
                //ParamsCvses = new DockPrlParamCvsOed[counter];
                //for (int i = 0; i < ParamsCvses.Length; i++)
                //{
                //    ParamsCvses[i] = new DockPrlParamCvsOed();
                //}
                //return true;
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Декодирование параметров протокола стыковки.
        /// </summary>
        /// <returns>Возвращает true, при успешном декодировании.</returns>
        public bool Decode()
        {
            int counter = 0, ecounter = 0;
            uint pStart = 0, pEnd = 0;

            for (uint i = 0; i < _data.Length; i+=4)
            {
                if (ToBigEndian(i, 4) == SingBegin)
                {
                    pStart = i;
                    for (uint j = i; j < _data.Length; j++)
                    {
                        if (ToBigEndian(j, 4) == SingEnd)
                        {
                            pEnd = j;
                            i = j;
                            break;
                        }
                    }
                    // Если пакет валидный, начинаем его разбор
                    if (!PackagesIsValid(pStart, pEnd) || !DecodePackage(counter, pStart, pEnd))
                    {
                        ParamsOedes[counter] = null;
                        ParamsCvses[counter] = null;
                        ecounter++;
                    }
                    counter++;
                }
            }
            MessageBox.Show(@"Counted packages: " + counter +@" Bad packages: "+ ecounter);
            return false;
        }

        private bool DecodePackage(int counter, uint pStart, uint pEnd)
        {
            ParamsOedes[counter].FrameNumber = (ushort)counter;
            var word1 = ToBigEndian(pStart + 24, 2);
            //Готов
            if ( (word1 & 0x1) != 0)
                ParamsOedes[counter].Ready = true;
            //Инд ЛО1
            if ((word1 & 0x2) != 0)
                ParamsOedes[counter].IndLo1 = true;
            //Инд ЛО2
            if ((word1 & 0x4) != 0)
                ParamsOedes[counter].IndLo2 = true;
            // Кр Закр(Крышка закрыта)
            if ((word1 & 0x10) != 0)
                ParamsOedes[counter].Capclosed = true;
            //Кр Откр (Крышка открыта)
            if ((word1 & 0x20) != 0)
                ParamsOedes[counter].Capclosed = true;
            // Засв (Засветка)
            if ((word1 & 0x40) != 0)
                ParamsOedes[counter].Illumination = true;
            // Нагрев
            if ((word1 & 0x80) != 0)
                ParamsOedes[counter].Heat = true;
            // Охл (Охлаждение)
            if ((word1 & 0x100) != 0)
                ParamsOedes[counter].Cooling = true;
            //Тнорма (Температура в норме)
            if ((word1 & 0x200) != 0)
                ParamsOedes[counter].Tnormal = true;
            //Строб К
            if ((word1 & 0x400) != 0)
                ParamsOedes[counter].StrobK = true;
            //Исправен
            if ((word1 & 0x800) != 0)
                ParamsOedes[counter].Serviceable = true;
            //УЗК-К2
            if ((word1 & 0x1000) != 0)
                ParamsOedes[counter].YzkK2 = true;
            //Y-CLS-F
            if ((word1 & 0x8000) != 0)
                ParamsOedes[counter].YClsF = true;

            var word2 = ToBigEndian(pStart + 24 + 2, 2);
            ParamsOedes[counter].Qkd1 = (int)word2;

            //ParamsOedes[counter].Qkd1 = 1;// Датчик канала 1 по координате q

            return true;
        }

        /// <summary>
        /// Проверка заголовка файла на валидность.
        /// </summary>
        /// <returns>Возвращает true, если заголовок валиден.</returns>
        public bool HeaderIsValid()
        {
            // Считываем записанную контрольную сумму заголовка
            uint controlSum = ToBigEndian(CsPointer, 4);

            // Высчитываем контрольную сумму заголовка
            uint countedSum = 0;
            for (uint i = 0; i < CsPointer; i += 4)
            {
                countedSum += ToBigEndian(i, 4);
            }
            //Устанавливаем флаг валидности заголовка
            if (controlSum == countedSum)
                return true;
            return false;
        }

        /// <summary>
        /// Проверка пакета на валидность.
        /// </summary>
        /// <param name="pStart">Указатель начала признака пакета.</param>
        /// <param name="pEnd">Указатель конца признака пакета.</param>
        /// <returns>Возвращает true, если пакет валиден.</returns>
        public bool PackagesIsValid(uint pStart, uint pEnd)
        {
            if (ToBigEndian(pStart, 4) == SingBegin && ToBigEndian(pEnd, 4) == SingEnd)
            {
                uint controlSum = ToBigEndian(pEnd + 4, 4);
                uint countedSum = 0;
                for (uint i = pStart; i < pEnd + 4; i += 4)
                {
                    countedSum += ToBigEndian(i, 4);
                }
                if (countedSum == controlSum)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Преобразование к Big endian.
        /// </summary>
        /// <param name="counter">Позиция первого байта.</param>
        /// <param name="size">Количество преобразуемых байтов.</param>
        /// <returns>Возращает преобразованое значение в UInt.</returns>
        private uint ToBigEndian(uint counter, int size)
        {
            byte[] cdata = new byte[size];
            int j = 0;
            for (uint i = counter; i < counter + size; i++)
            {
                cdata[j] = _data[i];
                j++;
            }
            for (int i = 0; i < cdata.Length / 2; i++)
            {
                var temp = cdata[i];
                cdata[i] = cdata[cdata.Length - i - 1];
                cdata[cdata.Length - i - 1] = temp;
            }
            uint number = 0;
            for (int i = 0; i < cdata.Length; i++)
            {
                number += (uint)cdata[i] << 8 * i;
            }
            return number;
        }
    }
}
