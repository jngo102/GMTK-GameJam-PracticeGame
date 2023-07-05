using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RegainAbility : MonoBehaviour {
    [SerializeField] private AbilityType regainAbilityType;
    
    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.TryGetComponent<Player>(out var player)) return;
        if (player is not InnerPlayer) return;
        switch (regainAbilityType) {
            case AbilityType.DoubleJump:
                GameManager.Instance.OuterPlayer.CanDoubleJump = true;
                break;
            case AbilityType.Glide:
                GameManager.Instance.OuterPlayer.CanGlide = true;
                break;
            case AbilityType.Sprint:
                GameManager.Instance.OuterPlayer.CanSprint = true;
                break;
        }
    }
}
