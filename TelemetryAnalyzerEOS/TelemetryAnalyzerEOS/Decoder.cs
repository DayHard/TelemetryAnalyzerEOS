using System.IO;
using System.Runtime.InteropServices;
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
                    ParamsOedes = new DockPrlParamOedCvs[ToBigEndian(2, 2)];
                    ParamsCvses = new DockPrlParamCvsOed[ToBigEndian(2, 2)];
                }
                else
                {
                    //Выделяем память под данные OED
                    ParamsOedes = new DockPrlParamOedCvs[2300];
                    //Выделяем память под данные CVS
                    ParamsCvses = new DockPrlParamCvsOed[2300];
                }

                //Инициализируем OED
                for (int i = 0; i < ParamsOedes.Length; i++)
                {
                    ParamsOedes[i] = new DockPrlParamOedCvs();
                }
                //Инициализируем CVS
                for (int i = 0; i < ParamsCvses.Length; i++)
                {
                    ParamsCvses[i] = new DockPrlParamCvsOed();
                }
                return true;
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
            for (uint i = CsPointer + 4; i < _data.Length; i+=4)
            {
                if (ToBigEndian(i, 4) == SingBegin)
                {
                    var pStart = i;
                    var offset = ToBigEndian(i + 4, 4);
                    var pEnd = offset - 8;
                    if (ToBigEndian(pEnd, 4) == SingEnd)
                    {
                        if (PackagesIsValid(pStart, pEnd))
                        {
                            DecodePackage(counter, pStart, pEnd);
                        }
                        else
                        {
                            //!!!!
                            ParamsOedes[counter] = null;
                            ecounter++;
                        }
                    }
                    // Если указанное смещение не верно, ищем признак конца
                    else
                    {
                        for (uint j = i+4; j < _data.Length; j+=4)
                        {
                            var sing = ToBigEndian(j, 4);
                            if (sing == SingEnd)
                            {
                                pEnd = j;
                                if (PackagesIsValid(pStart, pEnd))
                                {
                                    DecodePackage(counter, pStart, pEnd);
                                }
                                else
                                {
                                    //!!!!
                                    ParamsOedes[counter] = null;
                                    ecounter++;
                                }
                                break;
                            }

                            if (sing == SingBegin)
                            {
                                //!!!!
                                ParamsOedes[counter] = null;
                                i -= 4;
                                break;
                            }
                        }
                    }
                    counter++;
                }
            }
            MessageBox.Show(@"Counted packages: " + counter + @" Bad packages: " + ecounter);
            return false;
        }
        ///// <summary>
        ///// Декодирование параметров протокола стыковки.
        ///// </summary>
        ///// <returns>Возвращает true, при успешном декодировании.</returns> 
        //public bool Decode(int overload)
        //{
        //    int counter = 0, ecounter = 0;
        //    uint pStart = 0, pEnd = 0;

        //    for (uint i = 0; i < _data.Length; i+=4)
        //    {
        //        if (ToBigEndian(i, 4) == SingBegin)
        //        {
        //            pStart = i;
        //            for (uint j = i; j < _data.Length; j++)
        //            {
        //                if (ToBigEndian(j, 4) == SingEnd)
        //                {
        //                    pEnd = j;
        //                    i = j;
        //                    break;
        //                }
        //            }
        //            // Если пакет валидный, начинаем его разбор
        //            if (!PackagesIsValid(pStart, pEnd) || !DecodePackage(counter, pStart, pEnd))
        //            {
        //                ParamsOedes[counter] = null;
        //                ParamsCvses[counter] = null;
        //                ecounter++;
        //            }
        //            counter++;
        //        }
        //    }
        //    MessageBox.Show(@"Counted packages: " + counter +@" Bad packages: "+ ecounter);
        //    return false;
        //}

        private bool DecodePackage(int counter, uint pStart, uint pEnd)
        {
            if (!DecodeOedCvs(counter, pStart))
                return false;

            //if (!DecodeCvsOed(counter, pStart))
                //return false;

            return true;
        }
        //Направление ОЭД -> ЦВС декодирование
        private bool DecodeOedCvs(int counter, uint pStart)
        {
            uint shift = 24;

            ParamsOedes[counter].FrameNumber = (ushort)counter;
            //Сигналы
            var word1 = ToBigEndian(pStart + shift, 2);
            //Готов
            if ((word1 & 0x1) != 0)
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
            shift += 2;

            // Датчик канала 1 по координате q qkd1
            var word2 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Qkd1 = (int)word2 * 10;

            shift += 2;
            // Датчик канала 1 по координате fi fikd1 
            var word3 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Fikd1 = (int)word3 * 10;

            shift += 2;
            // Координатор канала 1 по координате q qkk1
            var word4 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Qkk1 = (int)word4 * 10;

            shift += 2;
            // Координатор канала 1 по координате fi fikk1
            var word5 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Fikk1 = (int)word5 * 10;

            shift += 2;
            // Датчик канала 2 по координате q qkd2
            var word6 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Qkd2 = (int)word6 * 2;

            shift += 2;
            // Датчик канала 2 по координате fi fikd2
            var word7 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Fikd2 = (int)word7 * 2;

            shift += 2;
            // Координатор канала 2 по координате q qkk2
            var word8 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Qkk2 = (int)word8 * 2;

            shift += 2;
            // Координатор канала 2 по координате fi fikk2
            var word9 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Fikk2 = (int)word9 * 2;

            shift += 2;
            // !!!!!!!!!!!Дальность
            var word10 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Dkp = (int)word10 / 2;

            shift += 6;
            // Уровень сигнала в канале 1 ус1
            var word11 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Yc1 = (int)word11;

            shift += 2;
            // Уровень сигнала в канале 2 ус2
            var word12 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Yc2 = (int)word12;

            shift += 4;
            // Размер поля анализа в канале 1 
            var word13 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].DeltaA1 = (int)word13 * 10;

            shift += 2;
            // Размер поля анализа в канале 2 
            var word14 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].DeltaA2 = (int)word14 * 2;

            shift += 2;
            // Отношение сигнал\порог в канале 1 
            var word15 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].K1Cp = (int)word15;

            shift += 2;
            // Отношение сигнал\порог в канале 2 
            var word16 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].K2Cp = (int)word16;

            shift += 4;
            // Значение фокуса в канале 1 
            var word17 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Focuspc = (int)word17;

            shift += 70;
            // Установка по q  в канале 1 
            var word18 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Lq1 = (int)word18;

            shift += 2;
            // Установка по fi  в канале 1 
            var word19 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Lfi1 = (int)word19;

            shift += 2;
            // Установка по q  в канале 2 
            var word20 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Lq2 = (int)word20;

            shift += 2;
            // Установка по fi  в канале 2
            var word21 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Lfi2 = (int)word21;

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
        /// Проверка пакета на валидность, с поиском окончания пакета.
        /// </summary>
        /// <param name="pStart">Указатель начала признака пакета.</param>
        /// <returns>Возвращает true, если наденный пакет валиден.</returns>
        public bool PackagesIsValid(uint pStart)
        {
            if (ToBigEndian(pStart, 4) == SingBegin)
            {
                uint countedSum = 0;
                uint i = pStart - 4;

                do
                {
                    i += 4;
                    countedSum += ToBigEndian(i, 4);
                } while (ToBigEndian(i, 4) != SingEnd);
                i += 4;
                uint controlSum = ToBigEndian(i, 4);
                if (controlSum == countedSum) return true;

            }
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
