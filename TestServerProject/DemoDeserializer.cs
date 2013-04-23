using System;
using System.Collections.Generic;
using System.Globalization;

namespace OccupOSCloud
{
   public static class DemoDeserializer
    {

       private static NumberStyles istyle = NumberStyles.Integer;
       private static NumberStyles fstyle = NumberStyles.Float;
       private static CultureInfo culture = CultureInfo.CreateSpecificCulture("en-GB");

       /*
         * DEMO DESERIALIZE METHODS
         * Temporary (non-general) for the creation of SensorData objects with nodeID. 
         */

        public static ReturnPacket DeserializeJSON(string input) {
            if (input != null && input != "{}") {
                ReturnPacket packet = new ReturnPacket();
                List<ReturnSensor> packetdata = new List<ReturnSensor>();
                input = ReturnNextLayer(input);
                packet.nodeID = int.Parse(GetIdElement(input));
                string[] sensors = CommaSplit(ReturnNextLayer(input));
                foreach (string sensor in sensors) {
                    List<Position> posdata = new List<Position>();
                    ReturnSensor data = new ReturnSensor();
                    data.RefNum = int.Parse(GetIdElement(sensor));
                    string[] readings = CommaSplit(ReturnNextLayer(sensor));
                    foreach (string reading in readings) {
                        string id = GetIdElement(reading);
                        int inum = -1;
                        float fnum = -1;
                        if (!id.Contains("1.")) {
                            switch (id) {
                                case ("a"): data.ReadTime = Convert.ToDateTime(GetDataElement(reading)); break;
                                case ("b"): data.PollTime = Convert.ToDateTime(GetDataElement(reading)); break;
                                case ("0"): int.TryParse(GetDataElement(reading), istyle, culture, out inum); data.EntityCount = inum; break;
                                case ("2"): float.TryParse(GetDataElement(reading), fstyle, culture, out fnum); data.SoundDb = fnum; break;
                                case ("3"): float.TryParse(GetDataElement(reading), fstyle, culture, out fnum); data.AnalogLight = fnum; break;
                                case ("4"): float.TryParse(GetDataElement(reading), fstyle, culture, out fnum); data.VibrationHz = fnum; break;
                                case ("5"): float.TryParse(GetDataElement(reading), fstyle, culture, out fnum); data.Humidity = fnum; break;
                                case ("7"): float.TryParse(GetDataElement(reading), fstyle, culture, out fnum); data.Pressure = fnum; break;
                                case ("8"): float.TryParse(GetDataElement(reading), fstyle, culture, out fnum); data.PowerWatt = fnum; break;
                                case ("9"): float.TryParse(GetDataElement(reading), fstyle, culture, out fnum); data.Temperature = fnum; break;
                                case ("10"): float.TryParse(GetDataElement(reading), fstyle, culture, out fnum); data.Windspeed = fnum; break;
                                default: break;
                            }
                        } else {
                            Position position = new Position();
                            string[] vals = CommaSplit(ReturnNextLayer(reading));
                            int.TryParse(GetDataElement(vals[0]), istyle, culture, out inum);
                            int.TryParse(GetDataElement(vals[1]), istyle, culture, out inum);
                            float.TryParse(GetDataElement(vals[2]), fstyle, culture, out fnum);
                            position.X = inum;
                            position.Y = inum;
                            position.Depth = fnum;
                            posdata.Add(position);
                        }
                    }
                    if (posdata.Count > 0) {
                        Position[] posarray = new Position[posdata.Count];
                        int pd_index = 0;
                        foreach (Position pos in posdata) {
                            posarray[pd_index] = pos;
                            pd_index++;
                        }
                        data.EntityPositions = posarray;
                    }
                    packetdata.Add(data);
                }
                ReturnSensor[] senarray = new ReturnSensor[packetdata.Count];
                int rs_index = 0;
                foreach (ReturnSensor rsdata in packetdata) {
                    senarray[rs_index] = rsdata;
                    rs_index++;
                }
                packet.data = senarray;
                return packet;
            } else return null;
        }

        public static string ReturnNextLayer(string input) {
                int startindex = input.IndexOf("{") + 1;
                int length = input.LastIndexOf("}") - startindex;
                if (startindex == -1 && length == -1) return input;
                if (startindex == -1 || length == -1)
                    throw new ArgumentException("Invalid JSON string format: brace not found");
                input = input.Substring(startindex, length).Trim();
                return input;
            }

        public static string[] CommaSplit(string input) {
            int layer = 0;
            int pos = 0;
            List<string> temp = new List<string>();
            foreach (char c in input) {
                if (c == '{') layer++;
                if (c == '}') layer--;
                if (layer == 0 && c == ',') {
                    temp.Add(input.Substring(0, pos));
                    input = input.Substring(pos + 1, input.Length - (pos + 1));
                    pos = -1;
                }
                    pos++;
            }
            temp.Add(input);
            string[] result = new string[temp.Count];
            pos = 0;
            foreach (string str in temp) {
                result[pos] = str;
                pos++;
            }
            return result;
        }

        public static string GetIdElement(string input) {
            input = input.Substring(0, input.IndexOf(':')).Trim();
            input = input.Substring(1, input.Length - 2);
            return input;
        }

        public static string GetDataElement(string input) {
            input = input.Substring(input.IndexOf(':') + 1, input.Length - (input.IndexOf(':') + 1)).Trim();
            if (input[0] == '\"' && input[input.Length - 1] == '\"')
                input = input.Substring(1, input.Length - 2);
            return input;
        }
    }

    public class ReturnPacket {
        public int nodeID;
        public ReturnSensor[] data;
    }

    public class ReturnSensor {
        public int RefNum;
        public DateTime ReadTime;
        public DateTime PollTime;
        public float AnalogLight = -1;
        public int EntityCount = -1;
        public Position[] EntityPositions = null;
        public float Humidity = -1;
        public float Pressure = -1;
        public float Temperature = -1;
        public float PowerWatt = -1;
        public float SoundDb = -1;
        public float VibrationHz = -1;
        public float Windspeed = -1;
    }

    public class Position {
        public float Depth = -1;
        public int X = -1;
        public int Y = -1;
    }
}
