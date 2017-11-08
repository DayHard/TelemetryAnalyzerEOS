using System;
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
        public bool Open(object path)
        {
            try
            {
                _data = new byte[(int)new FileInfo((string)path).Length];
                using (BinaryReader bReader = new BinaryReader(File.Open((string)path, FileMode.Open, FileAccess.Read)))
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
            }
            catch
            {
                return false;
            }
            return true;
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
                            DecodePackage(counter, pStart);
                        }
                        else
                        {
                            //!!!!
                            ParamsOedes[counter] = null;
                            ParamsCvses[counter] = null;
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
                                if (!PackagesIsValid(pStart, pEnd) && !DecodePackage(counter, pStart))
                                {
                                    //!!!!
                                    ParamsOedes[counter] = null;
                                    ParamsCvses[counter] = null;
                                    ecounter++;
                                }
                                break;
                            }

                            if (sing == SingBegin)
                            {
                                //!!!!
                                ParamsOedes[counter] = null;
                                ParamsCvses[counter] = null;
                                i -= 4;
                                break;
                            }
                        }
                    }
                    counter++;
                }
            }
            //MessageBox.Show(@"Counted packages: " + counter + @" Bad packages: " + ecounter);
            return true;
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

        private bool DecodePackage(int counter, uint pStart)
        {

            if (!DecodeOedCvs(counter, pStart))
                return false;

            if (!DecodeCvsOed(counter, pStart))
                return false;

            return true;
        }

        //Направление ЦВС -> ОЭД декодирование
        private bool DecodeCvsOed(int counter, uint pStart)
        {
            uint shift = 78 + 62;
            //Номер кадра
            ParamsCvses[counter].FrameNumber = ParamsOedes[counter].FrameNumber;

            //Сигналы
            var word1 = ToBigEndian(pStart + shift, 2);
            //Пуск
            if ((word1 & 0x1) != 0)
                ParamsCvses[counter].Launch = true;
            //Сход
            if ((word1 & 0x2) != 0)
                ParamsCvses[counter].Shod = true;
            //Сопр ЛО1
            if ((word1 & 0x4) != 0)
                ParamsCvses[counter].SoprLO1 = true;
            //Сопр ЛО2
            if ((word1 & 0x8) != 0)
                ParamsCvses[counter].SoprLO2 = true;
            //Откр Кр
            if ((word1 & 0x20) != 0)
                ParamsCvses[counter].Opencap = true;
            //Сброс ПС
            if ((word1 & 0x200) != 0)
                ParamsCvses[counter].Resetpc = true;
            //Режим
            if ((word1 & 0x800) != 0)
                ParamsCvses[counter].Mode = true;
            //Назем цель
            if ((word1 & 0x800) != 0)
                ParamsCvses[counter].Groundtarget = true;

            shift += 2;
            //Установка в канале 1 по q deltaqk1 
            var word2 = ToBigEndian(pStart + shift, 2);
            ParamsCvses[counter].Deltaqk1 = (short)(word2 * 10);

            shift += 2;
            //Установка в канале 1 по fi deltafik1 
            var word3 = ToBigEndian(pStart + shift, 2);
            ParamsCvses[counter].Deltafik1 = (short)(word3 * 10);

            shift += 2;
            //Установка в канале 2 по q deltaqk2 
            var word4 = ToBigEndian(pStart + shift, 2);
            ParamsCvses[counter].Deltaqk2 = (short)(word4 * 2);

            shift += 2;
            //Установка в канале 2 по fi deltafik2 
            var word5 = ToBigEndian(pStart + shift, 2);
            ParamsCvses[counter].Deltafik2 = (short)(word5 * 2);

            shift += 2;
            //Скорость ПОД по координате q qkpod
            var word6 = ToBigEndian(pStart + shift, 2);
            ParamsCvses[counter].Qkpod = (short)(word6 * 2);

            shift += 2;
            //Скорость ПОД по координате fi
            var word7 = ToBigEndian(pStart + shift, 2);
            ParamsCvses[counter].Fikpod = (short)(word7 * 2);

            shift += 2;
            //Программная дальность
            var word8 = ToBigEndian(pStart + shift, 2);
            ParamsCvses[counter].Dkp = (short)(word8 * 0.5);

            shift += 2;
            //ГДУ по координате q
            var word9 = ToBigEndian(pStart + shift, 2);
            ParamsCvses[counter].Qkgdu = (short)word9;

            shift += 2;
            //ГДУ по координате fi
            var word10 = ToBigEndian(pStart + shift, 2);
            ParamsCvses[counter].Fikgdu = (short)word10;

            return true;
        }

        //Направление ОЭД -> ЦВС декодирование
        private bool DecodeOedCvs(int counter, uint pStart)
        {
            uint shift = 8;
            //Номер кадра
            ParamsOedes[counter].FrameNumber = (short)ToBigEndian(pStart + shift, 2);
            shift += 24;
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
                ParamsOedes[counter].Capopen = true;
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
            //Y-CLS-F (не используется)
            if ((word1 & 0x8000) != 0)
                ParamsOedes[counter].YClsF = true;
            shift += 2;

            // Датчик канала 1 по координате q qkd1
            var word2 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Qkd1 = (short)(word2 * 10);

            shift += 2;
            // Датчик канала 1 по координате fi fikd1 
            var word3 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Fikd1 = (short)(word3 * 10);

            shift += 2;
            // Координатор канала 1 по координате q qkk1
            var word4 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Qkk1 = (short)(word4 * 10);

            shift += 2;
            // Координатор канала 1 по координате fi fikk1
            var word5 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Fikk1 = (short)(word5 * 10);

            shift += 2;
            // Датчик канала 2 по координате q qkd2
            var word6 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Qkd2 = (short)(word6 * 2);

            shift += 2;
            // Датчик канала 2 по координате fi fikd2
            var word7 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Fikd2 = (short)(word7 * 2);

            shift += 2;
            // Координатор канала 2 по координате q qkk2
            var word8 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Qkk2 = (short)(word8 * 2);

            shift += 2;
            // Координатор канала 2 по координате fi fikk2
            var word9 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Fikk2 = (short)(word9 * 2);

            shift += 2;
            //Дальность
            var word10 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Dkp = (short)(word10 / 2);

            shift += 2;
            //Угловой размер источника в канале 1 по координате q Lq1
            var word11 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Lq1 = (short) ((word11 & 0xFF) * 10);
            //Угловой размер источника в канале 1 по координате q Lfi1
            ParamsOedes[counter].Lfi1 = (short)((word11 & 0xFF00) * 10);

            shift += 2;
            //Угловой размер источника в канале 2 по координате q Lq2
            var word12 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Lq2 = (short)((word12 & 0xFF) * 10);
            //Угловой размер источника в канале 2 по координате q Lfi2
            ParamsOedes[counter].Lfi2 = (short)((word12 & 0xFF00) * 10);

            shift += 2;
            // Уровень сигнала в канале 1 ус1
            var word13 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Yc1 = (short)word13;

            shift += 2;
            // Уровень сигнала в канале 2 ус2
            var word14 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Yc2 = (short)word14;
             
            //!!! Не проверено
            shift += 2;
            // Количество часов Nch
            var word15 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Nch = (short)word15;

            shift += 2;
            // Размер поля анализа в канале 1 
            var word16 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].DeltaA1 = (short)(word16 * 10);

            shift += 2;
            // Размер поля анализа в канале 2 
            var word17 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].DeltaA2 = (short)(word17 * 2);

            shift += 2;
            // Отношение сигнал\порог в канале 1 
            var word18 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].K1Cp = (short)word18;

            shift += 2;
            // Отношение сигнал\порог в канале 2 
            var word19 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].K2Cp = (short)word19;

            //!!! Не проверено
            shift += 2;
            // Количество циклов
            var word20 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Nc = (short)word20;

            shift += 2;
            // Значение фокуса в канале 1 
            var word21 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Focuspc = (short)word21;
            
            shift += 2;
            // Площадь диафрагмы канала 1 Sdiafr1
            var word22 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Sdiafr1 = (short)word22;

            shift += 2;
            // Время накопления в канале 1 Tn1
            var word23 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Tn1 = (short)word23;

            shift += 2;
            // Текущая температура ОЭД Ttec
            var word24 = ToBigEndian(pStart + shift, 2);
            ParamsOedes[counter].Ttec = (short)(word24 & 0xFF);

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
