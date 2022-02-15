// ***********************************************************************
// Assembly         : TeslaLightShow
// Author           : Glen Souza
// Created          : 01-04-2022
//
// Last Modified By : Glen Souza
// Last Modified On : 01-07-2022
// ***********************************************************************
// <summary>Main LightShow class to pass data around in memory</summary>
// ***********************************************************************

namespace TeslaLightShow.Models;

public class LightShow
{
    /// <summary>
    /// Gets or sets the show number.
    /// </summary>
    /// <value>The show number.</value>
    public int Number { get; init; }
    
    /// <summary>
    /// Gets or sets the name of the show.
    /// </summary>
    /// <value>The name of the show.</value>
    public string ShowName { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the sequence file.
    /// </summary>
    /// <value>The sequence file for the show.</value>
    public string SequenceFile { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the audio file.
    /// </summary>
    /// <value>The audio file for the show.</value>
    public string AudioFile { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="LightShow"/> is included.
    /// </summary>
    /// <value><c>true</c> if include; otherwise, <c>false</c>.</value>
    public bool Include { get; set; }
}
