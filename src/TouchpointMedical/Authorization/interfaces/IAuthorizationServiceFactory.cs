namespace TouchpointMedical.Authorization
{
    public interface IAuthorizationServiceFactory
    {
        IAuthorizationService Get(string serviceName);
    }
}
