////////////////////////////////////////////////////////////////////////////////
//                                                                            
//  File:           DetectGPS
//
//  Facility:       Автомаический поиск GPS устройств.
//
//
//  Abstract:       Класс для поиска GPS устройств.
//
//  Environment:    VC# 8.0
//
//  Author:         Зайцев С.А.
//
//  Creation Date:  05і06.2007
//
//  Copyright (C) OOO "ЛайтКом", 2005-2007. Все права защищены.
//
////////////////////////////////////////////////////////////////////////////////

/*
 * $History: DetectGPS.cs $
 * 
 * *****************  Version 5  *****************
 * User: Sergey       Date: 9.06.07    Time: 9:11
 * Updated in $/LightCom/.NET/GPSDetector
 * 
 * *****************  Version 4  *****************
 * User: Sergey       Date: 8.06.07    Time: 21:38
 * Updated in $/LightCom/.NET/GPSDetector
 * 
 * *****************  Version 3  *****************
 * User: Sergey       Date: 8.06.07    Time: 21:20
 * Updated in $/LightCom/.NET/GPSDetector
 * 
 * *****************  Version 2  *****************
 * User: Sergey       Date: 8.06.07    Time: 8:26
 * Updated in $/LightCom/.NET/GPSDetector
 * 
 * *****************  Version 1  *****************
 * User: Sergey       Date: 5.06.07    Time: 13:42
 * Created in $/LightCom/.NET/GPSDetector
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace LightCom.Gps
{   
    /// <summary>
    /// Автомаический поиск GPS устройств
    /// </summary>
    public class DetectGPS
    {
        public enum EventType
        {
            etPortScanStarted,
            etPortScanFinished,
            etGPSPortFound,
            etGPSPortNotFound,
            etBaudRateScanStarted,
            etBaudRateScanFinished,
            etGPSBaudRateFound,
            etGPSBaudRateNotFound,
            etAbortRequestReceived
        };
        
        /// <summary>
        /// Делегат обработки событий сканирования
        /// </summary>
        /// <param name="et">Тип события</param>
        /// <param name="port">Имя порта</param>
        /// <param name="baudRate">Скорость порта</param>
        public delegate void GPSDetectorEventHandler (EventType et, string port, int baudRate);

        /// <summary>
        /// Класс потока
        /// </summary>
        public class ThreadClass
        {
            /// <summary>
            /// Конструктор класса
            /// </summary>
            /// <param name="sr">Параметры поиска</param>
            /// <param name="handler">Обработчик событий сканирования</param>
            public ThreadClass (SearchResult sr,
                                GPSDetectorEventHandler handler) 
            { 
                searchResult = sr;
                eventHandler = handler;
                abortRequest = false;
            }

            /// <summary>
            /// Запрос на прерывание сканирования
            /// </summary>
            public void SendAbortRequest ()
            {
                abortRequest = true;
            }

            /// <summary>
            /// Порождает событие сканирования
            /// </summary>
            /// <param name="eventType">Тип сообщения</param>
            /// <param name="baudRate">Скорость порта</param>
            void GenerateEvent (EventType eventType, int baudRate)
            {
                if (null == this.eventHandler)
                {
                    return;
                }
                this.eventHandler.Invoke (eventType, this.searchResult.portName, baudRate);
            }
                           
            /// <summary>
            /// Таблица сканируемых скоростей портов
            /// </summary>
            private static int [] baudRates = new int [] { 1200, 1800, 2400, 4800, 7200, 9600, 14400, 19200, 28800, 38400, 56000, 57600 };

            /// <summary>
            /// Функция потока
            /// </summary>
            public void DoWork () 
            {   
                System.IO.Ports.SerialPort port = null;
                try
                {
                    GenerateEvent (EventType.etPortScanStarted, 0);
                    port = new System.IO.Ports.SerialPort (searchResult.portName);
                    try
                    {
                        port.Open ();
                        port.ReadTimeout = 1000;
                        searchResult.baudRates = new List<int> (baudRates.Length);
                        foreach (int baudRate in baudRates)
                        {
                            if (abortRequest)
                            {
                                GenerateEvent (EventType.etAbortRequestReceived, baudRate);
                                return;
                            }

                            try
                            {
                                GenerateEvent (EventType.etBaudRateScanStarted, baudRate);
                                port.BaudRate = baudRate;
                                if (GPSReader.IsValidGPSPort (port))
                                {
                                    searchResult.baudRates.Add (baudRate);
                                    GenerateEvent (EventType.etGPSBaudRateFound, baudRate);
                                }
                                else
                                {
                                    GenerateEvent (EventType.etGPSBaudRateNotFound, baudRate);
                                }
                            }
                            catch (Exception)
                            {
                            }

                            GenerateEvent (EventType.etBaudRateScanFinished, baudRate);
                        }

                        if (null != searchResult.baudRates && searchResult.baudRates.Count > 0)
                        {
                            GenerateEvent (EventType.etGPSPortFound, 0);
                        }
                        else
                        {
                            GenerateEvent (EventType.etGPSPortNotFound, 0);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                finally
                {
                    if (null != port)
                    {
                        port.Close ();
                    }
                    GenerateEvent (EventType.etPortScanFinished, 0);
                }
            }

            /// <summary>
            /// Параметры поиска
            /// </summary>
            private SearchResult searchResult;

            /// <summary>
            /// Запрос на завершение потока
            /// </summary>
            public bool abortRequest;

            /// <summary>
            /// Обработчик событий сканирования
            /// </summary>
            GPSDetectorEventHandler eventHandler;
        }


        /// <summary>
        /// Результат поиска GPS устройств
        /// </summary>
        public struct SearchResult
        {
            /// <summary>
            /// Имя порта
            /// </summary>
            public string portName;

            /// <summary>
            /// Сисок скоростей, на которых рабтает порт
            /// </summary>
            public List<int> baudRates;            
        };

       /// <summary>
       /// Проверяет, выполняется ли сканирование
       /// </summary>
        public bool IsScanning
        {
            get
            {
                if (null == threads)
                {
                    return false;
                }

                foreach (Thread thread in threads)
                {
                    try
                    {
                        if (!thread.Join (0))
                        {
                            return true;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Возвращает список найденных GPS портов
        /// </summary>
        public List<SearchResult> ScanResult
        {   
            get 
            {
                if (IsScanning)
                {
                    return null;
                }

                List<SearchResult> res = new List<SearchResult>();                

                //
                // Обрабатываем результаты
                //

                for (int idx = result.Count; idx >= 0; --idx)
                {
                    if (result [idx].baudRates != null && result [idx].baudRates.Count > 0)
                    {
                        res.Add (result [idx]);
                        continue;
                    }
                    result.RemoveAt (idx);
                }

                return res;
           }
        }

        /// <summary>
        /// Ожидание завершения сканирования
        /// </summary>
        public void WaitForScanCompleted ()
        {
            if (null == threads)
            {
                return;
            }

            //
            // Ждем завершения потоков
            //

            foreach (Thread thread in threads)
            {
                try
                {
                    thread.Join ();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Запуск автоматического поиска GPS устройств.
        /// </summary>
        /// <returns>true, если сканирование начато</returns>
        public bool Start (GPSDetectorEventHandler handler)
        {
            result.Clear ();
            try
            {
                string [] portNames = System.IO.Ports.SerialPort.GetPortNames ();
                if (null == portNames)
                {
                    return false;
                }
                if (portNames.Length < 1)
                {
                    return false;
                }

                threads = new Thread [portNames.Length];
                int idx = 0;
                foreach (string portName in portNames)
                {
                    SearchResult sr = new SearchResult ();
                    sr.portName = portName;
                    result.Add (sr);
                    ThreadClass tc = new ThreadClass (sr, handler);
                    threadContextes.Add (tc);
                    threads [idx] = new Thread (tc.DoWork);
                    threads [idx].Start ();
                    ++idx;
                }

            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Запрос на прерывание сканирования
        /// </summary>
        public void SendAbortRequest ()
        {

            foreach (ThreadClass tc in threadContextes)
            {
                tc.SendAbortRequest ();
            }

        }

        /// <summary>
        /// Останавливает сканирование
        /// </summary>
        /// <param name="waitTimeout">время ожидания завершения потоков (мс)
        /// </param>
        public void Stop (int waitTimeout)
        {
            if (!this.IsScanning)
            {
                return;
            }
            SendAbortRequest ();

            while (this.IsScanning)
            {
                foreach (Thread thread in threads)
                {
                    try
                    {
                        if (!thread.Join (waitTimeout / threads.Length))
                        {
                            thread.Abort ();
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }            
        }

        private Thread [] threads;
        public Thread [] Threads
        {
            get { return threads; }
        }
        private List<SearchResult> result = new List<SearchResult> ();
        private List<ThreadClass> threadContextes = new List<ThreadClass> ();                
    }
}
