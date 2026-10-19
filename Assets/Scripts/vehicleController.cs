using UnityEngine;
using UnityEngine.InputSystem;
public class vehicleController : MonoBehaviour
{
    [SerializeField] Transform[] springPoints;
    [SerializeField] Transform[] accelPoints;
    [SerializeField] float restLength;
    [SerializeField] float wheelRadius;
    [SerializeField] float springStrength;
    [SerializeField] float damperStrength;
    [SerializeField] float acceleration;
    [SerializeField] float topSpeed;
    [SerializeField] Rigidbody vehicleRb;
    [SerializeField] float gripFactor;
    [SerializeField] float turnStrength;
    [SerializeField] float maxSteerAngle;
    float moveInput=0f;
    float steerInput=0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        InputControl();
    }
    void FixedUpdate() 
    {
        Suspension();
        Acceleration();
        Steering();
        Grip();
    }
    void InputControl() 
    {
        float wKey = Keyboard.current.wKey.isPressed ? 1f : 0f;
        float sKey = Keyboard.current.sKey.isPressed ? -1f : 0f;
        moveInput = wKey + sKey;
        float dKey = Keyboard.current.dKey.isPressed ? 1f : 0f;
        float aKey = Keyboard.current.aKey.isPressed ? -1f : 0f;
        steerInput = dKey + aKey;
    }
    void Steering() 
    {
        float forwardSpeed = Vector3.Dot(vehicleRb.linearVelocity,transform.forward);
        if (Mathf.Abs(forwardSpeed) > 1f) 
        {
            float directionModifier = Vector3.Dot(transform.forward,vehicleRb.linearVelocity) > 0f ? 1f : -1f;
            float rotationSpeed = turnStrength * steerInput * directionModifier * Time.fixedDeltaTime;
            transform.Rotate(Vector3.up, rotationSpeed);
        }
    }
    void Acceleration() 
    {
        for (int j=0;j<accelPoints.Length;j++) 
        {
            RaycastHit hit;
            if (Physics.Raycast(accelPoints[j].position, -accelPoints[j].up, out hit, restLength + wheelRadius)) 
            {
                Vector3 forwardInput = accelPoints[j].forward*moveInput * acceleration;
                vehicleRb.AddForce(forwardInput,ForceMode.Acceleration);
            }
            
        }
        vehicleRb.linearVelocity = Vector3.ClampMagnitude(vehicleRb.linearVelocity,topSpeed);
    }
    void Suspension() 
    {
        for (int i=0;i<springPoints.Length;i++) 
        {
            RaycastHit hit;
            if (Physics.Raycast(springPoints[i].position, -springPoints[i].up,out hit, restLength + wheelRadius)) 
            {
                float currentSpringLength = hit.distance - wheelRadius;
                float springCompression = (restLength - currentSpringLength) * springStrength;
                float springVelocity = Vector3.Dot(vehicleRb.GetPointVelocity(springPoints[i].position), springPoints[i].up);
                float damperCompression = springVelocity * damperStrength;
                float netForce = springCompression - damperCompression;
                vehicleRb.AddForceAtPosition(netForce * springPoints[i].up, springPoints[i].position);
            }
        }
    }
    void Grip() 
    {
        Vector3 forwardVelocity = transform.forward * Vector3.Dot(vehicleRb.linearVelocity,transform.forward);
        Vector3 lateralVelocity = transform.right * Vector3.Dot(vehicleRb.linearVelocity, transform.right);
        vehicleRb.linearVelocity = forwardVelocity + (lateralVelocity * gripFactor);
    }
}
