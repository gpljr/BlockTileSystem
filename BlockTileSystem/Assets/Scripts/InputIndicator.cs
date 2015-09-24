using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InputIndicator : MonoBehaviour {
    [SerializeField] Image buttonUp;
    [SerializeField] Image buttonDown;
    [SerializeField] Image buttonLeft;
    [SerializeField] Image buttonRight;

    [SerializeField] Sprite upNormal;
    [SerializeField] Sprite downNormal;
    [SerializeField] Sprite leftNormal;
    [SerializeField] Sprite rightNormal;
    [SerializeField] Sprite upPressed;
    [SerializeField] Sprite downPressed;
    [SerializeField] Sprite leftPressed;
    [SerializeField] Sprite rightPressed;

    [SerializeField] float duration = 0.3f;

    void OnEnable () {
        Events.g.AddListener<MoveInputEvent>(MoveInput);
    }
    void OnDisable () {
        Events.g.RemoveListener<MoveInputEvent>(MoveInput);
    }
    void MoveInput (MoveInputEvent e) {
        StartCoroutine(ButtonSet(e.direction));
    }

    IEnumerator ButtonSet (Direction direction) {
        switch (direction) {
            case Direction.North:
                buttonUp.sprite = upPressed;
                yield return new WaitForSeconds(duration);
                buttonUp.sprite = upNormal;
                break;
            case Direction.South:
                buttonDown.sprite = downPressed;
                yield return new WaitForSeconds(duration);
                buttonDown.sprite = downNormal;
                break;
            case Direction.West:
                buttonLeft.sprite = leftPressed;
                yield return new WaitForSeconds(duration);
                buttonLeft.sprite = leftNormal;
                break;
            case Direction.East:
                buttonRight.sprite = rightPressed;
                yield return new WaitForSeconds(duration);
                buttonRight.sprite = rightNormal;
                break;
            default:
                yield return null;
                break;
        }
    }


}
