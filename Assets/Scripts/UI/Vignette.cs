using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(RectTransform))]
public class Vignette : BaseUI {
    private RectTransform rectTransform;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update() {
        FollowOuterPlayer();
    }

    public override void Open() {
        base.Open();
    }

    public void FollowOuterPlayer() {
        rectTransform.anchoredPosition = GameManager.Instance.OuterPlayer.transform.position;
    }
}