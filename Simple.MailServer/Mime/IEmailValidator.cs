namespace Simple.MailServer.Mime
{
    public interface IEmailValidator
    {
        bool Validate(string email);
    }
}
