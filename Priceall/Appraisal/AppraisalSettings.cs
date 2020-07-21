namespace Priceall.Appraisal
{
    abstract class AppraisalSettings
    {

    }

    /// <summary>
    /// A custom settings supported by an appraisal service.
    /// </summary>
    /// <typeparam name="T">The type of the settings value.</typeparam>
    class AppraisalSettings<T> : AppraisalSettings where T : struct
    {
        public string Name { get; }

        public T Value { get; set; }

        public AppraisalSettings(string name, T value = default)
        {
            Name = name;
            Value = value;
        }
    }
}
