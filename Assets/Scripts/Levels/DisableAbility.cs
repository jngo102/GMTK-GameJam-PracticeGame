using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DisableAbility : MonoBehaviour {
    [SerializeField] private AbilityType disableAbilityType;
    
    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.TryGetComponent<Player>(out var player)) return;
        if (player is not OuterPlayer outerPlayer) return;
        switch (disableAbilityType) {
            case AbilityType.DoubleJump:
                outerPlayer.CanDoubleJump = false;
                // TODO: Spawn enemies at legs
                break;
            case AbilityType.Glide:
                outerPlayer.CanGlide = false;
                // TODO: Spawn enemies at arms
                break;
            case AbilityType.Sprint:
                outerPlayer.CanSprint = false;
                // TODO: Spawn enemies at heart
                break;
        }
    }
}
