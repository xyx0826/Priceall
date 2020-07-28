namespace Priceall.Http
{
    /// <summary>
    /// Network response from an appraisal service.
    /// </summary>
    class ServiceResponse
    {
        public ServiceStatus Status { get; }

        public string Response { get; }

        public ServiceResponse(ServiceStatus status, string response)
        {
            Status = status;
            Response = response;
        }

        public static ServiceResponse FromError(ServiceStatus status)
            => new ServiceResponse(status, null);
    }

    /// <summary>
    /// The status of a service response.
    /// </summary>
    enum ServiceStatus
    {
        Successful,
        ContentError,
        NetworkError
    }
}
