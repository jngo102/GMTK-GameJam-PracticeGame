using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TimedDoor : MonoBehaviour {
    [SerializeField] private float doorOpenTime = 2;
        
    private Animator animator;

    private float doorTimer;
    private bool isOpen;
    
    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        doorTimer = doorOpenTime + 1;
    }

    private void Update() {
        if (!isOpen) return;
        doorTimer += Time.deltaTime;
        if (doorTimer >= doorOpenTime && isOpen) {
            animator.Play("Close");
            isOpen = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player") && !isOpen) {
            doorTimer = 0;
            animator.Play("Open");
            isOpen = true;
        }
    }
}
