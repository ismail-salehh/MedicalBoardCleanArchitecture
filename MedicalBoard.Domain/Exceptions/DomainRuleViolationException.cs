namespace MedicalBoard.Domain.Exceptions;

public class DomainRuleViolationException : Exception
{
    public DomainRuleViolationException(string message) : base(message) { }
}
