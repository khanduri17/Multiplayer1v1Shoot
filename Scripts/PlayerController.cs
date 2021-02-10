using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask EnvironmentLayerMask;
    [SerializeField] private float speed = 5f;   
    [SerializeField] private float lookSensitivity = 3f;
    [SerializeField] private float thrusterForce = 1000f;
    [SerializeField] private float thrusterFuelBurnSpeed = 1f;
    [SerializeField] private float thrusterFuelRegenSpeed = 0.3f;
    [SerializeField] private float thrusterFuelAmount = 1f; 

    [Header("Spring Settings:")]
    [SerializeField] private JointProjectionMode jointMode = JointProjectionMode.PositionAndRotation;
    [SerializeField] private float jointSpring=20f;
    [SerializeField] private float jointMaxForce=40f;

    private PlayerMotor motor;
    private ConfigurableJoint joint;
    private Animator animator;

    public float getThrusterFuelAmount()
    {
        return thrusterFuelAmount;
    }



    void Start()
    {
        animator = GetComponent<Animator>();
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();
        setJointSettings(jointSpring);
    }
   
    
    void Update()
    {
        RaycastHit _hit;
        if (Physics.Raycast(transform.position, Vector3.down, out _hit, 100f, EnvironmentLayerMask))
        {
            joint.targetPosition = new Vector3(0f, -_hit.point.y, 0f);
        }
        else
        {
            joint.targetPosition = new Vector3(0f, 0f, 0f);
        }



        float _xMove = Input.GetAxis("Horizontal");
        float _zMove = Input.GetAxis("Vertical");
        Vector3 _moveHorizontal = transform.right * _xMove;
        Vector3 _moveVertical = transform.forward * _zMove;

        Vector3 _velocity = (_moveHorizontal + _moveVertical) * speed; 

        //apply movement

        motor.move(_velocity);

        //animate movement
        animator.SetFloat("ForwardVelocity", _zMove);

        //calculate rotation as a 3d vector(turning around)

        float _yrot = Input.GetAxisRaw("Mouse X");
        Vector3 _rotation = new Vector3(0f, _yrot, 0f) * lookSensitivity;

        //apply rotation 
        motor.Rotate(_rotation);

        //calculate  camera rotation as a 3d vector(turning around)
        float _xrot = Input.GetAxisRaw("Mouse Y");
        float _camRotation =  _xrot * lookSensitivity;

        //apply rotation
        motor.camRotate(_camRotation);

       //calculate thruster force based on player input
        Vector3 _thrusterForce = Vector3.zero;

        if (Input.GetButton("Jump") && thrusterFuelAmount > 0)
        {
            thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime;
            if (thrusterFuelAmount > 0.01f)
            {
                _thrusterForce = Vector3.up * thrusterForce;
                setJointSettings(0f);
            }
        }
        else
        {
            thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime;
            setJointSettings(jointSpring);
        }

        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0f, 1f);

        //apply thruster force
        motor.ApplyThruster(_thrusterForce);
        }

    private void setJointSettings(float _jointSpring)
    {
        joint.projectionMode = jointMode;
        joint.yDrive = new JointDrive
        {
            positionSpring = _jointSpring,
            maximumForce = jointMaxForce,
            
    };
    }



}
