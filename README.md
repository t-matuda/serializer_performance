# serializer_performance

- TestData
 - 3 string values and 1 double value
 - 151,110 records

|Library|Serialize[ms]|Size[byte]|Deserialize[ms]|
|:---|:---|:---|:---|
|utf8Json|274|10,452,066|483|
|JSON.NET|636|10,539,251|818|
|MessagePack C#|195|3,149,590|279|
|MessagePack C# LZ4|197|25,903|116|
|BinaryFormatter|377|5,591,439|1,667|
|BinaryFormatter Deflate|622|780,848|#N/A|