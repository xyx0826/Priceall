namespace Priceall.Appraisal
{
    /// <summary>
    /// An appraisal result.
    /// </summary>
    class AppraisalResult
    {
        public AppraisalStatus Status;

        public float Value;
    }

    /// <summary>
    /// The status of an appraisal result.
    /// </summary>
    enum AppraisalStatus
    {
        Successful,
        ContentError,
        NetworkError,
        InternalError
    }
}
