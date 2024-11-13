using UnityEngine;
using UnityEngine.UI;


public class GrapplingGun : MonoBehaviour
{
    [Header("Scripts Ref:")]
    public GrapplingRope grappleRope;
    public PlayerController playerController;

    [Header("Player Ref:")]
    public Player p;

    [Header("Layers Settings:")]
    [SerializeField] private LayerMask grappableLayerMask;

    [Header("Main Camera:")]
    public Camera m_camera;

    [Header("Transform Ref:")]
    public Transform gunHolder;
    public Transform gunPivot;
    public Transform firePoint;

    [Header("Physics Ref:")]
    public SpringJoint2D m_springJoint2D;
    public Rigidbody2D m_rigidbody;

    [Header("Distance:")]
    [SerializeField] private float maxDistance = 20;

    private enum LaunchType
    {
        Transform_Launch,
        Physics_Launch
    }

    [Header("Launching:")]
    [SerializeField] private LaunchType launchType = LaunchType.Physics_Launch;
    [SerializeField] private float launchSpeed = 1;


    [HideInInspector] public Vector2 grapplePoint;
    [HideInInspector] public Vector2 grappleDistanceVector;

    [HideInInspector] public bool isGrappling;
    private GameObject grappledObject;

    //public Texture2D defaultCursor;
    //public Texture2D specialCursor;
    [HideInInspector] public GameObject Canvas;
    [HideInInspector] public GameObject grappleTargetIndicator;
    [HideInInspector] public Image grappleTargetIndicatorImg;

    [Header("Grapple Forgiveness")]
    public float GrappleRadius; // set 'thickness' of grapple detection for easier use

    private GameManager GM;

    private void Awake()
    {
        Canvas = GameObject.FindGameObjectWithTag("Canvas");
        m_camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        grappleTargetIndicator = Canvas.transform.Find("GrappleIndicator").gameObject;
        grappleTargetIndicatorImg = grappleTargetIndicator.GetComponent<Image>();
        GM = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<GameManager>();
    }

    private void Start()
    {
        // Cursor.visible = false;
        grappleRope.enabled = false;
        m_springJoint2D.enabled = false;
    }

    private void Update()
    {
        Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
        RotateGun(mousePos); ////////////////// same thing?
        updateCursorLook();
        if (isGrappling && grappleRope.enabled)
        {
            if (grappledObject != null && grappledObject.layer == LayerMask.NameToLayer("Enemy")) 
            {
                GM.ChecklistScript.UpdateChecklistItem("GE", true);
                grapplePoint = grappledObject.transform.position;
            }
            if (grappledObject == null)
            {
                stopGrappling(); // if enemy dies as player is grappling it
            }
        } else {
            GrappleIndication();
        }
    }

    // shows where grapple target/latch onto (if doesn't exist on a surface, then can't be grappled)
    public void GrappleIndication() {
        Vector2 direction = (m_camera.ScreenToWorldPoint(Input.mousePosition) - gunPivot.position).normalized;
        RaycastHit2D _hit = Physics2D.CircleCast(firePoint.position, GrappleRadius, direction, maxDistance, grappableLayerMask);

        // Update the LineRenderer to show the projected grapple point
        if (_hit) {
            Vector3 endPoint = _hit.point;
            grappleTargetIndicator.SetActive(true);
            grappleTargetIndicator.transform.position = m_camera.WorldToScreenPoint(endPoint);
        } else {
            grappleTargetIndicator.SetActive(false);
        }
    }

    public void pull()
    {
        if (grappleRope.enabled)
        {
            RotateGun(grapplePoint);
        } else {
            Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
            RotateGun(mousePos);  ///////////////// same thing?
        }

        if (isGrappling)
        {
            launch();
        }
    }

    public void stopPulling()
    {
        Vector2 releaseVelocity = m_rigidbody.velocity;
        m_springJoint2D.enabled = false;
        //m_rigidbody.gravityScale = 1;
        m_rigidbody.velocity = releaseVelocity; //momentum
        isGrappling = false;
        playerController.p.anim.SetBool("midJump", false);
    }

    public void SetSpring(bool isGrounded)
    {
        if (isGrappling && (gunHolder.position.y < grapplePoint.y) && !p.isGrounded && Mathf.Abs(gunHolder.GetComponent<Rigidbody2D>().velocity.x) < 5)
        {
            m_springJoint2D.autoConfigureDistance = false;
            m_springJoint2D.connectedAnchor = grapplePoint;
            Vector2 distanceVector = grapplePoint - (Vector2)gunHolder.position;
            m_springJoint2D.distance = distanceVector.magnitude;
            m_springJoint2D.frequency = 0;
            m_springJoint2D.enabled = true;
        } else if (!isGrappling || !(gunHolder.position.y < grapplePoint.y) || p.isGrounded)
        {
            stopPulling();
        }
    }

    private void RotateGun(Vector3 lookPoint)
    {
        Vector3 distanceVector = lookPoint - gunPivot.position;
        float angle = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;
        gunPivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void SetGrapplePoint() {
        Vector2 direction = (m_camera.ScreenToWorldPoint(Input.mousePosition) - gunPivot.position).normalized;
        RaycastHit2D _hit = Physics2D.CircleCast(firePoint.position, GrappleRadius, direction, maxDistance, grappableLayerMask);

        if (_hit)
        {
            grapplePoint = _hit.point;
            grappleDistanceVector = grapplePoint - (Vector2)gunPivot.position;
            grappleRope.enabled = true;
            grappledObject = _hit.collider.gameObject;
        }
    }

    public void stopGrappling()
    {
        grappleRope.enabled = false;
        stopPulling();
        isGrappling = false;
    }

    public void launch()
    {
        m_springJoint2D.autoConfigureDistance = false;
        m_springJoint2D.connectedAnchor = grapplePoint;

        if (launchType == LaunchType.Physics_Launch)
        {
            Vector2 distanceVector = firePoint.position - gunHolder.position;
            m_springJoint2D.distance = distanceVector.magnitude;
            m_springJoint2D.frequency = launchSpeed;
            m_springJoint2D.enabled = true;
        }
        else if (launchType == LaunchType.Transform_Launch)
        {
            m_rigidbody.gravityScale = 0;
            m_rigidbody.velocity = Vector2.zero;
            Vector2 firePointDistance = firePoint.position - gunHolder.localPosition;
            Vector2 targetPos = grapplePoint - firePointDistance;
            gunHolder.position = Vector2.Lerp(gunHolder.position, targetPos, Time.deltaTime * (launchSpeed + 4));
            m_springJoint2D.enabled = false;
        }

    }

    public void PullEnemy()
    {
        if (isGrappling)
        {
            if (grappledObject != null && grappledObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                SpringJoint2D enemySpringJoint = grappledObject.GetComponent<SpringJoint2D>();
                if (enemySpringJoint != null)
                {
                    enemySpringJoint.connectedAnchor = firePoint.position;
                    enemySpringJoint.distance = 0;
                    enemySpringJoint.enabled = true;
                }
            }
        }
    }

    public void StopPullingEnemy()
    {
        if (isGrappling) 
        {
            if (grappledObject != null && grappledObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                SpringJoint2D enemySpringJoint = grappledObject.GetComponent<SpringJoint2D>();
                if (enemySpringJoint != null)
                {
                    enemySpringJoint.enabled = false;
                }
            }
            stopPulling();
        }
    }

    private void updateCursorLook()
    {
        // Move the cursor image to follow the mouse position
        Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
        if (!grappleRope.enabled)
        {
            grappleTargetIndicator.transform.position = Input.mousePosition; 
        } else {
            grappleTargetIndicator.transform.position = m_camera.WorldToScreenPoint(grapplePoint);
        }

        // Calculate direction and angle to rotate cursor
        Vector2 aimDirection = (mousePos - (Vector2)firePoint.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        grappleTargetIndicator.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);

    }
}
