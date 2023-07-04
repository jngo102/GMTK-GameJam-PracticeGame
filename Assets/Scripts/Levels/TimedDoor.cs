using UnityEngine;

public class TimedDoor : MonoBehaviour {
    [SerializeField] private float doorOpenTime = 2;
    [SerializeField] private float openDoorHeight = 3;
    [SerializeField] private float openCloseTime = 0.25f;
    [SerializeField] private GameObject door;
    
    private float doorTimer;
    private bool isOpen;

    private void Start() {
        doorTimer = doorOpenTime + 1;
    }

    private void Update() {
        if (!isOpen) return;
        doorTimer += Time.deltaTime;
        if (doorTimer >= doorOpenTime && isOpen) {
            iTween.MoveBy(door, Vector3.down * openDoorHeight, openCloseTime);
            isOpen = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player") && !isOpen) {
            doorTimer = 0;
            iTween.MoveBy(door, Vector3.up * openDoorHeight, openCloseTime);
            isOpen = true;
        }
    }
}
