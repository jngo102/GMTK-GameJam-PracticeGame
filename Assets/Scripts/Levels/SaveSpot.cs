using UnityEngine;

/// <summary>
///     Location that the player may save their game at.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class SaveSpot : MonoBehaviour, IDataPersistence {
    private bool playerInTrigger;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            playerInTrigger = true;
            Save(other.GetComponent<Player>());
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) playerInTrigger = false;
    }

    /// <inheritdoc />
    public void LoadData(SaveData saveData) { }

    /// <inheritdoc />
    public void SaveData(SaveData saveData) {
        if (playerInTrigger) saveData.saveScene = gameObject.scene.name;
    }

    /// <summary>
    ///     Save the game at the save spot.
    /// </summary>
    /// <param name="player">The player who saved at the save spot.</param>
    public static void Save(Player player) {
        player.GetComponent<HealthManager>().FullHeal();
        SaveDataManager.Instance.SaveGame();
    }
}