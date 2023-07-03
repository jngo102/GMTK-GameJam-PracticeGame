/// <summary>
///     Interface for objects that have data to be saved and loaded.
/// </summary>
public interface IDataPersistence {
    /// <summary>
    ///     Load data from a save data instance.
    /// </summary>
    /// <param name="saveData">The save data instance to load from.</param>
    public void LoadData(SaveData saveData);

    /// <summary>
    ///     Save data to a save data instance.
    /// </summary>
    /// <param name="saveData">The save data instance to save to.</param>
    public void SaveData(SaveData saveData);
}