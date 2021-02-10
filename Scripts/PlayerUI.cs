
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] RectTransform thrusterFuelFill;
    private PlayerController controller;

    public void setFuelAmount(float _amount)
    {
        thrusterFuelFill.localScale = new Vector3(1f, _amount, 1f);
    }
    private void Update()
    {
        setFuelAmount(controller.getThrusterFuelAmount());
    }
    public void setController(PlayerController _controller)
    {
        controller = _controller;
    }
}
