
namespace Vereinsmeisterschaften.Core.Contracts.Services
{
    /// <summary>
    /// Delegate void for progress changes
    /// </summary>
    /// <param name="sender">Progress sender</param>
    /// <param name="progress">Progress 0 .. 100 </param>
    /// <param name="currentStep">Optional string describing the current step</param>
    public delegate void ProgressDelegate(object sender, float progress, string currentStep = "");

    /// <summary>
    /// Delegate to format data
    /// </summary>
    /// <param name="data">Input object to format</param>
    /// <returns>Return the formated object as string</returns>
    public delegate string FormatDataDelegate(object data);

    /// <summary>
    /// Delegate to format data headers
    /// </summary>
    /// <param name="header">Input header to format</param>
    /// <param name="type">Type of the header object</param>
    /// <returns>Return the formated object as string</returns>
    public delegate string FormatDataHeaderDelegate(string header, Type type);

    /// <summary>
    /// Delegate to change a specific property in the data object
    /// </summary>
    /// <typeparam name="T">Type of the data object</typeparam>
    /// <param name="dataObj">Data object to change</param>
    /// <param name="propertyName">Name of the property in the data object to change</param>
    /// <param name="value">String value with the new value for the property</param>
    public delegate void SetPropertyFromStringDelegate<T>(T dataObj, string propertyName, string value);
}
