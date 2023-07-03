using System;

/// <summary>
///     Data structure containing all objects with persistent data.
/// </summary>
[Serializable]
public class SaveData {
    /// <summary>
    ///     The player's current health.
    /// </summary>
    public float currentHealth;

    /// <summary>
    ///     The player's maximum health.
    /// </summary>
    public float maxHealth;
    
    /// <summary>
    ///     Input binding overrides that the player has made.
    /// </summary>
    public string bindingOverrides;

    /// <summary>
    ///     The game's default language.
    /// </summary>
    public string language;

    /// <summary>
    ///     The scene of the save spot that the player last saved at.
    /// </summary>
    public string saveScene;

    public SaveData() {
        currentHealth = 5;
        maxHealth = 5;
        bindingOverrides = "";
        language = "en";
        saveScene = "Level1";
    }
}