using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Utf8JsonTest
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var testData = JsonTest.MakeSwaptionVolatilities();
            byte[] serializedData;
            using (var sw = new SwEx("Serialize by utf8json"))
            {
                serializedData = new JsonTest().SerializeByUtf8Json(testData);
            }
            File.WriteAllBytes(@"c:\workspace\vols.txt", serializedData);

            using (var sw = new SwEx("Serialize by JSON.NET"))
            {
                serializedData = new JsonTest().SerializeByJsonNet(testData);
            }
            File.WriteAllBytes(@"c:\workspace\volsNet.txt", serializedData);


            using (var sw = new SwEx("Serialize by MsgPack"))
            {
                serializedData = new JsonTest().SerializeByMsgPack(testData);
            }
            File.WriteAllBytes(@"c:\workspace\volsMsgPack.dat", serializedData);

            using (var sw = new SwEx("Serialize by MsgPack LZ4"))
            {
                serializedData = new JsonTest().SerializeByMsgPackLZ4(testData);
            }
            File.WriteAllBytes(@"c:\workspace\volsMsgPackLZ4.dat", serializedData);

            using (var sw = new SwEx("Serialize by BinaryFormatter"))
            {
                serializedData = new JsonTest().SerializeByBinaryFormatter(testData);
            }
            File.WriteAllBytes(@"c:\workspace\volsBF.dat", serializedData);

            using (var sw = new SwEx("Serialize by BinaryFormatter with compression"))
            {
                serializedData = new JsonTest().SerializeByBinaryFormatterDeflate(testData);
            }
            File.WriteAllBytes(@"c:\workspace\volsBFDeflate.dat", serializedData);


            var json1 = File.ReadAllBytes(@"C:\workspace\vols.txt");
            using (var sw = new SwEx("Deserialize by utf8Json"))
            {
                new JsonTest().DeserializeByUtf8Json(json1);
            }

            var json2 = File.ReadAllText(@"C:\workspace\vols.txt");
            using (var sw = new SwEx("Deserialize by Json.NET"))
            {
                new JsonTest().DeserializeByJsonNet(json2);
            }

            var data1 = File.ReadAllBytes(@"c:\workspace\volsMsgPack.dat");
            using (var sw = new SwEx("Deserialize by MessagePack"))
            {
                new JsonTest().DeserializeByMsgPack(data1);
            }

            var data2 = File.ReadAllBytes(@"c:\workspace\volsMsgPackLZ4.dat");
            using (var sw = new SwEx("Deserialize by MessagePack LZ4"))
            {
                new JsonTest().DeserializeByMsgPackLZ4(data2);
            }

            var data3 = File.ReadAllBytes(@"c:\workspace\volsBF.dat");
            using (var sw = new SwEx("Deserialize by BinaryFormatter"))
            {
                new JsonTest().DeserializeByBinaryFormatter(data3);
            }

            var data4 = File.ReadAllBytes(@"c:\workspace\volsBFDeflate.dat");
            using (var sw = new SwEx("Deserialize by BinaryFormatter with compression"))
            {
                new JsonTest().DeserializeByBinaryFormatterDeflate(data4);
            }


            //Console.WriteLine("Size = {0}", serializedData.Length);
            Console.WriteLine(Process.GetCurrentProcess().PeakWorkingSet64 / 1024d / 1024d);
        }
    }



    public class JsonTest
    {
        public void DeserializeByUtf8Json(byte[] json)
        {
            var results = Utf8Json.JsonSerializer.Deserialize<SwaptionVolatility[]>(json);
            Console.WriteLine(results.Count());
            Console.WriteLine(results.Last().Value);
        }

        public void DeserializeByJsonNet(string json)
        {
            var results = Newtonsoft.Json.JsonConvert.DeserializeObject<SwaptionVolatility[]>(json);
            Console.WriteLine(results.Count());
            Console.WriteLine(results.Last().Value);
        }

        public void DeserializeByMsgPack(byte[] data)
        {
            var results = MessagePack.MessagePackSerializer.Deserialize<SwaptionVolatility[]>(data);
            Console.WriteLine(results.Count());
            Console.WriteLine(results.Last().Value);
        }

        public void DeserializeByMsgPackLZ4(byte[] data)
        {
            var results = MessagePack.LZ4MessagePackSerializer.Deserialize<SwaptionVolatility[]>(data);
            Console.WriteLine(results.Count());
            Console.WriteLine(results.Last().Value);
        }

        public void DeserializeByBinaryFormatter(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                var results = (SwaptionVolatility[])(new BinaryFormatter().Deserialize(stream));
                Console.WriteLine(results.Count());
                Console.WriteLine(results.Last().Value);
            }
        }

        public void DeserializeByBinaryFormatterDeflate(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            using (var deflate = new DeflateStream(stream, CompressionMode.Decompress))
            {
                var results = (SwaptionVolatility[])(new BinaryFormatter().Deserialize(deflate));
                Console.WriteLine(results.Count());
                Console.WriteLine(results.Last().Value);
            }
        }

        public byte[] SerializeByUtf8Json(IEnumerable<SwaptionVolatility> data)
        {
            return Utf8Json.JsonSerializer.Serialize<IEnumerable<SwaptionVolatility>>(data);
        }

        public byte[] SerializeByJsonNet(IEnumerable<SwaptionVolatility> data)
        {
            return Encoding.GetEncoding("SJIS").GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(data));
        }

        public byte[] SerializeByMsgPack(IEnumerable<SwaptionVolatility> data)
        {
            return MessagePack.MessagePackSerializer.Serialize<IEnumerable<SwaptionVolatility>>(data);
        }

        public byte[] SerializeByMsgPackLZ4(IEnumerable<SwaptionVolatility> data)
        {
            return MessagePack.LZ4MessagePackSerializer.Serialize<IEnumerable<SwaptionVolatility>>(data);
        }

        public byte[] SerializeByBinaryFormatter(SwaptionVolatility[] data)
        {
            using (var stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, data);
                return stream.ToArray();
            }
        }

        public byte[] SerializeByBinaryFormatterDeflate(SwaptionVolatility[] data)
        {
            using (var stream = new MemoryStream())
            using (var deflate = new DeflateStream(stream, CompressionMode.Compress))
            {
                new BinaryFormatter().Serialize(deflate, data);
                return stream.ToArray();
            }
        }


        public static SwaptionVolatility[] MakeSwaptionVolatilities()
        {
            var testData = MakeTestData().Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData())
                .Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).Union(MakeTestData()).ToArray();
            Console.WriteLine(testData.Count());

            return testData;
        }


        static IEnumerable<SwaptionVolatility> MakeTestData()
        {
            var result = new List<SwaptionVolatility>();
            result.AddRange(MakeTestData("6M"));
            result.AddRange(MakeTestData("1Y"));
            result.AddRange(MakeTestData("2Y"));
            result.AddRange(MakeTestData("3Y"));
            result.AddRange(MakeTestData("4Y"));
            result.AddRange(MakeTestData("5Y"));
            result.AddRange(MakeTestData("6Y"));
            result.AddRange(MakeTestData("7Y"));
            result.AddRange(MakeTestData("8Y"));
            result.AddRange(MakeTestData("9Y"));
            result.AddRange(MakeTestData("10Y"));
            result.AddRange(MakeTestData("11Y"));
            result.AddRange(MakeTestData("12Y"));
            result.AddRange(MakeTestData("13Y"));
            result.AddRange(MakeTestData("14Y"));
            result.AddRange(MakeTestData("15Y"));
            result.AddRange(MakeTestData("16Y"));
            result.AddRange(MakeTestData("17Y"));
            result.AddRange(MakeTestData("18Y"));
            result.AddRange(MakeTestData("19Y"));
            result.AddRange(MakeTestData("20Y"));
            result.AddRange(MakeTestData("25Y"));
            result.AddRange(MakeTestData("30Y"));
            return result;

        }

        static IEnumerable<SwaptionVolatility> MakeTestData(string tenor)
        {
            var rnd = new Random();
            var result = new List<SwaptionVolatility>();
            result.Add(new SwaptionVolatility { Ccy = "JPY", Tenor = tenor, Expiry = "1M", Value = rnd.NextDouble() });
            result.Add(new SwaptionVolatility { Ccy = "JPY", Tenor = tenor, Expiry = "2M", Value = rnd.NextDouble() });
            result.Add(new SwaptionVolatility { Ccy = "JPY", Tenor = tenor, Expiry = "3M", Value = rnd.NextDouble() });
            result.Add(new SwaptionVolatility { Ccy = "JPY", Tenor = tenor, Expiry = "6M", Value = rnd.NextDouble() });
            result.Add(new SwaptionVolatility { Ccy = "JPY", Tenor = tenor, Expiry = "1Y", Value = rnd.NextDouble() });
            result.Add(new SwaptionVolatility { Ccy = "JPY", Tenor = tenor, Expiry = "2Y", Value = rnd.NextDouble() });
            result.Add(new SwaptionVolatility { Ccy = "JPY", Tenor = tenor, Expiry = "3Y", Value = rnd.NextDouble() });
            result.Add(new SwaptionVolatility { Ccy = "JPY", Tenor = tenor, Expiry = "4Y", Value = rnd.NextDouble() });
            result.Add(new SwaptionVolatility { Ccy = "JPY", Tenor = tenor, Expiry = "5Y", Value = rnd.NextDouble() });
            result.Add(new SwaptionVolatility { Ccy = "JPY", Tenor = tenor, Expiry = "6Y", Value = rnd.NextDouble() });
            result.Add(new SwaptionVolatility { Ccy = "JPY", Tenor = tenor, Expiry = "7Y", Value = rnd.NextDouble() });
            result.Add(new SwaptionVolatility { Ccy = "JPY", Tenor = tenor, Expiry = "8Y", Value = rnd.NextDouble() });
            result.Add(new SwaptionVolatility { Ccy = "JPY", Tenor = tenor, Expiry = "9Y", Value = rnd.NextDouble() });
            result.Add(new SwaptionVolatility { Ccy = "JPY", Tenor = tenor, Expiry = "10Y", Value = rnd.NextDouble() });
            result.Add(new SwaptionVolatility { Ccy = "JPY", Tenor = tenor, Expiry = "15Y", Value = rnd.NextDouble() });
            result.Add(new SwaptionVolatility { Ccy = "JPY", Tenor = tenor, Expiry = "20Y", Value = rnd.NextDouble() });
            result.Add(new SwaptionVolatility { Ccy = "JPY", Tenor = tenor, Expiry = "25Y", Value = rnd.NextDouble() });
            result.Add(new SwaptionVolatility { Ccy = "JPY", Tenor = tenor, Expiry = "30Y", Value = rnd.NextDouble() });

            return result;
        }

    }
}
