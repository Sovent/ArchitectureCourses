namespace Patterns
{
    public interface IUserRegistrar
    {
        void RegisterUser(string login, string password);
    }

    public interface IRedmineGateway
    {
        void CreateUser(string login, string password);
    }

    public interface IGitlabGateway
    {
        void CreateUser(string login, string password);
    }

    public interface INotifier
    {
        void NotifyNewUserAppeared(string login);
    }

    public class FacadeController
    {
        private readonly IUserRegistrar _userRegistrar;
        private readonly IRedmineGateway _redmineGateway;
        private readonly IGitlabGateway _gitlabGateway;
        private readonly INotifier _notifier;

        public FacadeController(
            IUserRegistrar userRegistrar, 
            IRedmineGateway redmineGateway,
            IGitlabGateway gitlabGateway,
            INotifier notifier)
        {
            _userRegistrar = userRegistrar;
            _redmineGateway = redmineGateway;
            _gitlabGateway = gitlabGateway;
            _notifier = notifier;
        }

        public void RegisterNewUser(string login, string password)
        {
            _userRegistrar.RegisterUser(login, password);
            _redmineGateway.CreateUser(login, password);
            _gitlabGateway.CreateUser(login, password);
            _notifier.NotifyNewUserAppeared(login);
        }
    }
}