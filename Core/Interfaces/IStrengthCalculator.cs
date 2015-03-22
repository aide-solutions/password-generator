
namespace Core.Interfaces
{
  
    /// <summary>
    /// Password strength calculator interface contract
    /// </summary>
    public interface IStrengthCalculator
    {
        /// <summary>
        /// Calculates the strength of a password
        /// </summary>
        /// <param name="password">A char array representing the password value</param>
        /// <returns>A byte value</returns>
        byte CalculateStrength(char[] password);
    }
}