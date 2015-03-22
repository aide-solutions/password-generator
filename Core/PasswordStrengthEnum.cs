namespace Core
{
    /// <summary>
    /// Password strength enumeration
    /// </summary>
    public enum PasswordStrengthEnum : byte
    {
        NotDefined = 0,
        None       = 1,
        Weak       = 2,
        Medium     = 3,
        Strong     = 4,
        Best       = 5
    }
}