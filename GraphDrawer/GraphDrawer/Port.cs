﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphDrawer
{
    public class Port
    {
        private SerialPort _port;
        private Action<DataSet> _packetReceived;
        private DataSet _set = new DataSet(ActionType.Charging);
        public Port(Action<DataSet> packetReceived)
        {
            _port = new SerialPort("COM3");
            _port.Open();
            _packetReceived = packetReceived;
            _port.DataReceived += _port_DataReceived;
            //StartProccess();
        }
        public void StartProccess()
        {
            _port.Write("1");
        }
        public void StopProccess()
        {
            _port.Write("0");
        }
        private void Read()
        {
            while (_port.BytesToRead > 0)
            {
                string value = _port.ReadLine().Replace("\r", "");
                if (value.Contains("-20"))
                {
                    _packetReceived(_set);
                    _set = new DataSet(ActionType.Charging);
                }
                if (value.Contains("-21"))
                {
                    _packetReceived(_set);
                    _set = new DataSet(ActionType.Discharging);
                }
                if (value != "")
                    _set.AddValue(value);
                if(_set.Count%50==0)
                    _packetReceived(_set);
            }
        }
        private void _port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Read();
        }
    }
}
