using UnityEngine;
using UnityEngine.InputSystem;

public class CanonScript : MonoBehaviour, InputSystem_Actions.ICannonActions
{
    [SerializeField] private GameObject CannonPivot;
    [SerializeField] private GameObject Bullet;
    [SerializeField] private GameObject FirePoint;
    [SerializeField] private GameObject ChargeBar;
    private InputSystem_Actions InputActions;
    private Vector2 aimScreenPosition;
    private Camera mainCamera;

    private bool isCharging = false;
    private float chargePower = 0f;
    private float maxCharge = 12f;
    public float force = 2f;

    public void OnAim(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        aimScreenPosition = context.ReadValue<Vector2>();
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(aimScreenPosition);
        Vector2 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float fixedAngle = Mathf.Clamp(angle, 0, 90);
        CannonPivot.transform.rotation = Quaternion.Euler(0f, 0f, fixedAngle);
    }

    public void OnCharge(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isCharging = true;
        }
        if(context.canceled)
        {
            isCharging = false;
            ShotBullet();
            chargePower = 0f;
            ChargeBar.transform.localScale = new Vector3(0.05f, ChargeBar.transform.localScale.y, 1f);
        }
    }

    private void OnEnable()
    {
        InputActions.Enable();
    }
    private void OnDisable()
    {
        InputActions.Disable();
    }
    void Awake()
    {
        mainCamera = Camera.main;

        InputActions = new InputSystem_Actions();
        InputActions.Cannon.SetCallbacks(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (isCharging)
        {
            chargePower += force * Time.deltaTime;
            chargePower = Mathf.Clamp(chargePower, 0f, maxCharge);
            float porcentaje = chargePower / maxCharge;

            float escalaXFluida = Mathf.Lerp(0f, 100f, porcentaje);

            ChargeBar.transform.localScale = new Vector3(escalaXFluida, ChargeBar.transform.localScale.y, 1f);
        }
    }
    private void ShotBullet()
    {
        GameObject bullet = Instantiate(Bullet, FirePoint.transform.position, transform.rotation);

        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        rbBullet.AddForce(CannonPivot.transform.right * chargePower, ForceMode2D.Impulse);

        Destroy(bullet, 2f);
    }
}
