using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    [SerializeField] Camera cam;
    Vector3 velocity = Vector3.zero ;
    Vector3 rotation = Vector3.zero;
    private float camRotationX = 0f;
    private float currentCamRotationX = 0f;
    Vector3 thrusterForce = Vector3.zero;
    [SerializeField] private float cameraRotLimit = 85f;
    Rigidbody rigidbody;
    
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>(); 
    }
    // get vectors from controller
    public void move(Vector3 _velocity)
    {
        velocity = _velocity;
    }
    public void Rotate(Vector3 _rotation)
    {
        rotation = _rotation;
    }
    public void camRotate(float _camrotationX)
    {
        camRotationX = _camrotationX;
    }
    public void ApplyThruster(Vector3 _thrusterForce)
    {
        thrusterForce = _thrusterForce;
    }

    //run every physics iteration
    void FixedUpdate()
    {
        performMovement();
        performRotation();
    }

    void performMovement()
    {
        if (velocity != Vector3.zero)
        {
            rigidbody.MovePosition(rigidbody.position + velocity * Time.fixedDeltaTime);
        }
        if (thrusterForce != Vector3.zero)
        {
            rigidbody.AddForce(thrusterForce*Time.fixedDeltaTime,ForceMode.Acceleration);
        }


    }
    void performRotation()
    {
        rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler(rotation));
        if (cam != null)
        {
            currentCamRotationX -= camRotationX;
            currentCamRotationX = Mathf.Clamp(currentCamRotationX,-cameraRotLimit, cameraRotLimit);

            cam.transform.localEulerAngles = new Vector3(currentCamRotationX,0f,0f);
        }
    }




}
