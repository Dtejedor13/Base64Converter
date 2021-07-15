namespace Base64ConverterCore.Interfaces
{
    interface IMediaConverter
    {
        string ConvertToString(object value);

        object ConvertToObject(string value);
    }
}
