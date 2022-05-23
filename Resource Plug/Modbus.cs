using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resource_Plug {
    static class Modbus {
        public static byte[] GetData(byte[] buff) {
            if (buff.Length > byte.MaxValue) {
                throw new Exception("size of modbus data is too long");
            }
            byte[] bytes = new byte[8 + buff.Length + 1];
            //add drvice
            bytes[0] = 0xA1;
            bytes[1] = 0x00;
            //sensor data conut
            bytes[2] = 0x00;
            bytes[3] = 0xFF;
            //display direction
            bytes[4] = 0x00;
            //nell flag
            bytes[5] = 0x00;
            //data size
            bytes[6] = Convert.ToByte(buff.Length);
            //data start flag
            bytes[7] = 0xAB;
            //data
            for (int i = 0; i < buff.Length; i++) {
                bytes[i + 8] = buff[i];
            }

            bytes[8 + buff.Length] = CRC.Crc8(bytes, 0, 8 + buff.Length - 1)[0];


            return bytes;
        }
    }
}
