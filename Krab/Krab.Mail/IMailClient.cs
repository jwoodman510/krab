namespace Krab.Mail
{
    public interface IMailClient
    {
        void Send(string from, string to, string subject, string type, string value);
    }
}
