namespace MedicalBoard.Domain.Exceptions;

public class DomainRuleViolationException(string message) : Exception(message){}