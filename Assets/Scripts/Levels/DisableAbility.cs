using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DisableAbility : MonoBehaviour {
    [SerializeField] private DisableAbilityType disableAbilityType;
    
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.TryGetComponent<Player>(out var player)) {
            if (player is InnerPlayer innerPlayer) {
                
            } else if (player is OuterPlayer outerPlayer) {
                switch (disableAbilityType) {
                    case DisableAbilityType.DoubleJump:
                        outerPlayer.CanDoubleJump = false;
                        // TODO: Spawn enemies at legs
                        break;
                    case DisableAbilityType.Glide:
                        outerPlayer.CanGlide = false;
                        // TODO: Spawn enemies at arms
                        break;
                    case DisableAbilityType.Sprint:
                        outerPlayer.CanSprint = false;
                        // TODO: Spawn enemies at heart
                        break;
                }
            }
        }
    }
}
