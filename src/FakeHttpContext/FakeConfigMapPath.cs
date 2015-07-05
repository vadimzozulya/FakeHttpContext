namespace FakeHttpContext
{
  using System;
  using System.IO;
  using System.Web.Configuration;

  internal class FakeConfigMapPath : IConfigMapPath
  {
    /// <summary>
    /// Gets the machine-configuration file name.
    /// </summary>
    /// <returns>
    /// The machine-configuration file name.
    /// </returns>
    public string GetMachineConfigFilename()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the name of the configuration file at the Web root.
    /// </summary>
    /// <returns>
    /// The name of the configuration file at the Web root.
    /// </returns>
    public string GetRootWebConfigFilename()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Populates the directory and name of the configuration file based on the site ID and site path.
    /// </summary>
    /// <param name="siteID">A unique identifier for the site.</param><param name="path">The URL associated with the site.</param><param name="directory">The physical directory of the configuration path.</param><param name="baseName">The name of the configuration file.</param>
    public void GetPathConfigFilename(string siteID, string path, out string directory, out string baseName)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Populates the default site name and the site ID.
    /// </summary>
    /// <param name="siteName">The default site name.</param><param name="siteID">A unique identifier for the site.</param>
    public void GetDefaultSiteNameAndID(out string siteName, out string siteID)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Populates the site name and site ID based on a site argument value.
    /// </summary>
    /// <param name="siteArgument">The site name or site identifier.</param><param name="siteName">The default site name.</param><param name="siteID">A unique identifier for the site.</param>
    public void ResolveSiteArgument(string siteArgument, out string siteName, out string siteID)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the physical directory path based on the site ID and URL associated with the site.
    /// </summary>
    /// <returns>
    /// The physical directory path.
    /// </returns>
    /// <param name="siteID">A unique identifier for the site.</param><param name="path">The URL associated with the site.</param>
    public string MapPath(string siteID, string path)
    {
      return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path.TrimStart('/').Replace('/', '\\'));
    }

    /// <summary>
    /// Gets the virtual-directory name associated with a specific site.
    /// </summary>
    /// <returns>
    /// The <paramref name="siteID"/> must be unique. No two sites share the same id. The <paramref name="siteID"/> distinguishes sites that have the same name.
    /// </returns>
    /// <param name="siteID">A unique identifier for the site.</param><param name="path">The URL associated with the site.</param>
    public string GetAppPathForPath(string siteID, string path)
    {
      throw new NotImplementedException();
    }
  }
}