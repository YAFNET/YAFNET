namespace YAF.Configuration.Pattern;

/// <summary>
///     Provides a method for automatic overriding of a base hash...
/// </summary>
public class RegistryDictionaryOverride : RegistryDictionary
{
    /// <summary>
    ///     Gets or sets a value indicating whether DefaultGetOverride.
    /// </summary>
    public bool DefaultGetOverride { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether DefaultSetOverride.
    /// </summary>
    public bool DefaultSetOverride { get; set; }

    /// <summary>
    ///     Gets or sets OverrideDictionary.
    /// </summary>
    public RegistryDictionary OverrideDictionary { get; set; }

    /// <summary>
    ///     The get value.
    /// </summary>
    /// <param name="name">
    ///     The name.
    /// </param>
    /// <param name="defaultValue">
    ///     The default value.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public override T GetValue<T>(string name, T defaultValue)
    {
        return this.GetValue(name, defaultValue, this.DefaultGetOverride);
    }

    /// <summary>
    ///     The get value.
    /// </summary>
    /// <param name="name">
    ///     The name.
    /// </param>
    /// <param name="defaultValue">
    ///     The default value.
    /// </param>
    /// <param name="allowOverride">
    ///     The allow override.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public virtual T GetValue<T>(string name, T defaultValue, bool allowOverride)
    {
        if (allowOverride
            && this.OverrideDictionary != null
            && this.OverrideDictionary.ContainsKey(name)
            && this.OverrideDictionary[name] != null)
        {
            return this.OverrideDictionary.GetValue(name, defaultValue);
        }

        // just pull the value from this dictionary...
        return base.GetValue(name, defaultValue);
    }

    /// <summary>
    ///     The set value.
    /// </summary>
    /// <param name="name">
    ///     The name.
    /// </param>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    public override void SetValue<T>(string name, T value)
    {
        this.SetValue(name, value, this.DefaultSetOverride);
    }

    /// <summary>
    ///     The set value.
    /// </summary>
    /// <param name="name">
    ///     The name.
    /// </param>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <param name="setOverrideOnly">
    ///     The set override only.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    public virtual void SetValue<T>(string name, T value, bool setOverrideOnly)
    {
        if (this.OverrideDictionary != null)
        {
            if (setOverrideOnly)
            {
                // just set the override dictionary...
                this.OverrideDictionary.SetValue(name, value);
                return;
            }

            if (this.OverrideDictionary.ContainsKey(name) && this.OverrideDictionary[name] != null)
            {
                // set the overriden value to null/erase it...
                this.OverrideDictionary.SetValue(name, (T)Convert.ChangeType(null, typeof(T)));
            }
        }

        // save new value in the base...
        base.SetValue(name, value);
    }
}