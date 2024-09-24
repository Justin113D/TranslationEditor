namespace J113D.TranslationEditor.Data.JSON
{
    public readonly struct JsonProjectValue(string value, bool keepDefault, int valueVersionIndex)
    {
        public string Value { get; } = value;
        public bool KeepDefault { get; } = keepDefault;
        public int ValueVersionIndex { get; } = valueVersionIndex;
    }
}
