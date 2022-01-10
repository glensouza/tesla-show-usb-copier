// ***********************************************************************
// Assembly         : TeslaLightShow
// Author           : Glen Souza
// Created          : 01-04-2022
//
// Last Modified By : Glen Souza
// Last Modified On : 01-07-2022
// ***********************************************************************
// <copyright file="LightShow.cs" company="TeslaLightShow">
//     Copyright (c) SmartImageSoft. All rights reserved.
// </copyright>
// <summary>Main LightShow class to pass data around in memory</summary>
// ***********************************************************************

namespace TeslaLightShow;

public class LightShow
{
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
    public string AudioFile { get; init; } = string.Empty;
}
